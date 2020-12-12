using DungeonGame.Items;
using DungeonGame.Items.Consumables;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DungeonGame.Controllers
{
	public static class PlayerController
	{
		public static bool OutOfArrows(Player player)
		{
			return !player._PlayerQuiver.HaveArrows();
		}
		public static void LookAtObject(Player player, string[] input)
		{
			var parsedInput = InputController.ParseInput(input);
			var playerRoom = RoomController.Rooms[player._PlayerLocation];
			var roomMatch = playerRoom._RoomObjects.FindIndex(f =>
				f._Name.Contains(parsedInput));
			if (roomMatch != -1)
			{
				playerRoom.LookNpc(input, player);
				return;
			}
			var playerInvIndex = player._Inventory.FindIndex(f => f._Name.Contains(input[1]) ||
																				 f._Name == parsedInput);
			if (playerInvIndex != -1)
			{
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					player._Inventory[playerInvIndex]._Desc);
				return;
			}
			var playerConsIndex = player._Consumables.FindIndex(f => f._Name.Contains(input[1]) ||
																	f._Name == parsedInput);
			if (playerConsIndex != -1)
			{
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					player._Consumables[playerConsIndex]._Desc);
			}
		}
		public static int GetInventoryWeight(Player player)
		{
			return player._Inventory.Sum(item => item._Weight) +
				   player._Consumables.Sum(consumable => consumable._Weight);
		}
		public static void ShowInventory(Player player)
		{
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				"Your inventory contains:");
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			foreach (var item in player._Inventory)
			{
				if (!item._Equipped) continue;
				var itemName = GearController.GetItemDetails(item);
				var itemInfo = new StringBuilder(itemName);
				if (itemName.Contains("Quiver"))
					itemInfo.Append(" (Arrows: " + player._PlayerQuiver._Quantity + "/" + player._PlayerQuiver._MaxQuantity + ")");
				itemInfo.Append(" <_Equipped>");
				if (item is Armor || item is Weapon)
				{
					var playerItem = item as IRainbowGear;
					if (playerItem._IsRainbowGear)
					{
						GearController.StoreRainbowGearOutput(itemInfo.ToString());
						continue;
					}
				}
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					itemInfo.ToString());
			}
			foreach (var item in player._Inventory)
			{
				if (item._Equipped) continue;
				var itemName = GearController.GetItemDetails(item);
				var itemInfo = new StringBuilder(itemName);
				if (player._PlayerQuiver?._Name == itemName)
					itemInfo.Append("Arrows: " + player._PlayerQuiver._Quantity + "/" + player._PlayerQuiver._MaxQuantity);
				if (item is Armor || item is Weapon)
				{
					var playerItem = item as IRainbowGear;
					if (playerItem._IsRainbowGear)
					{
						GearController.StoreRainbowGearOutput(itemInfo.ToString());
						continue;
					}
				}
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					itemInfo.ToString());
			}
			var consumableDict = new Dictionary<string, int>();
			foreach (var item in player._Consumables)
			{
				var itemInfo = new StringBuilder();
				itemInfo.Append(item._Name);
				if (item._Name.Contains("potion"))
				{
					switch (item._PotionCategory)
					{
						case Consumable.PotionType.Health:
							itemInfo.Append(" (+" + item._RestoreHealth._RestoreHealthAmt + " " + item._PotionCategory + ")");
							break;
						case Consumable.PotionType.Mana:
							itemInfo.Append(" (+" + item._RestoreMana._RestoreManaAmt + " " + item._PotionCategory + ")");
							break;
						case Consumable.PotionType.Intelligence:
						case Consumable.PotionType.Strength:
						case Consumable.PotionType.Dexterity:
						case Consumable.PotionType.Constitution:
							itemInfo.Append(" (+" + item._ChangeStat._ChangeAmount + " " + item._PotionCategory + ")");
							break;
						case null:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				if (item.GetType() == typeof(Arrows))
				{
					Arrows arrows = item as Arrows;
					itemInfo.Append(" (" + arrows._Quantity + ")");
				}
				var itemName = textInfo.ToTitleCase(itemInfo.ToString());
				if (!consumableDict.ContainsKey(itemName))
				{
					consumableDict.Add(itemName, 1);
					continue;
				}
				var dictValue = consumableDict[itemName];
				dictValue += 1;
				consumableDict[itemName] = dictValue;
			}
			foreach (var consumable in consumableDict)
			{
				var consumableString = consumable.Key + " (Quantity: " + consumable.Value + ")";
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					consumableString);
			}
			var goldString = "_Gold: " + player._Gold + " coins.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				goldString);
			var weightString = "_Weight: " + GetInventoryWeight(player) + "/" + player._MaxCarryWeight;
			OutputController.Display.StoreUserOutput(
				Settings.FormatInfoText(),
				Settings.FormatDefaultBackground(),
				weightString);
		}

		public static void LevelUpCheck(Player player)
		{
			if (player._Experience < player._ExperienceToLevel || player._Level == 10) return;
			foreach (var effect in player._Effects.ToList().Where(effect => effect._IsHarmful = true))
			{
				effect._IsEffectExpired = true;
			}
			player._Level++;
			player._Experience -= player._ExperienceToLevel;
			player._ExperienceToLevel = Settings.GetBaseExperienceToLevel() * player._Level;
			var levelUpString = "You have leveled! You are now level " + player._Level + ".";
			OutputController.Display.StoreUserOutput(
				Settings.FormatLevelUpText(),
				Settings.FormatDefaultBackground(),
				levelUpString);
			var statsToAssign = 5;
			while (statsToAssign > 0)
			{
				var levelUpStatString = "Please choose " + statsToAssign +
										" stats to raise. Your choices are: str, dex, int, const.";
				const string levelUpStatInfo =
					"You may raise a stat more than once by putting a number after the stat, IE str 2.";
				const string levelUpStatStr = "Str will increase your max carrying weight and warrior abilities.";
				const string levelUpStatDex = "Dex will increase your dodge chance and archer abilities";
				const string levelUpStatInt =
					"Int will increase your mana and decrease your training cost for spells and abilities.";
				const string levelUpStatConst = "Const will increase your max hit points.";
				DisplayPlayerStats(player);
				OutputController.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatString);
				OutputController.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatInfo);
				OutputController.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatStr);
				OutputController.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatDex);
				OutputController.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatInt);
				OutputController.Display.StoreUserOutput(
					Settings.FormatAnnounceText(),
					Settings.FormatDefaultBackground(),
					levelUpStatConst);
				OutputController.Display.RetrieveUserOutput();
				OutputController.Display.ClearUserOutput();
				var statNumber = 0;
				try
				{
					var input = InputController.GetFormattedInput(Console.ReadLine());
					if (input.Length > 1)
					{
						if (GameController.IsWholeNumber(input[1]) == false) continue;
						statNumber = Convert.ToInt32(input[1]);
					}
					switch (input[0])
					{
						case "str":
							if (statNumber > 0 && statNumber <= statsToAssign)
							{
								player._Strength += statNumber;
								statsToAssign -= statNumber;
							}
							else
							{
								player._Strength++;
								statsToAssign--;
							}
							break;
						case "dex":
							if (statNumber > 0 && statNumber <= statsToAssign)
							{
								player._Dexterity += statNumber;
								statsToAssign -= statNumber;
							}
							else
							{
								player._Dexterity++;
								statsToAssign--;
							}
							break;
						case "int":
							if (statNumber > 0 && statNumber <= statsToAssign)
							{
								player._Intelligence += statNumber;
								statsToAssign -= statNumber;
							}
							else
							{
								player._Intelligence++;
								statsToAssign--;
							}
							break;
						case "const":
							if (statNumber > 0 && statNumber <= statsToAssign)
							{
								player._Constitution += statNumber;
								statsToAssign -= statNumber;
							}
							else
							{
								player._Constitution++;
								statsToAssign--;
							}
							break;
					}
				}
				catch (IndexOutOfRangeException)
				{
					OutputController.Display.StoreUserOutput(
						Settings.FormatAnnounceText(),
						Settings.FormatDefaultBackground(),
						"You did not select an appropriate stat!");
				}
			}
			OutputController.Display.StoreUserOutput(
				Settings.FormatAnnounceText(),
				Settings.FormatDefaultBackground(),
				"All stats have been assigned!");
			player._FireResistance += 5;
			player._FrostResistance += 5;
			player._ArcaneResistance += 5;
			CalculatePlayerStats(player);
			// Leveling sets player back to max stats
			player._HitPoints = player._MaxHitPoints;
			player._RagePoints = player._MaxRagePoints;
			player._ComboPoints = player._MaxComboPoints;
			player._ManaPoints = player._MaxManaPoints;
			// Update level for rainbow gear if any in player inventory
			foreach (var item in player._Inventory)
			{
				if (item is Armor || item is Weapon)
				{
					var rainbowItem = (IRainbowGear)item;
					if (rainbowItem != null && rainbowItem._IsRainbowGear)
					{
						rainbowItem.UpdateRainbowStats(player);
					}
				}
			}
		}
		public static void CalculatePlayerStats(Player player)
		{
			switch (player._PlayerClass)
			{
				case Player.PlayerClassType.Mage:
					player._MaxManaPoints = player._Intelligence * 10;
					if (player._ManaPoints > player._MaxManaPoints) player._ManaPoints = player._MaxManaPoints;
					break;
				case Player.PlayerClassType.Warrior:
					player._MaxRagePoints = player._Strength * 10;
					if (player._RagePoints > player._MaxRagePoints) player._RagePoints = player._MaxRagePoints;
					break;
				case Player.PlayerClassType.Archer:
					player._MaxComboPoints = player._Dexterity * 10;
					if (player._ComboPoints > player._MaxComboPoints) player._ComboPoints = player._MaxComboPoints;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			player._MaxHitPoints = player._Constitution * 10;
			if (player._HitPoints > player._MaxHitPoints) player._HitPoints = player._MaxHitPoints;
			player._MaxCarryWeight = (int)(player._Strength * 2.5);
			player._DodgeChance = player._Dexterity * 1.5;
		}
		public static void DisplayPlayerStats(Player player)
		{
			Settings.FormatGeneralInfoText();
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				Settings.FormatTextBorder());
			var playerHealthString = "Health: " + player._HitPoints + "/" + player._MaxHitPoints + " ";
			var healLineOutput = new List<string>() {
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				playerHealthString};
			var hitPointMaxUnits = player._MaxHitPoints / 10;
			var hitPointUnits = player._HitPoints / hitPointMaxUnits;
			for (var i = 0; i < hitPointUnits; i++)
			{
				healLineOutput.Add(Settings.FormatGeneralInfoText());
				healLineOutput.Add(Settings.FormatHealthBackground());
				healLineOutput.Add("    ");
			}
			OutputController.Display.StoreUserOutput(healLineOutput);
			switch (player._PlayerClass)
			{
				case Player.PlayerClassType.Mage:
					var playerManaString = "Mana: " + player._ManaPoints + "/" + player._MaxManaPoints + " ";
					var manaLineOutput = new List<string>();
					manaLineOutput.Add(Settings.FormatGeneralInfoText());
					manaLineOutput.Add(Settings.FormatDefaultBackground());
					manaLineOutput.Add(playerManaString);
					var manaBufferAmount = playerHealthString.Length - playerManaString.Length;
					var manaBufferStringBuilder = new StringBuilder();
					for (var b = 0; b < manaBufferAmount; b++)
					{
						manaBufferStringBuilder.Append(" ");
					}
					manaLineOutput.Add(Settings.FormatGeneralInfoText());
					manaLineOutput.Add(Settings.FormatDefaultBackground());
					manaLineOutput.Add(manaBufferStringBuilder.ToString());
					var manaPointMaxUnits = player._MaxManaPoints / 10;
					var manaPointUnits = player._ManaPoints / manaPointMaxUnits;
					for (var i = 0; i < manaPointUnits; i++)
					{
						manaLineOutput.Add(Settings.FormatGeneralInfoText());
						manaLineOutput.Add(Settings.FormatManaBackground());
						manaLineOutput.Add("    ");
					}
					OutputController.Display.StoreUserOutput(manaLineOutput);
					break;
				case Player.PlayerClassType.Warrior:
					var playerRageString = "Rage: " + player._RagePoints + "/" + player._MaxRagePoints + " ";
					var rageLineOutput = new List<string>();
					rageLineOutput.Add(Settings.FormatGeneralInfoText());
					rageLineOutput.Add(Settings.FormatDefaultBackground());
					rageLineOutput.Add(playerRageString);
					var rageBufferAmount = playerHealthString.Length - playerRageString.Length;
					var rageBufferStringBuilder = new StringBuilder();
					for (var b = 0; b < rageBufferAmount; b++)
					{
						rageBufferStringBuilder.Append(" ");
					}
					rageLineOutput.Add(Settings.FormatGeneralInfoText());
					rageLineOutput.Add(Settings.FormatDefaultBackground());
					rageLineOutput.Add(rageBufferStringBuilder.ToString());
					var ragePointMaxUnits = player._MaxRagePoints / 10;
					var ragePointUnits = player._RagePoints / ragePointMaxUnits;
					for (var i = 0; i < ragePointUnits; i++)
					{
						rageLineOutput.Add(Settings.FormatGeneralInfoText());
						rageLineOutput.Add(Settings.FormatRageBackground());
						rageLineOutput.Add("    ");
					}
					OutputController.Display.StoreUserOutput(rageLineOutput);
					break;
				case Player.PlayerClassType.Archer:
					var playerComboString = "Combo: " + player._ComboPoints + "/" + player._MaxComboPoints + " ";
					var comboLineOutput = new List<string>();
					comboLineOutput.Add(Settings.FormatGeneralInfoText());
					comboLineOutput.Add(Settings.FormatDefaultBackground());
					comboLineOutput.Add(playerComboString);
					var comboBufferAmount = playerHealthString.Length - playerComboString.Length;
					var comboBufferStringBuilder = new StringBuilder();
					for (var b = 0; b < comboBufferAmount; b++)
					{
						comboBufferStringBuilder.Append(" ");
					}
					comboLineOutput.Add(Settings.FormatGeneralInfoText());
					comboLineOutput.Add(Settings.FormatDefaultBackground());
					comboLineOutput.Add(comboBufferStringBuilder.ToString());
					var comboPointMaxUnits = player._MaxComboPoints / 10;
					var comboPointUnits = player._ComboPoints / comboPointMaxUnits;
					for (var i = 0; i < comboPointUnits; i++)
					{
						comboLineOutput.Add(Settings.FormatGeneralInfoText());
						comboLineOutput.Add(Settings.FormatComboBackground());
						comboLineOutput.Add("    ");
					}
					OutputController.Display.StoreUserOutput(comboLineOutput);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			var expString = "EXP: " + player._Experience;
			var expLineOutput =
				new List<string> { Settings.FormatGeneralInfoText(), Settings.FormatDefaultBackground(), expString };
			var expBufferAmount = playerHealthString.Length - expString.Length;
			var expBufferStringBuilder = new StringBuilder();
			for (var b = 0; b < expBufferAmount; b++)
			{
				expBufferStringBuilder.Append(" ");
			}
			expLineOutput.Add(Settings.FormatGeneralInfoText());
			expLineOutput.Add(Settings.FormatDefaultBackground());
			expLineOutput.Add(expBufferStringBuilder.ToString());
			var expPointMaxUnits = player._ExperienceToLevel / 10;
			var expPointUnits = player._Experience / expPointMaxUnits;
			for (var i = 0; i < expPointUnits; i++)
			{
				expLineOutput.Add(Settings.FormatGeneralInfoText());
				expLineOutput.Add(Settings.FormatExpBackground());
				expLineOutput.Add("    ");
			}
			OutputController.Display.StoreUserOutput(expLineOutput);
			var baseStatsString = "Str: " + player._Strength + " Int: " + player._Intelligence +
							  " Dex: " + player._Dexterity + " _Level: " + player._Level;
			var statsSb = new StringBuilder(baseStatsString);
			if (player._PlayerClass == Player.PlayerClassType.Archer)
				statsSb.Append(" Arrows: " + player._PlayerQuiver?._Quantity);
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				statsSb.ToString());
			var resistString = "Fire Resist: " + player._FireResistance + " Frost Resist: " + player._FrostResistance +
							   " Arcane Resist: " + player._ArcaneResistance;
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				resistString);
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				Settings.FormatTextBorder());
		}
		public static void ListAbilities(Player player)
		{
			if (player._PlayerClass != Player.PlayerClassType.Mage)
			{
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					"You have the following abilities:");
				foreach (var ability in player._Abilities)
				{
					var abilityName = textInfo.ToTitleCase(ability._Name);
					var abilityString = abilityName + ", Rank " + ability._Rank;
					OutputController.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						abilityString);
				}
			}
			else if (player._PlayerClass == Player.PlayerClassType.Mage)
			{
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You're not a warrior or archer!");
			}
			else
			{
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't list that.");
			}
		}
		public static void ListSpells(Player player)
		{
			if (player._PlayerClass == Player.PlayerClassType.Mage)
			{
				var textInfo = new CultureInfo("en-US", false).TextInfo;
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"Your spellbook contains:");
				foreach (var spell in player._Spellbook)
				{
					var spellName = textInfo.ToTitleCase(spell._Name);
					var spellString = spellName + ", Rank " + spell._Rank;
					OutputController.Display.StoreUserOutput(
						Settings.FormatInfoText(),
						Settings.FormatDefaultBackground(),
						spellString);
				}
			}
			else if (player._PlayerClass != Player.PlayerClassType.Mage)
			{
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You're not a mage!");
			}
			else
			{
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't list that.");
			}
		}
		public static void AbilityInfo(Player player, string[] input)
		{
			var inputName = InputController.ParseInput(input);
			var index = player._Abilities.FindIndex(
				f => f._Name == inputName || f._Name.Contains(inputName));
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			if (index != -1 && player._PlayerClass == Player.PlayerClassType.Warrior)
			{
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					textInfo.ToTitleCase(player._Abilities[index]._Name));
				var rankString = "Rank: " + player._Abilities[index]._Rank;
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					rankString);
				var rageCostString = "Rage Cost: " + player._Abilities[index]._RageCost;
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					rageCostString);
				switch (player._Abilities[index]._WarAbilityCategory)
				{
					case PlayerAbility.WarriorAbility.Slash:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case PlayerAbility.WarriorAbility.Rend:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case PlayerAbility.WarriorAbility.Charge:
						PlayerAbility.StunAbilityInfo(player, index);
						break;
					case PlayerAbility.WarriorAbility.Block:
						PlayerAbility.DefenseAbilityInfo(player, index);
						break;
					case PlayerAbility.WarriorAbility.Berserk:
						PlayerAbility.BerserkAbilityInfo(player, index);
						break;
					case PlayerAbility.WarriorAbility.Disarm:
						PlayerAbility.DisarmAbilityInfo(player, index);
						break;
					case PlayerAbility.WarriorAbility.Bandage:
						PlayerAbility.BandageAbilityInfo(player, index);
						break;
					case PlayerAbility.WarriorAbility.PowerAura:
						PlayerAbility.PowerAuraAbilityInfo(player, index);
						break;
					case PlayerAbility.WarriorAbility.WarCry:
						PlayerAbility.WarCryAbilityInfo(player, index);
						break;
					case PlayerAbility.WarriorAbility.Onslaught:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (index != -1 && player._PlayerClass == Player.PlayerClassType.Archer)
			{
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					textInfo.ToTitleCase(player._Abilities[index]._Name));
				var rankString = "Rank: " + player._Abilities[index]._Rank;
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					rankString);
				var comboCostString = "Combo Cost: " + player._Abilities[index]._ComboCost;
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					comboCostString);
				switch (player._Abilities[index]._ArcAbilityCategory)
				{
					case PlayerAbility.ArcherAbility.Distance:
						PlayerAbility.DistanceAbilityInfo(player, index);
						break;
					case PlayerAbility.ArcherAbility.Gut:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case PlayerAbility.ArcherAbility.Precise:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case PlayerAbility.ArcherAbility.Stun:
						PlayerAbility.StunAbilityInfo(player, index);
						break;
					case PlayerAbility.ArcherAbility.Double:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case PlayerAbility.ArcherAbility.Wound:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case PlayerAbility.ArcherAbility.Bandage:
						PlayerAbility.BandageAbilityInfo(player, index);
						break;
					case PlayerAbility.ArcherAbility.SwiftAura:
						PlayerAbility.SwiftAuraAbilityInfo(player, index);
						break;
					case PlayerAbility.ArcherAbility.ImmolatingArrow:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					case PlayerAbility.ArcherAbility.Ambush:
						PlayerAbility.OffenseDamageAbilityInfo(player, index);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (index != -1 && player._PlayerClass == Player.PlayerClassType.Mage)
			{
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You're not a warrior or archer!");
			}
			else
			{
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You don't have that ability.");
			}
		}
		public static void SpellInfo(Player player, string[] input)
		{
			var inputName = new StringBuilder();
			for (var i = 1; i < input.Length; i++)
			{
				inputName.Append(input[i]);
				if (i != input.Length - 1) inputName.Append(" ");
			}
			var index = player._Spellbook.FindIndex(f =>
				f._Name == inputName.ToString() || f._Name == input[1] || f._Name.Contains(inputName.ToString()));
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			if (index != -1 && player._PlayerClass == Player.PlayerClassType.Mage)
			{
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					textInfo.ToTitleCase(player._Spellbook[index]._Name));
				var rankString = "Rank: " + player._Spellbook[index]._Rank;
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					rankString);
				var manaCostString = "Mana Cost: " + player._Spellbook[index]._ManaCost;
				OutputController.Display.StoreUserOutput(
					Settings.FormatInfoText(),
					Settings.FormatDefaultBackground(),
					manaCostString);
				switch (player._Spellbook[index]._SpellCategory)
				{
					case PlayerSpell.SpellType.Fireball:
						PlayerSpell.FireOffenseSpellInfo(player, index);
						break;
					case PlayerSpell.SpellType.Frostbolt:
						PlayerSpell.FrostOffenseSpellInfo(player, index);
						break;
					case PlayerSpell.SpellType.Lightning:
						PlayerSpell.ArcaneOffenseSpellInfo(player, index);
						break;
					case PlayerSpell.SpellType.Heal:
						PlayerSpell.HealingSpellInfo(player, index);
						break;
					case PlayerSpell.SpellType.Rejuvenate:
						PlayerSpell.HealingSpellInfo(player, index);
						break;
					case PlayerSpell.SpellType.Diamondskin:
						PlayerSpell.AugmentArmorSpellInfo(player, index);
						break;
					case PlayerSpell.SpellType.TownPortal:
						PlayerSpell.PortalSpellInfo();
						break;
					case PlayerSpell.SpellType.Reflect:
						PlayerSpell.ReflectDamageSpellInfo(player, index);
						break;
					case PlayerSpell.SpellType.ArcaneIntellect:
						PlayerSpell.ArcaneIntellectSpellInfo(player, index);
						break;
					case PlayerSpell.SpellType.FrostNova:
						PlayerSpell.FrostOffenseSpellInfo(player, index);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (index != -1 && player._PlayerClass != Player.PlayerClassType.Mage)
			{
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You're not a mage!");
			}
			else
			{
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You don't have that spell.");
			}
		}
		public static void QuestInfo(Player player, string[] input)
		{
			var inputName = new StringBuilder();
			for (var i = 1; i < input.Length; i++)
			{
				inputName.Append(input[i]);
				if (i != input.Length - 1) inputName.Append(" ");
			}
			var index = player._QuestLog.FindIndex(f =>
				f._Name.ToLowerInvariant() == inputName.ToString() || f._Name.ToLowerInvariant() == input[1] ||
				f._Name.ToLowerInvariant().Contains(inputName.ToString()));
			if (index != -1)
			{
				player._QuestLog[index].ShowQuest();
			}
			else
			{
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You don't have that quest.");
			}
		}
		public static int CalculateAbilityDamage(Player player, Monster opponent, int index)
		{
			if (player._Abilities[index]._DamageGroup == PlayerAbility.DamageType.Physical)
			{
				return player._Abilities[index]._Offensive._Amount;
			}
			var damageReductionPercentage = player._Abilities[index]._DamageGroup switch
			{
				PlayerAbility.DamageType.Fire => (opponent._FireResistance / 100.0),
				PlayerAbility.DamageType.Frost => (opponent._FrostResistance / 100.0),
				PlayerAbility.DamageType.Arcane => (opponent._ArcaneResistance / 100.0),
				_ => 0.0
			};
			return (int)(player._Abilities[index]._Offensive._Amount * (1 - damageReductionPercentage));
		}
		public static int CalculateSpellDamage(Player player, Monster opponent, int index)
		{
			if (player._Spellbook[index]._DamageGroup == PlayerSpell.DamageType.Physical)
			{
				return player._Spellbook[index]._Offensive._Amount;
			}
			var damageReductionPercentage = player._Spellbook[index]._DamageGroup switch
			{
				PlayerSpell.DamageType.Fire => (opponent._FireResistance / 100.0),
				PlayerSpell.DamageType.Frost => (opponent._FrostResistance / 100.0),
				PlayerSpell.DamageType.Arcane => (opponent._ArcaneResistance / 100.0),
				_ => 0.0
			};
			return (int)(player._Spellbook[index]._Offensive._Amount * (1 - damageReductionPercentage));
		}
	}
}