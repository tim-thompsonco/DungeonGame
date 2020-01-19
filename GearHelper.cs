using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DungeonGame {
	public static class GearHelper {
		public static void EquipInitialGear(Player player, UserOutput output) {
			EquipWeapon(player, player.Inventory[0] as Weapon, output);
			EquipArmor(player, player.Inventory[1] as Armor, output);
			EquipArmor(player, player.Inventory[2] as Armor, output);
			EquipArmor(player, player.Inventory[3] as Armor, output);
			if (player.PlayerClass == Player.PlayerClassType.Archer) {
				EquipQuiver(player, player.Inventory[4] as Quiver, output);
			}
		}
		public static void DecreaseArmorDurability(Player player) {
			player.PlayerChestArmor?.DecreaseDurability();
			player.PlayerHeadArmor?.DecreaseDurability();
			player.PlayerLegsArmor?.DecreaseDurability();
		}
		public static int CheckArmorRating(Player player, UserOutput output) {
			var totalArmorRating = 0;
			if (player.PlayerChestArmor != null && player.PlayerChestArmor.IsEquipped()) {
				totalArmorRating += (int)player.PlayerChestArmor.GetArmorRating();
			}
			if (player.PlayerHeadArmor != null && player.PlayerHeadArmor.IsEquipped()) {
				totalArmorRating += (int)player.PlayerHeadArmor.GetArmorRating();
			}
			if (player.PlayerLegsArmor != null && player.PlayerLegsArmor.IsEquipped()) {
				totalArmorRating += (int)player.PlayerLegsArmor.GetArmorRating();
			}
			if (!player.InCombat) return totalArmorRating;
			Helper.RemovedExpiredEffects(player);
			foreach (var effect in player.Effects) {
				if (effect.EffectGroup != Effect.EffectType.ChangeArmor) continue;
				totalArmorRating += effect.EffectAmountOverTime;
				effect.ChangeArmorRound(output);
			}
			return totalArmorRating;
		}
		public static void DropItem(Player player, string[] input, UserOutput output, List<IRoom> roomList) {
			if (input[1] == null) {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					"What item did you want to drop?");
				return;
			}
			var room = roomList.Find(f => f.X == player.X && f.Y == player.Y && f.Z == player.Z);
			var roomIndex = roomList.IndexOf(room);
			var itemIndex = player.Inventory.FindIndex(
				f => f.Name == input[1] || f.Name.Contains(input[1]));
			if (itemIndex != -1) {
				if (player.Inventory[itemIndex].Equipped) {
					output.StoreUserOutput(
						Helper.FormatFailureOutputText(),
						Helper.FormatDefaultBackground(),
						"You have to unequip that item first!");
					return;
				}
				roomList[roomIndex].RoomObjects.Add(player.Inventory[itemIndex]);
				var dropInventoryString = "You dropped " +
				                        player.Inventory[itemIndex].Name + ".";
				player.Inventory.RemoveAt(itemIndex);
				output.StoreUserOutput(
					Helper.FormatSuccessOutputText(),
					Helper.FormatDefaultBackground(),
					dropInventoryString);
				return;
			}
			itemIndex = player.Consumables.FindIndex(
				f => f.Name == input[1] || f.Name.Contains(input[1]));
			if (itemIndex != -1) {
				roomList[roomIndex].RoomObjects.Add(player.Consumables[itemIndex]);
				var dropConsumableString = "You dropped " +
				                          player.Consumables[itemIndex].Name + ".";
				player.Consumables.RemoveAt(itemIndex);
				output.StoreUserOutput(
					Helper.FormatSuccessOutputText(),
					Helper.FormatDefaultBackground(),
					dropConsumableString);
				return;
			}
			output.StoreUserOutput(
				Helper.FormatFailureOutputText(),
				Helper.FormatDefaultBackground(),
				"You don't have that item in your inventory!");
		}
		public static void PickupItem(Player player, string[] input, UserOutput output, List<IRoom> roomList) {
			if (input[1] == null) {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					"What item did you want to pickup?");
				return;
			}
			var room = roomList.Find(f => f.X == player.X && f.Y == player.Y && f.Z == player.Z);
			var roomIndex = roomList.IndexOf(room);
			var itemIndex = roomList[roomIndex].RoomObjects.FindIndex(
				f => f.GetName() == input[1] || f.GetName().Contains(input[1]));
			if (!(roomList[roomIndex].RoomObjects[itemIndex] is IEquipment item)) {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					"You can't pick that up!");
				return;
			}
			var playerWeight = PlayerHelper.GetInventoryWeight(player);
			var itemWeight = item.Weight;
			if (playerWeight + itemWeight > player.MaxCarryWeight) {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					"You can't carry that much!");
			}
			if (itemIndex == -1) {
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					"That item is not in the room!");
				return;
			}
			if (roomList[roomIndex].RoomObjects[itemIndex].GetType() == typeof(Consumable)) {
				player.Consumables.Add(roomList[roomIndex].RoomObjects[itemIndex] as Consumable);
			}
			else {
				player.Inventory.Add(roomList[roomIndex].RoomObjects[itemIndex] as IEquipment);
			}
			var pickupItemString = "You picked up " +
			                       roomList[roomIndex].RoomObjects[itemIndex].GetName() + ".";
			roomList[roomIndex].RoomObjects.RemoveAt(itemIndex);
			output.StoreUserOutput(
				Helper.FormatSuccessOutputText(),
				Helper.FormatDefaultBackground(),
				pickupItemString);
		}
		public static void EquipItem(Player player, string[] input, UserOutput output) {
			var inputString = new StringBuilder();
			for (var i = 1; i < input.Length; i++) {
				inputString.Append(input[i]);
				inputString.Append(' ');
			}
			var inputName = inputString.ToString().Trim();
			foreach (var item in player.Inventory) {
				var itemName = item.GetName().Split(' ');
				var itemType = item.GetType().Name;
				var itemFound = (itemName.Last() == inputName || item.GetName() == inputName);
				switch (itemFound) {
					case true when input[0] == "equip": {
						if (Helper.IsWearable(item) && item.IsEquipped() == false) {
							switch (itemType) {
								case "Weapon":
									EquipWeapon(player, (Weapon)item, output);
									break;
								case "Armor":
									EquipArmor(player, (Armor)item, output);
									break;
								case "Quiver":
									EquipQuiver(player, (Quiver) item, output);
									break;
							}
							return;
						}
						if (Helper.IsWearable(item)) {
							output.StoreUserOutput(
								Helper.FormatFailureOutputText(),
								Helper.FormatDefaultBackground(),
								"You have already equipped that.");
							return;
						}
						output.StoreUserOutput(
							Helper.FormatFailureOutputText(),
							Helper.FormatDefaultBackground(),
							"You can't equip that!");
						return;
					}
					case true when input[0] == "unequip": {
						if (!Helper.IsWearable(item)) return;
						switch (itemType) {
							case "Weapon":
								UnequipWeapon(player, (Weapon)item, output);
								break;
							case "Armor":
								UnequipArmor(player, (Armor)item, output);
								break;
							case "Quiver":
								UnequipQuiver(player, (Quiver) item, output);
								break;
						}
						return;
					}
				}
			}
			var noItemFoundString = "You don't have " + inputName + " in your inventory!"; 
			output.StoreUserOutput(
				Helper.FormatFailureOutputText(),
				Helper.FormatDefaultBackground(),
				noItemFoundString);
		}
		public static void UnequipWeapon(Player player, Weapon weapon, UserOutput output) {
			if (!weapon.IsEquipped()) {
				var alreadyUnequipString = "You have already unequipped " + weapon.GetName() + "."; 
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					alreadyUnequipString);
				return;
			}
			weapon.Equipped = false;
			var unequipString = "You have unequipped " + player.PlayerWeapon.GetName() + ".";
			output.StoreUserOutput(
				Helper.FormatSuccessOutputText(),
				Helper.FormatDefaultBackground(),
				unequipString);
			player.PlayerWeapon = null;
		}
		public static void UnequipQuiver(Player player, Quiver quiver, UserOutput output) {
			if (!quiver.IsEquipped()) {
				var alreadyUnequipString = "You have already unequipped " + quiver.GetName() + "{0}.";
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					alreadyUnequipString);
				return;
			}
			quiver.Equipped = false;
			var unequipString = "You have unequipped " + player.PlayerQuiver.GetName() + ".";
			output.StoreUserOutput(
				Helper.FormatSuccessOutputText(),
				Helper.FormatDefaultBackground(),
				unequipString);
			player.PlayerQuiver = null;
		}
		public static void UnequipArmor(Player player, Armor armor, UserOutput output) {
			if (!armor.IsEquipped()) {
				var alreadyUnequipString = "You have already unequipped " + armor.GetName() + ".";
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					alreadyUnequipString);
				return;
			}
			armor.Equipped = false;
			switch (armor.ArmorCategory) {
				case Armor.ArmorSlot.Head:
					var unequipHeadString = "You have unequipped " + player.PlayerHeadArmor.GetName() + ".";
					output.StoreUserOutput(
						Helper.FormatSuccessOutputText(),
						Helper.FormatDefaultBackground(),
						unequipHeadString);
					player.PlayerHeadArmor = null;
					break;
				case Armor.ArmorSlot.Back:
					var unequipBackString = "You have unequipped " + player.PlayerBackArmor.GetName() + ".";
					output.StoreUserOutput(
						Helper.FormatSuccessOutputText(),
						Helper.FormatDefaultBackground(),
						unequipBackString);
					player.PlayerBackArmor = null;
					break;
				case Armor.ArmorSlot.Chest:
					var unequipChestString = "You have unequipped " + player.PlayerChestArmor.GetName() + ".";
					output.StoreUserOutput(
						Helper.FormatSuccessOutputText(),
						Helper.FormatDefaultBackground(),
						unequipChestString);
					player.PlayerChestArmor = null;
					break;
				case Armor.ArmorSlot.Wrist:
					var unequipWristString = "You have unequipped " + player.PlayerWristArmor.GetName() + ".";
					output.StoreUserOutput(
						Helper.FormatSuccessOutputText(),
						Helper.FormatDefaultBackground(),
						unequipWristString);
					player.PlayerWristArmor = null;
					break;
				case Armor.ArmorSlot.Waist:
					var unequipWaistString = "You have unequipped " + player.PlayerWaistArmor.GetName() + ".";
					output.StoreUserOutput(
						Helper.FormatSuccessOutputText(),
						Helper.FormatDefaultBackground(),
						unequipWaistString);
					player.PlayerWaistArmor = null;
					break;
				case Armor.ArmorSlot.Legs:
					var unequipLegsString = "You have unequipped " + player.PlayerLegsArmor.GetName() + ".";
					output.StoreUserOutput(
						Helper.FormatSuccessOutputText(),
						Helper.FormatDefaultBackground(),
						unequipLegsString);
					player.PlayerLegsArmor = null;
					break;
				case Armor.ArmorSlot.Hands:
					var unequipHandsString = "You have unequipped " + player.PlayerHandsArmor.GetName() + ".";
					output.StoreUserOutput(
						Helper.FormatSuccessOutputText(),
						Helper.FormatDefaultBackground(),
						unequipHandsString);
					player.PlayerHandsArmor = null;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public static void EquipWeapon(Player player, Weapon weapon, UserOutput output) {
			if (weapon.IsEquipped()) {
				var alreadyEquipString = "You have already equipped " + weapon.GetName() + ".";
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
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
					output.StoreUserOutput(
						Helper.FormatFailureOutputText(),
						Helper.FormatDefaultBackground(),
						"You cannot use that type of weapon!");
					return;
			}
			if (player.PlayerWeapon != null && player.PlayerWeapon.IsEquipped()) {
				UnequipWeapon(player, player.PlayerWeapon, output);
			}
			player.PlayerWeapon = weapon;
			weapon.Equipped = true;
			var equipSuccessString = "You have equipped " + player.PlayerWeapon.GetName() + ".";
			output.StoreUserOutput(
				Helper.FormatSuccessOutputText(),
				Helper.FormatDefaultBackground(),
				equipSuccessString);
		}
		public static void EquipQuiver(Player player, Quiver quiver, UserOutput output) {
			if (quiver.IsEquipped()) {
				var alreadyEquipString = "You have already equipped " + quiver.GetName() + ".";
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
					alreadyEquipString);
				return;
			}
			if (player.PlayerQuiver != null && player.PlayerQuiver.IsEquipped()) {
				UnequipQuiver(player, player.PlayerQuiver, output);
			}
			player.PlayerQuiver = quiver;
			quiver.Equipped = true;
			var equipString = "You have equipped " + player.PlayerQuiver.GetName() + ".";
			output.StoreUserOutput(
				Helper.FormatSuccessOutputText(),
				Helper.FormatDefaultBackground(),
				equipString);
		}
		public static void EquipArmor(Player player, Armor armor, UserOutput output) {
			if (armor.IsEquipped()) {
				var alreadyEquipString = "You have already equipped " + armor.GetName() + ".";
				output.StoreUserOutput(
					Helper.FormatFailureOutputText(),
					Helper.FormatDefaultBackground(),
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
					output.StoreUserOutput(
						Helper.FormatFailureOutputText(),
						Helper.FormatDefaultBackground(),
						"You cannot wear that type of armor!");
					return;
			}
			switch (armor.ArmorCategory) {
				case Armor.ArmorSlot.Head:
					if (player.PlayerHeadArmor != null && player.PlayerHeadArmor.IsEquipped()) {
						UnequipArmor(player, player.PlayerHeadArmor, output);
					}
					player.PlayerHeadArmor = armor;
					armor.Equipped = true;
					var equipHeadString = "You have equipped " + player.PlayerHeadArmor.GetName() + ".";
					output.StoreUserOutput(
						Helper.FormatSuccessOutputText(),
						Helper.FormatDefaultBackground(),
						equipHeadString);
					break;
				case Armor.ArmorSlot.Back:
					if (player.PlayerBackArmor != null && player.PlayerBackArmor.IsEquipped()) {
						UnequipArmor(player, player.PlayerBackArmor, output);
					}
					player.PlayerBackArmor = armor;
					armor.Equipped = true;
					var equipBackString = "You have equipped " + player.PlayerBackArmor.GetName() + ".";
					output.StoreUserOutput(
						Helper.FormatSuccessOutputText(),
						Helper.FormatDefaultBackground(),
						equipBackString);
					break;
				case Armor.ArmorSlot.Chest:
					if (player.PlayerChestArmor != null && player.PlayerChestArmor.IsEquipped()) {
						UnequipArmor(player, player.PlayerChestArmor, output);
					}
					player.PlayerChestArmor = armor;
					armor.Equipped = true;
					var equipChestString = "You have equipped " + player.PlayerChestArmor.GetName() + ".";
					output.StoreUserOutput(
						Helper.FormatSuccessOutputText(),
						Helper.FormatDefaultBackground(),
						equipChestString);
					break;
				case Armor.ArmorSlot.Wrist:
					if (player.PlayerWristArmor != null && player.PlayerWristArmor.IsEquipped()) {
						UnequipArmor(player, player.PlayerWristArmor, output);
					}
					player.PlayerWristArmor = armor;
					armor.Equipped = true;
					var equipWristString = "You have equipped " + player.PlayerWristArmor.GetName() + ".";
					output.StoreUserOutput(
						Helper.FormatSuccessOutputText(),
						Helper.FormatDefaultBackground(),
						equipWristString);
					break;
				case Armor.ArmorSlot.Waist:
					if (player.PlayerWaistArmor != null && player.PlayerWaistArmor.IsEquipped()) {
						UnequipArmor(player, player.PlayerWaistArmor, output);
					}
					player.PlayerWaistArmor = armor;
					armor.Equipped = true;
					var equipWaistString = "You have equipped " + player.PlayerWaistArmor.GetName() + ".";
					output.StoreUserOutput(
						Helper.FormatSuccessOutputText(),
						Helper.FormatDefaultBackground(),
						equipWaistString);
					break;
				case Armor.ArmorSlot.Legs:
					if (player.PlayerLegsArmor != null && player.PlayerLegsArmor.IsEquipped()) {
						UnequipArmor(player, player.PlayerLegsArmor, output);
					}
					player.PlayerLegsArmor = armor;
					armor.Equipped = true;
					var equipLegsString = "You have equipped " + player.PlayerLegsArmor.GetName() + ".";
					output.StoreUserOutput(
						Helper.FormatSuccessOutputText(),
						Helper.FormatDefaultBackground(),
						equipLegsString);
					break;
				case Armor.ArmorSlot.Hands:
					if (player.PlayerHandsArmor != null && player.PlayerHandsArmor.IsEquipped()) {
						UnequipArmor(player, player.PlayerHandsArmor, output);
					}
					player.PlayerHandsArmor = armor;
					armor.Equipped = true;
					var equipHandsString = "You have equipped " + player.PlayerHandsArmor.GetName() + ".";
					output.StoreUserOutput(
						Helper.FormatSuccessOutputText(),
						Helper.FormatDefaultBackground(),
						equipHandsString);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}