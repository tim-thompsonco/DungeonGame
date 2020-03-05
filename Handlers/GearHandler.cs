using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DungeonGame {
	public static class GearHandler {
		private static bool IsWearable(IEquipment item) {
			return item.GetType().Name == "Armor" || item.GetType().Name == "Weapon" || item.GetType().Name == "Quiver";
		}
		public static void EquipInitialGear(Player player) {
			EquipWeapon(player, player.Inventory[0] as Weapon);
			EquipArmor(player, player.Inventory[1] as Armor);
			EquipArmor(player, player.Inventory[2] as Armor);
			EquipArmor(player, player.Inventory[3] as Armor);
			if (player.PlayerClass == Player.PlayerClassType.Archer) {
				EquipQuiver(player, player.Inventory[4] as Quiver);
			}
		}
		public static void DecreaseArmorDurability(Player player) {
			player.PlayerChestArmor?.DecreaseDurability();
			player.PlayerHeadArmor?.DecreaseDurability();
			player.PlayerLegsArmor?.DecreaseDurability();
		}
		public static int CheckArmorRating(Player player) {
			var totalArmorRating = 0;
			if (player.PlayerChestArmor != null && player.PlayerChestArmor.Equipped) {
				totalArmorRating += (int)player.PlayerChestArmor.GetArmorRating();
			}
			if (player.PlayerHeadArmor != null && player.PlayerHeadArmor.Equipped) {
				totalArmorRating += (int)player.PlayerHeadArmor.GetArmorRating();
			}
			if (player.PlayerLegsArmor != null && player.PlayerLegsArmor.Equipped) {
				totalArmorRating += (int)player.PlayerLegsArmor.GetArmorRating();
			}
			if (!player.InCombat) return totalArmorRating;
			GameHandler.RemovedExpiredEffects(player);
			totalArmorRating += player.Effects.Where(
				effect => effect.EffectGroup == Effect.EffectType.ChangeArmor).Sum(effect => effect.EffectAmountOverTime);
			return totalArmorRating < 0 ? 0 : totalArmorRating;
		}
		public static void UseWeaponKit(Player player, string[] userInput) {
			var weaponIndex = player.Inventory.FindIndex(f => f.Name.Contains(userInput[1]));
			if (weaponIndex == -1) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What weapon did you want to upgrade?");
				return;
			}
			var kitIndex = player.Consumables.FindIndex(f => f.Name.Contains(userInput[2]));
			if (kitIndex == -1) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What weapon kit did you want to use?");
				return;
			}
			var weapon = player.Inventory[weaponIndex] as Weapon;
			if (player.Consumables[kitIndex].KitCategory != Consumable.KitType.Weapon) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't upgrade " + weapon.Name + " with that!");
				return;
			}
			if (player.Consumables[kitIndex].ChangeWeapon.KitCategory == ChangeWeapon.KitType.Bowstring && 
				weapon.WeaponGroup != Weapon.WeaponType.Bow || 
				player.Consumables[kitIndex].ChangeWeapon.KitCategory == ChangeWeapon.KitType.Grindstone && 
				weapon.WeaponGroup == Weapon.WeaponType.Bow) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't upgrade " + weapon.Name + " with that!");
				return;
			}
			if (!weapon.Equipped) {
				var inputValid = false;
				while (!inputValid) {
					var weaponString = weapon.Name + " is not equipped. Are you sure you want to upgrade that?";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(), 
						Settings.FormatDefaultBackground(),
						weaponString);
					OutputHandler.Display.BuildUserOutput();
					OutputHandler.Display.ClearUserOutput();
					var input = InputHandler.ParseInput(InputHandler.GetFormattedInput(Console.ReadLine()));
					if (input == "no" || input == "n") return;
					if (input == "yes" || input == "y") inputValid = true;
				}
			}
			player.Consumables[kitIndex].ChangeWeapon.ChangeWeaponPlayer(weapon);
			var upgradeSuccess = "You upgraded " + weapon.Name + " with a weapon kit.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(), 
				Settings.FormatDefaultBackground(),
				upgradeSuccess);
			player.Consumables.RemoveAt(kitIndex);
		}
		public static void UseArmorKit(Player player, string[] userInput) {
			var armorIndex = player.Inventory.FindIndex(f => f.Name.Contains(userInput[1]));
			if (armorIndex == -1) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What armor did you want to upgrade?");
				return;
			}
			var kitIndex = player.Consumables.FindIndex(f => f.Name.Contains(userInput[2]));
			if (kitIndex == -1) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What armor kit did you want to use?");
				return;
			}
			var armor = player.Inventory[armorIndex] as Armor;
			if (player.Consumables[kitIndex].KitCategory != Consumable.KitType.Armor) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't upgrade " + armor.Name + " with that!");
				return;
			}
			if (player.Consumables[kitIndex].ChangeArmor.KitCategory == ChangeArmor.KitType.Cloth && 
				armor.ArmorGroup != Armor.ArmorType.Cloth || 
				player.Consumables[kitIndex].ChangeArmor.KitCategory == ChangeArmor.KitType.Leather && 
				armor.ArmorGroup != Armor.ArmorType.Leather || 
				player.Consumables[kitIndex].ChangeArmor.KitCategory == ChangeArmor.KitType.Plate &&
			     armor.ArmorGroup != Armor.ArmorType.Plate ||
				player.Consumables[kitIndex].KitCategory == Consumable.KitType.Weapon) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't upgrade " + armor.Name + " with that!");
				return;
			}
			if (!armor.Equipped) {
				var inputValid = false;
				while (!inputValid) {
					var armorString = armor.Name + " is not equipped. Are you sure you want to upgrade that?";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(), 
						Settings.FormatDefaultBackground(),
						armorString);
					OutputHandler.Display.BuildUserOutput();
					OutputHandler.Display.ClearUserOutput();
					var input = InputHandler.ParseInput(InputHandler.GetFormattedInput(Console.ReadLine()));
					if (input == "no" || input == "n") return;
					if (input == "yes" || input == "y") inputValid = true;
				}
			}
			player.Consumables[kitIndex].ChangeArmor.ChangeArmorPlayer(armor);
			var upgradeSuccess = "You upgraded " + armor.Name + " with an armor kit.";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(), 
				Settings.FormatDefaultBackground(),
				upgradeSuccess);
			player.Consumables.RemoveAt(kitIndex);
		}
		public static void DropItem(Player player, string[] input) {
			if (input[1] == null) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What item did you want to drop?");
				return;
			}
			var room = RoomHandler.Rooms.Find(f => f.X == player.X && f.Y == player.Y && f.Z == player.Z);
			var roomIndex = RoomHandler.Rooms.IndexOf(room);
			var itemIndex = player.Inventory.FindIndex(
				f => f.Name == input[1] || f.Name.Contains(input[1]));
			if (itemIndex != -1) {
				if (player.Inventory[itemIndex].Equipped) {
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You have to unequip that item first!");
					return;
				}
				RoomHandler.Rooms[roomIndex].RoomObjects.Add(player.Inventory[itemIndex]);
				var dropInventoryString = "You dropped " +
				                        player.Inventory[itemIndex].Name + ".";
				player.Inventory.RemoveAt(itemIndex);
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatSuccessOutputText(),
					Settings.FormatDefaultBackground(),
					dropInventoryString);
				return;
			}
			itemIndex = player.Consumables.FindIndex(
				f => f.Name == input[1] || f.Name.Contains(input[1]));
			if (itemIndex != -1) {
				RoomHandler.Rooms[roomIndex].RoomObjects.Add(player.Consumables[itemIndex]);
				var dropConsumableString = "You dropped " +
				                          player.Consumables[itemIndex].Name + ".";
				player.Consumables.RemoveAt(itemIndex);
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
		public static void PickupItem(Player player, string[] input) {
			if (input[1] == null) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"What item did you want to pickup?");
				return;
			}
			var room = RoomHandler.Rooms.Find(f => f.X == player.X && f.Y == player.Y && f.Z == player.Z);
			var roomIndex = RoomHandler.Rooms.IndexOf(room);
			var itemIndex = RoomHandler.Rooms[roomIndex].RoomObjects.FindIndex(
				f => f.Name == input[1] || f.Name.Contains(input[1]));
			if (!(RoomHandler.Rooms[roomIndex].RoomObjects[itemIndex] is IEquipment item)) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't pick that up!");
				return;
			}
			var playerWeight = PlayerHandler.GetInventoryWeight(player);
			var itemWeight = item.Weight;
			if (playerWeight + itemWeight > player.MaxCarryWeight) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"You can't carry that much!");
			}
			if (itemIndex == -1) {
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					"That item is not in the room!");
				return;
			}
			if (RoomHandler.Rooms[roomIndex].RoomObjects[itemIndex].GetType() == typeof(Consumable)) {
				player.Consumables.Add(RoomHandler.Rooms[roomIndex].RoomObjects[itemIndex] as Consumable);
			}
			else {
				player.Inventory.Add(RoomHandler.Rooms[roomIndex].RoomObjects[itemIndex] as IEquipment);
			}
			var pickupItemString = "You picked up " +
			                       RoomHandler.Rooms[roomIndex].RoomObjects[itemIndex].Name + ".";
			RoomHandler.Rooms[roomIndex].RoomObjects.RemoveAt(itemIndex);
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				pickupItemString);
		}
		public static void EquipItem(Player player, string[] input) {
			var inputString = new StringBuilder();
			for (var i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			var inputName = inputString.ToString().Trim();
			foreach (var item in player.Inventory) {
				var itemName = item.Name.Split(' ');
				var itemType = item.GetType().Name;
				var itemFound = itemName.Last() == inputName || item.Name == inputName ||
				                 itemName.Contains(inputName);
				switch (itemFound) {
					case true when input[0] == "equip": {
						if (IsWearable(item) && item.Equipped == false) {
							switch (itemType) {
								case "Weapon":
									EquipWeapon(player, (Weapon)item);
									break;
								case "Armor":
									EquipArmor(player, (Armor)item);
									break;
								case "Quiver":
									EquipQuiver(player, (Quiver) item);
									break;
							}
							return;
						}
						if (IsWearable(item)) {
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
					case true when input[0] == "unequip": {
						if (!IsWearable(item)) return;
						switch (itemType) {
							case "Weapon":
								UnequipWeapon(player, (Weapon)item);
								break;
							case "Armor":
								UnequipArmor(player, (Armor)item);
								break;
							case "Quiver":
								UnequipQuiver(player, (Quiver) item);
								break;
						}
						return;
					}
				}
			}
			var noItemFoundString = "You don't have " + inputName + " in your inventory!"; 
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatFailureOutputText(),
				Settings.FormatDefaultBackground(),
				noItemFoundString);
		}
		private static void UnequipWeapon(Player player, Weapon weapon) {
			if (!weapon.Equipped) {
				var alreadyUnequipString = "You have already unequipped " + weapon.Name + "."; 
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					alreadyUnequipString);
				return;
			}
			weapon.Equipped = false;
			var unequipString = "You have unequipped " + player.PlayerWeapon.Name + ".";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				unequipString);
			player.PlayerWeapon = null;
		}
		private static void UnequipQuiver(Player player, Quiver quiver) {
			if (!quiver.Equipped) {
				var alreadyUnequipString = "You have already unequipped " + quiver.Name + "{0}.";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					alreadyUnequipString);
				return;
			}
			quiver.Equipped = false;
			var unequipString = "You have unequipped " + player.PlayerQuiver.Name + ".";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				unequipString);
			player.PlayerQuiver = null;
		}
		private static void UnequipArmor(Player player, Armor armor) {
			if (!armor.Equipped) {
				var alreadyUnequipString = "You have already unequipped " + armor.Name + ".";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					alreadyUnequipString);
				return;
			}
			armor.Equipped = false;
			switch (armor.ArmorCategory) {
				case Armor.ArmorSlot.Head:
					var unequipHeadString = "You have unequipped " + player.PlayerHeadArmor.Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipHeadString);
					player.PlayerHeadArmor = null;
					break;
				case Armor.ArmorSlot.Back:
					var unequipBackString = "You have unequipped " + player.PlayerBackArmor.Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipBackString);
					player.PlayerBackArmor = null;
					break;
				case Armor.ArmorSlot.Chest:
					var unequipChestString = "You have unequipped " + player.PlayerChestArmor.Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipChestString);
					player.PlayerChestArmor = null;
					break;
				case Armor.ArmorSlot.Wrist:
					var unequipWristString = "You have unequipped " + player.PlayerWristArmor.Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipWristString);
					player.PlayerWristArmor = null;
					break;
				case Armor.ArmorSlot.Waist:
					var unequipWaistString = "You have unequipped " + player.PlayerWaistArmor.Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipWaistString);
					player.PlayerWaistArmor = null;
					break;
				case Armor.ArmorSlot.Legs:
					var unequipLegsString = "You have unequipped " + player.PlayerLegsArmor.Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipLegsString);
					player.PlayerLegsArmor = null;
					break;
				case Armor.ArmorSlot.Hands:
					var unequipHandsString = "You have unequipped " + player.PlayerHandsArmor.Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						unequipHandsString);
					player.PlayerHandsArmor = null;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private static void EquipWeapon(Player player, Weapon weapon) {
			if (weapon.Equipped) {
				var alreadyEquipString = "You have already equipped " + weapon.Name + ".";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					alreadyEquipString);
				return;
			}
			// Check to see if player can use weapon, if so, continue, otherwise return output error msg
			switch (weapon.WeaponGroup) {
				case Weapon.WeaponType.Dagger when player.CanUseDagger:
					break;
				case Weapon.WeaponType.OneHandedSword when player.CanUseOneHandedSword:
					break;
				case Weapon.WeaponType.TwoHandedSword when player.CanUseTwoHandedSword:
					break;
				case Weapon.WeaponType.Axe when player.CanUseAxe:
					break;
				case Weapon.WeaponType.Bow when player.CanUseBow:
					break;
				default:
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You cannot use that type of weapon!");
					return;
			}
			if (player.PlayerWeapon != null && player.PlayerWeapon.Equipped) {
				UnequipWeapon(player, player.PlayerWeapon);
			}
			player.PlayerWeapon = weapon;
			weapon.Equipped = true;
			var equipSuccessString = "You have equipped " + player.PlayerWeapon.Name + ".";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				equipSuccessString);
		}
		private static void EquipQuiver(Player player, Quiver quiver) {
			if (quiver.Equipped) {
				var alreadyEquipString = "You have already equipped " + quiver.Name + ".";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					alreadyEquipString);
				return;
			}
			if (player.PlayerQuiver != null && player.PlayerQuiver.Equipped) {
				UnequipQuiver(player, player.PlayerQuiver);
			}
			player.PlayerQuiver = quiver;
			quiver.Equipped = true;
			var equipString = "You have equipped " + player.PlayerQuiver.Name + ".";
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				equipString);
		}
		private static void EquipArmor(Player player, Armor armor) {
			if (armor.Equipped) {
				var alreadyEquipString = "You have already equipped " + armor.Name + ".";
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatFailureOutputText(),
					Settings.FormatDefaultBackground(),
					alreadyEquipString);
				return;
			}
			// Check to see if player can use armor type, if so, continue, otherwise return output error msg
			switch (armor.ArmorGroup) {
				case Armor.ArmorType.Cloth when player.CanWearCloth:
					break;
				case Armor.ArmorType.Leather when player.CanWearLeather:
					break;
				case Armor.ArmorType.Plate when player.CanWearPlate:
					break;
				default:
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatFailureOutputText(),
						Settings.FormatDefaultBackground(),
						"You cannot wear that type of armor!");
					return;
			}
			switch (armor.ArmorCategory) {
				case Armor.ArmorSlot.Head:
					if (player.PlayerHeadArmor != null && player.PlayerHeadArmor.Equipped) {
						UnequipArmor(player, player.PlayerHeadArmor);
					}
					player.PlayerHeadArmor = armor;
					armor.Equipped = true;
					var equipHeadString = "You have equipped " + player.PlayerHeadArmor.Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						equipHeadString);
					break;
				case Armor.ArmorSlot.Back:
					if (player.PlayerBackArmor != null && player.PlayerBackArmor.Equipped) {
						UnequipArmor(player, player.PlayerBackArmor);
					}
					player.PlayerBackArmor = armor;
					armor.Equipped = true;
					var equipBackString = "You have equipped " + player.PlayerBackArmor.Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						equipBackString);
					break;
				case Armor.ArmorSlot.Chest:
					if (player.PlayerChestArmor != null && player.PlayerChestArmor.Equipped) {
						UnequipArmor(player, player.PlayerChestArmor);
					}
					player.PlayerChestArmor = armor;
					armor.Equipped = true;
					var equipChestString = "You have equipped " + player.PlayerChestArmor.Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						equipChestString);
					break;
				case Armor.ArmorSlot.Wrist:
					if (player.PlayerWristArmor != null && player.PlayerWristArmor.Equipped) {
						UnequipArmor(player, player.PlayerWristArmor);
					}
					player.PlayerWristArmor = armor;
					armor.Equipped = true;
					var equipWristString = "You have equipped " + player.PlayerWristArmor.Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						equipWristString);
					break;
				case Armor.ArmorSlot.Waist:
					if (player.PlayerWaistArmor != null && player.PlayerWaistArmor.Equipped) {
						UnequipArmor(player, player.PlayerWaistArmor);
					}
					player.PlayerWaistArmor = armor;
					armor.Equipped = true;
					var equipWaistString = "You have equipped " + player.PlayerWaistArmor.Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						equipWaistString);
					break;
				case Armor.ArmorSlot.Legs:
					if (player.PlayerLegsArmor != null && player.PlayerLegsArmor.Equipped) {
						UnequipArmor(player, player.PlayerLegsArmor);
					}
					player.PlayerLegsArmor = armor;
					armor.Equipped = true;
					var equipLegsString = "You have equipped " + player.PlayerLegsArmor.Name + ".";
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						equipLegsString);
					break;
				case Armor.ArmorSlot.Hands:
					if (player.PlayerHandsArmor != null && player.PlayerHandsArmor.Equipped) {
						UnequipArmor(player, player.PlayerHandsArmor);
					}
					player.PlayerHandsArmor = armor;
					armor.Equipped = true;
					var equipHandsString = "You have equipped " + player.PlayerHandsArmor.Name + ".";
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