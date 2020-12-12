using DungeonGame.Items;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DungeonGame
{
	public static class GearHandler
	{
		private static bool IsWearable(IEquipment item)
		{
			return item.GetType().Name == "Armor" || item.GetType().Name == "Weapon" || item.GetType().Name == "Quiver";
		}
		public static void EquipInitialGear(Player player)
		{
			EquipWeapon(player, player._Inventory[0] as Weapon);
			EquipArmor(player, player._Inventory[1] as Armor);
			EquipArmor(player, player._Inventory[2] as Armor);
			EquipArmor(player, player._Inventory[3] as Armor);
			if (player._PlayerClass == Player.PlayerClassType.Archer)
			{
				EquipQuiver(player, player._Inventory[4] as Quiver);
			}
		}
		public static string GetItemDetails(IEquipment item)
		{
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			var itemInfo = new StringBuilder();
			itemInfo.Append(item._Name);
			switch (item)
			{
				case Armor isItemArmor:
					itemInfo.Append(" (AR: " + isItemArmor._ArmorRating + ")");
					break;
				case Weapon isItemWeapon:
					itemInfo.Append(" (DMG: " + isItemWeapon._RegDamage + " CR: " + isItemWeapon._CritMultiplier + ")");
					break;
			}
			var itemName = textInfo.ToTitleCase(itemInfo.ToString());
			return itemName;
		}
		public static void StoreRainbowGearOutput(string itemName)
		{
			var sameLineOutput = new List<string>();
			for (var i = 0; i < itemName.Length; i++)
			{
				var colorIndex = i % 7;
				var colorString = colorIndex switch
				{
					0 => "cyan",
					1 => "green",
					2 => "yellow",
					3 => "white",
					4 => "red",
					5 => "blue",
					6 => "magenta",
					_ => throw new ArgumentOutOfRangeException()
				};
				sameLineOutput.Add(colorString); // Foreground color
				sameLineOutput.Add(Settings.FormatDefaultBackground()); // Background color
				sameLineOutput.Add(itemName[i].ToString()); // What prints to display
			}
			OutputHandler.Display.StoreUserOutput(sameLineOutput);
		}
		public static void DecreaseArmorDurability(Player player)
		{
			player._PlayerChestArmor?.DecreaseDurability();
			player._PlayerHeadArmor?.DecreaseDurability();
			player._PlayerLegsArmor?.DecreaseDurability();
		}
		public static int CheckArmorRating(Player player)
		{
			var totalArmorRating = 0;
			if (player._PlayerChestArmor != null && player._PlayerChestArmor._Equipped)
			{
				totalArmorRating += (int)player._PlayerChestArmor.GetArmorRating();
			}
			if (player._PlayerHeadArmor != null && player._PlayerHeadArmor._Equipped)
			{
				totalArmorRating += (int)player._PlayerHeadArmor.GetArmorRating();
			}
			if (player._PlayerLegsArmor != null && player._PlayerLegsArmor._Equipped)
			{
				totalArmorRating += (int)player._PlayerLegsArmor.GetArmorRating();
			}
			if (!player._InCombat) return totalArmorRating;
			totalArmorRating += player._Effects.Where(
				effect => effect._EffectGroup == Effect.EffectType.ChangeArmor).Sum(effect => effect._EffectAmountOverTime);
			return totalArmorRating < 0 ? 0 : totalArmorRating;
		}
		public static void UseWeaponKit(Player player, string[] userInput)
		{
			var kitIndex = player._Consumables.FindIndex(f => f._Name.Contains(userInput[2]));
			if (kitIndex == -1)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What weapon kit did you want to use?");
				return;
			}
			var weaponIndex = player._Inventory.FindIndex(f =>
				f._Name.Contains(userInput[1].ToLowerInvariant()));
			if (weaponIndex == -1)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What weapon did you want to upgrade?");
				return;
			}
			var weapon = player._Inventory[weaponIndex] as Weapon;
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			var weaponName = textInfo.ToTitleCase(weapon._Name);
			if (player._Consumables[kitIndex]._KitCategory != Consumable.KitType.Weapon)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't upgrade " + weaponName + " with that!");
				return;
			}
			if (player._Consumables[kitIndex]._ChangeWeapon._KitCategory == ChangeWeapon.KitType.Bowstring &&
				weapon._WeaponGroup != Weapon.WeaponType.Bow ||
				player._Consumables[kitIndex]._ChangeWeapon._KitCategory == ChangeWeapon.KitType.Grindstone &&
				weapon._WeaponGroup == Weapon.WeaponType.Bow)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't upgrade " + weaponName + " with that!");
				return;
			}
			if (!weapon._Equipped)
			{
				var inputValid = false;
				while (!inputValid)
				{
					var weaponString = weaponName + " is not equipped. Are you sure you want to upgrade that?";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						weaponString);
					OutputHandler.Display.RetrieveUserOutput();
					OutputHandler.Display.ClearUserOutput();
					var input = InputHandler.GetFormattedInput(Console.ReadLine());
					if (input[0] == "no" || input[0] == "n") return;
					if (input[0] == "yes" || input[0] == "y") inputValid = true;
				}
			}
			player._Consumables[kitIndex]._ChangeWeapon.ChangeWeaponPlayer(weapon);
			weapon._ItemValue += player._Consumables[kitIndex]._ItemValue;
			var upgradeSuccess = "You upgraded " + weaponName + " with a weapon kit.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				upgradeSuccess);
			player._Consumables.RemoveAt(kitIndex);
		}
		public static void UseArmorKit(Player player, string[] userInput)
		{
			var kitIndex = player._Consumables.FindIndex(f => f._Name.Contains(userInput[2]));
			if (kitIndex == -1)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What armor kit did you want to use?");
				return;
			}
			var armorIndex = player._Inventory.FindIndex(f =>
				f._Name.Contains(userInput[1].ToLowerInvariant()));
			if (armorIndex == -1)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What armor did you want to upgrade?");
				return;
			}
			var armor = player._Inventory[armorIndex] as Armor;
			var textInfo = new CultureInfo("en-US", false).TextInfo;
			var armorName = textInfo.ToTitleCase(armor._Name);
			if (player._Consumables[kitIndex]._KitCategory != Consumable.KitType.Armor)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't upgrade " + armorName + " with that!");
				return;
			}
			if (player._Consumables[kitIndex]._ChangeArmor._KitCategory == ChangeArmor.KitType.Cloth &&
				armor._ArmorGroup != Armor.ArmorType.Cloth ||
				player._Consumables[kitIndex]._ChangeArmor._KitCategory == ChangeArmor.KitType.Leather &&
				armor._ArmorGroup != Armor.ArmorType.Leather ||
				player._Consumables[kitIndex]._ChangeArmor._KitCategory == ChangeArmor.KitType.Plate &&
				 armor._ArmorGroup != Armor.ArmorType.Plate ||
				player._Consumables[kitIndex]._KitCategory == Consumable.KitType.Weapon)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't upgrade " + armorName + " with that!");
				return;
			}
			if (!armor._Equipped)
			{
				var inputValid = false;
				while (!inputValid)
				{
					var armorString = armorName + " is not equipped. Are you sure you want to upgrade that?";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						armorString);
					OutputHandler.Display.BuildUserOutput();
					OutputHandler.Display.ClearUserOutput();
					var input = InputHandler.GetFormattedInput(Console.ReadLine());
					if (input[0] == "no" || input[0] == "n") return;
					if (input[0] == "yes" || input[0] == "y") inputValid = true;
				}
			}
			player._Consumables[kitIndex]._ChangeArmor.ChangeArmorPlayer(armor);
			armor._ItemValue += player._Consumables[kitIndex]._ItemValue;
			var upgradeSuccess = "You upgraded " + armorName + " with an armor kit.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				upgradeSuccess);
			player._Consumables.RemoveAt(kitIndex);
		}
		public static void DropItem(Player player, string[] input)
		{
			if (input[1] == null)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What item did you want to drop?");
				return;
			}
			var itemIndex = player._Inventory.FindIndex(
				f => f._Name == input[1] || f._Name.Contains(input[1]));
			var playerRoom = RoomHandler.Rooms[player._PlayerLocation];
			if (itemIndex != -1)
			{
				if (player._Inventory[itemIndex]._Equipped)
				{
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You have to unequip that item first!");
					return;
				}
				playerRoom._RoomObjects.Add(player._Inventory[itemIndex]);
				var dropInventoryString = "You dropped " +
										player._Inventory[itemIndex]._Name + ".";
				player._Inventory.RemoveAt(itemIndex);
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					dropInventoryString);
				return;
			}
			itemIndex = player._Consumables.FindIndex(
				f => f._Name == input[1] || f._Name.Contains(input[1]));
			if (itemIndex != -1)
			{
				playerRoom._RoomObjects.Add(player._Consumables[itemIndex]);
				var dropConsumableString = "You dropped " +
										  player._Consumables[itemIndex]._Name + ".";
				player._Consumables.RemoveAt(itemIndex);
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					dropConsumableString);
				return;
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"You don't have that item in your inventory!");
		}
		public static void PickupItem(Player player, string[] input)
		{
			if (input[1] == null)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What item did you want to pickup?");
				return;
			}
			var playerRoom = RoomHandler.Rooms[player._PlayerLocation];
			var itemIndex = playerRoom._RoomObjects.FindIndex(
				f => f._Name == input[1] || f._Name.Contains(input[1]));
			if (!(playerRoom._RoomObjects[itemIndex] is IEquipment item))
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't pick that up!");
				return;
			}
			var playerWeight = PlayerHandler.GetInventoryWeight(player);
			var itemWeight = item._Weight;
			if (playerWeight + itemWeight > player._MaxCarryWeight)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't carry that much!");
			}
			if (itemIndex == -1)
			{
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"That item is not in the room!");
				return;
			}
			if (playerRoom._RoomObjects[itemIndex].GetType() == typeof(Consumable))
			{
				player._Consumables.Add(playerRoom._RoomObjects[itemIndex] as Consumable);
			}
			else
			{
				player._Inventory.Add(playerRoom._RoomObjects[itemIndex] as IEquipment);
			}
			var pickupItemString = "You picked up " +
								   playerRoom._RoomObjects[itemIndex]._Name + ".";
			playerRoom._RoomObjects.RemoveAt(itemIndex);
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				pickupItemString);
		}
		public static void EquipItem(Player player, string[] input)
		{
			var inputString = new StringBuilder();
			for (var i = 1; i < input.Length; i++)
			{
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			var inputName = inputString.ToString().Trim();
			var item = player._Inventory.Find(f => f._Name.Contains(inputName));
			if (item == null)
			{
				var noItemFoundString = "You don't have " + inputName + " in your inventory!";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					noItemFoundString);
				return;
			}
			var itemType = item.GetType().Name;
			if (input[0] == "equip")
			{
				if (IsWearable(item) && item._Equipped == false)
				{
					switch (itemType)
					{
						case "Weapon":
							EquipWeapon(player, (Weapon)item);
							break;
						case "Armor":
							EquipArmor(player, (Armor)item);
							break;
						case "Quiver":
							EquipQuiver(player, (Quiver)item);
							break;
					}
					return;
				}
				if (IsWearable(item))
				{
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You have already equipped that.");
					return;
				}
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't equip that!");
				return;
			}
			if (input[0] == "unequip")
			{
				if (!IsWearable(item)) return;
				switch (itemType)
				{
					case "Weapon":
						UnequipWeapon(player, (Weapon)item);
						break;
					case "Armor":
						UnequipArmor(player, (Armor)item);
						break;
					case "Quiver":
						UnequipQuiver(player, (Quiver)item);
						break;
				}
			}
		}
		private static void UnequipWeapon(Player player, Weapon weapon)
		{
			if (!weapon._Equipped)
			{
				var alreadyUnequipString = "You have already unequipped " + weapon._Name + ".";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					alreadyUnequipString);
				return;
			}
			weapon._Equipped = false;
			var unequipString = "You have unequipped " + player._PlayerWeapon._Name + ".";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				unequipString);
			player._PlayerWeapon = null;
		}
		private static void UnequipQuiver(Player player, Quiver quiver)
		{
			if (!quiver._Equipped)
			{
				var alreadyUnequipString = "You have already unequipped " + quiver._Name + "{0}.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					alreadyUnequipString);
				return;
			}
			quiver._Equipped = false;
			var unequipString = "You have unequipped " + player._PlayerQuiver._Name + ".";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				unequipString);
			player._PlayerQuiver = null;
		}
		private static void UnequipArmor(Player player, Armor armor)
		{
			if (!armor._Equipped)
			{
				var alreadyUnequipString = "You have already unequipped " + armor._Name + ".";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					alreadyUnequipString);
				return;
			}
			armor._Equipped = false;
			switch (armor._ArmorCategory)
			{
				case Armor.ArmorSlot.Head:
					var unequipHeadString = "You have unequipped " + player._PlayerHeadArmor._Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipHeadString);
					player._PlayerHeadArmor = null;
					break;
				case Armor.ArmorSlot.Back:
					var unequipBackString = "You have unequipped " + player._PlayerBackArmor._Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipBackString);
					player._PlayerBackArmor = null;
					break;
				case Armor.ArmorSlot.Chest:
					var unequipChestString = "You have unequipped " + player._PlayerChestArmor._Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipChestString);
					player._PlayerChestArmor = null;
					break;
				case Armor.ArmorSlot.Wrist:
					var unequipWristString = "You have unequipped " + player._PlayerWristArmor._Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipWristString);
					player._PlayerWristArmor = null;
					break;
				case Armor.ArmorSlot.Waist:
					var unequipWaistString = "You have unequipped " + player._PlayerWaistArmor._Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipWaistString);
					player._PlayerWaistArmor = null;
					break;
				case Armor.ArmorSlot.Legs:
					var unequipLegsString = "You have unequipped " + player._PlayerLegsArmor._Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipLegsString);
					player._PlayerLegsArmor = null;
					break;
				case Armor.ArmorSlot.Hands:
					var unequipHandsString = "You have unequipped " + player._PlayerHandsArmor._Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipHandsString);
					player._PlayerHandsArmor = null;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private static void EquipWeapon(Player player, Weapon weapon)
		{
			if (weapon._Equipped)
			{
				var alreadyEquipString = "You have already equipped " + weapon._Name + ".";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					alreadyEquipString);
				return;
			}
			// Check to see if player can use weapon, if so, continue, otherwise return output error msg
			switch (weapon._WeaponGroup)
			{
				case Weapon.WeaponType.Dagger when player._CanUseDagger:
					break;
				case Weapon.WeaponType.OneHandedSword when player._CanUseOneHandedSword:
					break;
				case Weapon.WeaponType.TwoHandedSword when player._CanUseTwoHandedSword:
					break;
				case Weapon.WeaponType.Axe when player._CanUseAxe:
					break;
				case Weapon.WeaponType.Bow when player._CanUseBow:
					break;
				default:
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You cannot use that type of weapon!");
					return;
			}
			if (player._PlayerWeapon != null && player._PlayerWeapon._Equipped)
			{
				UnequipWeapon(player, player._PlayerWeapon);
			}
			player._PlayerWeapon = weapon;
			weapon._Equipped = true;
			var equipSuccessString = "You have equipped " + player._PlayerWeapon._Name + ".";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				equipSuccessString);
		}
		private static void EquipQuiver(Player player, Quiver quiver)
		{
			if (quiver._Equipped)
			{
				var alreadyEquipString = "You have already equipped " + quiver._Name + ".";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					alreadyEquipString);
				return;
			}
			if (player._PlayerQuiver != null && player._PlayerQuiver._Equipped)
			{
				UnequipQuiver(player, player._PlayerQuiver);
			}
			player._PlayerQuiver = quiver;
			quiver._Equipped = true;
			var equipString = "You have equipped " + player._PlayerQuiver._Name + ".";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				equipString);
		}
		private static void EquipArmor(Player player, Armor armor)
		{
			if (armor._Equipped)
			{
				var alreadyEquipString = "You have already equipped " + armor._Name + ".";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					alreadyEquipString);
				return;
			}
			// Check to see if player can use armor type, if so, continue, otherwise return output error msg
			switch (armor._ArmorGroup)
			{
				case Armor.ArmorType.Cloth when player._CanWearCloth:
					break;
				case Armor.ArmorType.Leather when player._CanWearLeather:
					break;
				case Armor.ArmorType.Plate when player._CanWearPlate:
					break;
				default:
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You cannot wear that type of armor!");
					return;
			}
			switch (armor._ArmorCategory)
			{
				case Armor.ArmorSlot.Head:
					if (player._PlayerHeadArmor != null && player._PlayerHeadArmor._Equipped)
					{
						UnequipArmor(player, player._PlayerHeadArmor);
					}
					player._PlayerHeadArmor = armor;
					armor._Equipped = true;
					var equipHeadString = "You have equipped " + player._PlayerHeadArmor._Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						equipHeadString);
					break;
				case Armor.ArmorSlot.Back:
					if (player._PlayerBackArmor != null && player._PlayerBackArmor._Equipped)
					{
						UnequipArmor(player, player._PlayerBackArmor);
					}
					player._PlayerBackArmor = armor;
					armor._Equipped = true;
					var equipBackString = "You have equipped " + player._PlayerBackArmor._Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						equipBackString);
					break;
				case Armor.ArmorSlot.Chest:
					if (player._PlayerChestArmor != null && player._PlayerChestArmor._Equipped)
					{
						UnequipArmor(player, player._PlayerChestArmor);
					}
					player._PlayerChestArmor = armor;
					armor._Equipped = true;
					var equipChestString = "You have equipped " + player._PlayerChestArmor._Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						equipChestString);
					break;
				case Armor.ArmorSlot.Wrist:
					if (player._PlayerWristArmor != null && player._PlayerWristArmor._Equipped)
					{
						UnequipArmor(player, player._PlayerWristArmor);
					}
					player._PlayerWristArmor = armor;
					armor._Equipped = true;
					var equipWristString = "You have equipped " + player._PlayerWristArmor._Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						equipWristString);
					break;
				case Armor.ArmorSlot.Waist:
					if (player._PlayerWaistArmor != null && player._PlayerWaistArmor._Equipped)
					{
						UnequipArmor(player, player._PlayerWaistArmor);
					}
					player._PlayerWaistArmor = armor;
					armor._Equipped = true;
					var equipWaistString = "You have equipped " + player._PlayerWaistArmor._Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						equipWaistString);
					break;
				case Armor.ArmorSlot.Legs:
					if (player._PlayerLegsArmor != null && player._PlayerLegsArmor._Equipped)
					{
						UnequipArmor(player, player._PlayerLegsArmor);
					}
					player._PlayerLegsArmor = armor;
					armor._Equipped = true;
					var equipLegsString = "You have equipped " + player._PlayerLegsArmor._Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						equipLegsString);
					break;
				case Armor.ArmorSlot.Hands:
					if (player._PlayerHandsArmor != null && player._PlayerHandsArmor._Equipped)
					{
						UnequipArmor(player, player._PlayerHandsArmor);
					}
					player._PlayerHandsArmor = armor;
					armor._Equipped = true;
					var equipHandsString = "You have equipped " + player._PlayerHandsArmor._Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						equipHandsString);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}