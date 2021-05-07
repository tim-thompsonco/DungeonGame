using DungeonGame.Effects;
using DungeonGame.Items;
using DungeonGame.Items.Consumables.Kits;
using DungeonGame.Items.Equipment;
using DungeonGame.Players;
using DungeonGame.Rooms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DungeonGame.Controllers {
	public static class GearController {
		public static void EquipInitialGear(Player player) {
			Weapon playerWeapon = player._Inventory.Find(item => item is Weapon) as Weapon;
			EquipWeapon(player, playerWeapon);

			List<IItem> playerArmorList = player._Inventory.FindAll(item => item is Armor);
			foreach (IItem playerArmor in playerArmorList) {
				EquipArmor(player, (Armor)playerArmor);
			}

			if (player._PlayerClass == Player.PlayerClassType.Archer) {
				Quiver quiver = player._Inventory.Find(item => item is Quiver) as Quiver;
				EquipQuiver(player, quiver);
			}
		}
		public static string GetItemDetails(IItem item) {
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			StringBuilder itemInfo = new StringBuilder();
			itemInfo.Append(item.Name);
			switch (item) {
				case Armor isItemArmor:
					itemInfo.Append($" (AR: {isItemArmor.ArmorRating})");
					break;
				case Weapon isItemWeapon:
					itemInfo.Append($" (DMG: {isItemWeapon._RegDamage} CR: {isItemWeapon._CritMultiplier})");
					break;
			}
			string itemName = textInfo.ToTitleCase(itemInfo.ToString());
			return itemName;
		}
		public static void StoreRainbowGearOutput(string itemName) {
			List<string> sameLineOutput = new List<string>();
			for (int i = 0; i < itemName.Length; i++) {
				int colorIndex = i % 7;
				string colorString = colorIndex switch {
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
			OutputController.Display.StoreUserOutput(sameLineOutput);
		}
		public static void DecreaseArmorDurability(Player player) {
			player._PlayerChestArmor?.DecreaseDurability();
			player._PlayerHeadArmor?.DecreaseDurability();
			player._PlayerLegsArmor?.DecreaseDurability();
		}

		public static int CheckArmorRating(Player player) {
			int totalArmorRating = 0;

			if (player._PlayerChestArmor != null && player._PlayerChestArmor.Equipped) {
				totalArmorRating += (int)player._PlayerChestArmor.GetArmorRating();
			}

			if (player._PlayerHeadArmor != null && player._PlayerHeadArmor.Equipped) {
				totalArmorRating += (int)player._PlayerHeadArmor.GetArmorRating();
			}

			if (player._PlayerLegsArmor != null && player._PlayerLegsArmor.Equipped) {
				totalArmorRating += (int)player._PlayerLegsArmor.GetArmorRating();
			}

			if (!player._InCombat) {
				return totalArmorRating;
			}

			foreach (IEffect effect in player._Effects.Where(effect => effect is ChangeArmorEffect)) {
				ChangeArmorEffect changeArmorEffect = effect as ChangeArmorEffect;
				totalArmorRating += changeArmorEffect.ChangeArmorAmount;
			}

			return totalArmorRating < 0 ? 0 : totalArmorRating;
		}

		public static void UseWeaponKit(Player player, string[] userInput) {
			int kitIndex = player._Inventory.FindIndex(f => f.Name.Contains(userInput[2]));
			if (kitIndex == -1) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What weapon kit did you want to use?");
				return;
			}
			int weaponIndex = player._Inventory.FindIndex(f =>
				f.Name.Contains(userInput[1].ToLower()));
			if (weaponIndex == -1) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What weapon did you want to upgrade?");
				return;
			}
			Weapon weapon = player._Inventory[weaponIndex] as Weapon;
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			string weaponName = textInfo.ToTitleCase(weapon.Name);
			if (!weapon.Equipped) {
				bool inputValid = false;
				while (!inputValid) {
					string weaponString = $"{weaponName} is not equipped. Are you sure you want to upgrade that?";
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						weaponString);
					OutputController.Display.RetrieveUserOutput();
					OutputController.Display.ClearUserOutput();
					string[] input = InputController.GetFormattedInput(Console.ReadLine());
					if (input[0] == "no" || input[0] == "n") {
						return;
					}

					if (input[0] == "yes" || input[0] == "y") {
						inputValid = true;
					}
				}
			}

			WeaponKit weaponKit = player._Inventory[kitIndex] as WeaponKit;
			weaponKit.AttemptAugmentPlayerWeapon(weapon);

			if (weaponKit.KitHasBeenUsed) {
				player._Inventory.RemoveAt(kitIndex);
			}
		}

		public static void UseArmorKit(Player player, string[] userInput) {
			int kitIndex = player._Inventory.FindIndex(f => f.Name.Contains(userInput[2]));
			if (kitIndex == -1) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What armor kit did you want to use?");
				return;
			}
			int armorIndex = player._Inventory.FindIndex(f =>
				f.Name.Contains(userInput[1].ToLower()));
			if (armorIndex == -1) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What armor did you want to upgrade?");
				return;
			}
			Armor armor = player._Inventory[armorIndex] as Armor;
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			string armorName = textInfo.ToTitleCase(armor.Name);
			if (!armor.Equipped) {
				bool inputValid = false;
				while (!inputValid) {
					string armorString = $"{armorName} is not equipped. Are you sure you want to upgrade that?";
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						armorString);
					OutputController.Display.BuildUserOutput();
					OutputController.Display.ClearUserOutput();
					string[] input = InputController.GetFormattedInput(Console.ReadLine());
					if (input[0] == "no" || input[0] == "n") {
						return;
					}

					if (input[0] == "yes" || input[0] == "y") {
						inputValid = true;
					}
				}
			}

			ArmorKit armorKit = player._Inventory[kitIndex] as ArmorKit;
			armorKit.AttemptAugmentArmorPlayer(armor);

			if (armorKit.KitHasBeenUsed) {
				player._Inventory.RemoveAt(kitIndex);
			}
		}
		public static void DropItem(Player player, string[] input) {
			if (input[1] == null) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What item did you want to drop?");
				return;
			}
			int itemIndex = player._Inventory.FindIndex(
				f => f.Name == input[1] || f.Name.Contains(input[1]));
			IRoom playerRoom = RoomController._Rooms[player._PlayerLocation];
			if (itemIndex != -1) {
				IEquipment equippedItem = player._Inventory[itemIndex] as IEquipment;
				if (equippedItem.Equipped) {
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You have to unequip that item first!");
					return;
				}
				playerRoom._RoomObjects.Add(player._Inventory[itemIndex]);
				string dropInventoryString = $"You dropped {player._Inventory[itemIndex].Name}.";
				player._Inventory.RemoveAt(itemIndex);
				OutputController.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					dropInventoryString);
				return;
			}
			itemIndex = player._Inventory.FindIndex(
				f => f.Name == input[1] || f.Name.Contains(input[1]));
			if (itemIndex != -1) {
				playerRoom._RoomObjects.Add(player._Inventory[itemIndex]);
				string dropInventorytring = $"You dropped {player._Inventory[itemIndex].Name}.";
				player._Inventory.RemoveAt(itemIndex);
				OutputController.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					dropInventorytring);
				return;
			}
			OutputController.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				"You don't have that item in your inventory!");
		}
		public static void PickupItem(Player player, string[] input) {
			if (input[1] == null) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What item did you want to pickup?");
				return;
			}
			IRoom playerRoom = RoomController._Rooms[player._PlayerLocation];
			int itemIndex = playerRoom._RoomObjects.FindIndex(
				f => f.Name == input[1] || f.Name.Contains(input[1]));
			if (!(playerRoom._RoomObjects[itemIndex] is IItem item)) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't pick that up!");
				return;
			}
			int playerWeight = PlayerController.GetInventoryWeight(player);
			int itemWeight = item.Weight;
			if (playerWeight + itemWeight > player._MaxCarryWeight) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't carry that much!");
			}
			if (itemIndex == -1) {
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"That item is not in the room!");
				return;
			}
			player._Inventory.Add(playerRoom._RoomObjects[itemIndex] as IItem);
			string pickupItemString = $"You picked up {playerRoom._RoomObjects[itemIndex].Name}.";
			playerRoom._RoomObjects.RemoveAt(itemIndex);
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				pickupItemString);
		}
		public static void EquipItem(Player player, string[] input) {
			StringBuilder inputString = new StringBuilder();
			for (int i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			string inputName = inputString.ToString().Trim();
			IItem item = player._Inventory.Find(f => f.Name.Contains(inputName));
			if (item == null) {
				string noItemFoundString = $"You don't have {inputName} in your inventory!";
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					noItemFoundString);
				return;
			}
			string itemType = item.GetType().Name;
			if (input[0] == "equip") {
				if (item is IEquipment itemToEquip && itemToEquip.Equipped == false) {
					switch (itemType) {
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
				if (item is IEquipment) {
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You have already equipped that.");
					return;
				}
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't equip that!");
				return;
			}
			if (input[0] == "unequip") {
				if (!(item is IEquipment)) {
					return;
				}

				switch (itemType) {
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
		private static void UnequipWeapon(Player player, Weapon weapon) {
			if (!weapon.Equipped) {
				string alreadyUnequipString = $"You have already unequipped {weapon.Name}.";
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					alreadyUnequipString);
				return;
			}
			weapon.Equipped = false;
			string unequipString = $"You have unequipped {player._PlayerWeapon.Name}.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				unequipString);
			player._PlayerWeapon = null;
		}
		private static void UnequipQuiver(Player player, Quiver quiver) {
			if (!quiver._Equipped) {
				string alreadyUnequipString = $"You have already unequipped {quiver.Name}.";
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					alreadyUnequipString);
				return;
			}
			quiver._Equipped = false;
			string unequipString = $"You have unequipped {player._PlayerQuiver.Name}.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				unequipString);
			player._PlayerQuiver = null;
		}
		private static void UnequipArmor(Player player, Armor armor) {
			if (!armor.Equipped) {
				string alreadyUnequipString = $"You have already unequipped {armor.Name}.";
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					alreadyUnequipString);
				return;
			}
			armor.Equipped = false;
			switch (armor.ArmorCategory) {
				case Armor.ArmorSlot.Head:
					string unequipHeadString = $"You have unequipped {player._PlayerHeadArmor.Name}.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipHeadString);
					player._PlayerHeadArmor = null;
					break;
				case Armor.ArmorSlot.Back:
					string unequipBackString = $"You have unequipped {player._PlayerBackArmor.Name}.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipBackString);
					player._PlayerBackArmor = null;
					break;
				case Armor.ArmorSlot.Chest:
					string unequipChestString = $"You have unequipped {player._PlayerChestArmor.Name}.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipChestString);
					player._PlayerChestArmor = null;
					break;
				case Armor.ArmorSlot.Wrist:
					string unequipWristString = $"You have unequipped {player._PlayerWristArmor.Name}.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipWristString);
					player._PlayerWristArmor = null;
					break;
				case Armor.ArmorSlot.Waist:
					string unequipWaistString = $"You have unequipped {player._PlayerWaistArmor.Name}.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipWaistString);
					player._PlayerWaistArmor = null;
					break;
				case Armor.ArmorSlot.Legs:
					string unequipLegsString = $"You have unequipped {player._PlayerLegsArmor.Name}.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipLegsString);
					player._PlayerLegsArmor = null;
					break;
				case Armor.ArmorSlot.Hands:
					string unequipHandsString = $"You have unequipped {player._PlayerHandsArmor.Name}.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipHandsString);
					player._PlayerHandsArmor = null;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private static void EquipWeapon(Player player, Weapon weapon) {
			if (weapon.Equipped) {
				string alreadyEquipString = $"You have already equipped {weapon.Name}.";
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					alreadyEquipString);
				return;
			}
			// Check to see if player can use weapon, if so, continue, otherwise return output error msg
			switch (weapon._WeaponGroup) {
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
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You cannot use that type of weapon!");
					return;
			}
			if (player._PlayerWeapon != null && player._PlayerWeapon.Equipped) {
				UnequipWeapon(player, player._PlayerWeapon);
			}
			player._PlayerWeapon = weapon;
			weapon.Equipped = true;
			string equipSuccessString = $"You have equipped {player._PlayerWeapon.Name}.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				equipSuccessString);
		}
		private static void EquipQuiver(Player player, Quiver quiver) {
			if (quiver._Equipped) {
				string alreadyEquipString = $"You have already equipped {quiver.Name}.";
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					alreadyEquipString);
				return;
			}
			if (player._PlayerQuiver != null && player._PlayerQuiver._Equipped) {
				UnequipQuiver(player, player._PlayerQuiver);
			}
			player._PlayerQuiver = quiver;
			quiver._Equipped = true;
			string equipString = $"You have equipped {player._PlayerQuiver.Name}.";
			OutputController.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				equipString);
		}
		private static void EquipArmor(Player player, Armor armor) {
			if (armor.Equipped) {
				string alreadyEquipString = $"You have already equipped {armor.Name}.";
				OutputController.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					alreadyEquipString);
				return;
			}
			// Check to see if player can use armor type, if so, continue, otherwise return output error msg
			switch (armor.ArmorGroup) {
				case Armor.ArmorType.Cloth when player._CanWearCloth:
					break;
				case Armor.ArmorType.Leather when player._CanWearLeather:
					break;
				case Armor.ArmorType.Plate when player._CanWearPlate:
					break;
				default:
					OutputController.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You cannot wear that type of armor!");
					return;
			}
			switch (armor.ArmorCategory) {
				case Armor.ArmorSlot.Head:
					if (player._PlayerHeadArmor != null && player._PlayerHeadArmor.Equipped) {
						UnequipArmor(player, player._PlayerHeadArmor);
					}
					player._PlayerHeadArmor = armor;
					armor.Equipped = true;
					string equipHeadString = $"You have equipped {player._PlayerHeadArmor.Name}.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						equipHeadString);
					break;
				case Armor.ArmorSlot.Back:
					if (player._PlayerBackArmor != null && player._PlayerBackArmor.Equipped) {
						UnequipArmor(player, player._PlayerBackArmor);
					}
					player._PlayerBackArmor = armor;
					armor.Equipped = true;
					string equipBackString = $"You have equipped {player._PlayerBackArmor.Name}.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						equipBackString);
					break;
				case Armor.ArmorSlot.Chest:
					if (player._PlayerChestArmor != null && player._PlayerChestArmor.Equipped) {
						UnequipArmor(player, player._PlayerChestArmor);
					}
					player._PlayerChestArmor = armor;
					armor.Equipped = true;
					string equipChestString = $"You have equipped {player._PlayerChestArmor.Name}.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						equipChestString);
					break;
				case Armor.ArmorSlot.Wrist:
					if (player._PlayerWristArmor != null && player._PlayerWristArmor.Equipped) {
						UnequipArmor(player, player._PlayerWristArmor);
					}
					player._PlayerWristArmor = armor;
					armor.Equipped = true;
					string equipWristString = $"You have equipped {player._PlayerWristArmor.Name}.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						equipWristString);
					break;
				case Armor.ArmorSlot.Waist:
					if (player._PlayerWaistArmor != null && player._PlayerWaistArmor.Equipped) {
						UnequipArmor(player, player._PlayerWaistArmor);
					}
					player._PlayerWaistArmor = armor;
					armor.Equipped = true;
					string equipWaistString = $"You have equipped {player._PlayerWaistArmor.Name}.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						equipWaistString);
					break;
				case Armor.ArmorSlot.Legs:
					if (player._PlayerLegsArmor != null && player._PlayerLegsArmor.Equipped) {
						UnequipArmor(player, player._PlayerLegsArmor);
					}
					player._PlayerLegsArmor = armor;
					armor.Equipped = true;
					string equipLegsString = $"You have equipped {player._PlayerLegsArmor.Name}.";
					OutputController.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						equipLegsString);
					break;
				case Armor.ArmorSlot.Hands:
					if (player._PlayerHandsArmor != null && player._PlayerHandsArmor.Equipped) {
						UnequipArmor(player, player._PlayerHandsArmor);
					}
					player._PlayerHandsArmor = armor;
					armor.Equipped = true;
					string equipHandsString = $"You have equipped {player._PlayerHandsArmor.Name}.";
					OutputController.Display.StoreUserOutput(
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