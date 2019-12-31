using System;
using System.Linq;
using System.Text;

namespace DungeonGame {
	public static class GearHelper {
		public static void EquipInitialGear(Player player, UserOutput output) {
			EquipWeapon(player, player.Inventory[0] as Weapon, output);
			EquipArmor(player, player.Inventory[1] as Armor);
			EquipArmor(player, player.Inventory[2] as Armor);
			EquipArmor(player, player.Inventory[3] as Armor);
			if (player.PlayerClass == Player.PlayerClassType.Archer) {
				EquipQuiver(player, player.Inventory[4] as Quiver, output);
			}
		}
		public static void DecreaseArmorDurability(Player player) {
			player.PlayerChestArmor?.DecreaseDurability();
			player.PlayerHeadArmor?.DecreaseDurability();
			player.PlayerLegsArmor?.DecreaseDurability();
		}
		public static int CheckArmorRating(Player player) {
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
			if (player.IsArmorChanged) {
				totalArmorRating += player.ChangeArmorAmount;
			}
			return totalArmorRating;
		}
		public static void EquipItem(Player player, string[] input, UserOutput output) {
			Console.ForegroundColor = ConsoleColor.Green;
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
									EquipArmor(player, (Armor)item);
									break;
								case "Quiver":
									EquipQuiver(player, (Quiver) item, output);
									break;
							}
							return;
						}
						else if (Helper.IsWearable(item)) {
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
								UnequipArmor(player, (Armor)item);
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
		public static void UnequipArmor(Player player, Armor armor) {
			if (!armor.IsEquipped()) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You have already unequipped {0}.", armor.GetName());
				return;
			}
			armor.Equipped = false;
			var itemSlot = armor.ArmorCategory.ToString();
			switch (itemSlot) {
				case "Head":
					Helper.FormatSuccessOutputText();
					Console.WriteLine("You have unequipped {0}.", player.PlayerHeadArmor.GetName());
					player.PlayerHeadArmor = null;
					break;
				case "Chest":
					Helper.FormatSuccessOutputText();
					Console.WriteLine("You have unequipped {0}.", player.PlayerChestArmor.GetName());
					player.PlayerChestArmor = null;
					break;
				case "Legs":
					Helper.FormatSuccessOutputText();
					Console.WriteLine("You have unequipped {0}.", player.PlayerLegsArmor.GetName());
					player.PlayerLegsArmor = null;
					break;
			}
		}
		public static void EquipWeapon(Player player, Weapon weapon, UserOutput output) {
			if (weapon.IsEquipped()) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You have already equipped {0}.", weapon.GetName());
				return;
			}
			var itemType = weapon.WeaponGroup.ToString();
			switch (itemType) {
				case "Bow" when player.CanUseBow:
					break;
				case "Axe" when player.CanUseAxe:
					break;
				case "Dagger" when player.CanUseDagger:
					break;
				case "OneHandedSword" when player.CanUseOneHandedSword:
					break;
				case "TwoHandedSword" when player.CanUseTwoHandedSword:
					break;
				default:
					Console.WriteLine("You cannot use that type of weapon!");
					return;
			}
			if (player.PlayerWeapon != null && player.PlayerWeapon.IsEquipped()) {
				UnequipWeapon(player, player.PlayerWeapon, output);
			}
			player.PlayerWeapon = weapon;
			weapon.Equipped = true;
			Helper.FormatSuccessOutputText();
			Console.WriteLine("You have equipped {0}.", player.PlayerWeapon.GetName());
		}
		public static void EquipQuiver(Player player, Quiver quiver, UserOutput output) {
			if (quiver.IsEquipped()) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You have already equipped {0}.", quiver.GetName());
				return;
			}
			if (player.PlayerQuiver != null && player.PlayerQuiver.IsEquipped()) {
				UnequipQuiver(player, player.PlayerQuiver, output);
			}
			player.PlayerQuiver = quiver;
			quiver.Equipped = true;
			Helper.FormatSuccessOutputText();
			Console.WriteLine("You have equipped {0}.", player.PlayerQuiver.GetName());
		}
		public static void EquipArmor(Player player, Armor armor) {
			if (armor.IsEquipped()) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You have already equipped {0}.", armor.GetName());
				return;
			}
			var itemSlot = armor.ArmorCategory.ToString();
			var itemType = armor.ArmorGroup.ToString();
			switch (itemType) {
				case "Cloth" when player.CanWearCloth:
					break;
				case "Leather" when player.CanWearLeather:
					break;
				case "Plate" when player.CanWearPlate:
					break;
				default:
					Console.WriteLine("You cannot wear that type of armor!");
					return;
			}
			switch (itemSlot) {
				case "Head":
					if (player.PlayerHeadArmor != null && player.PlayerHeadArmor.IsEquipped()) {
						UnequipArmor(player, player.PlayerHeadArmor);
					}
					player.PlayerHeadArmor = armor;
					armor.Equipped = true;
					Helper.FormatSuccessOutputText();
					Console.WriteLine("You have equipped {0}.", player.PlayerHeadArmor.GetName());
					break;
				case "Chest":
					if (player.PlayerChestArmor != null && player.PlayerChestArmor.IsEquipped()) {
						UnequipArmor(player, player.PlayerChestArmor);
					}
					player.PlayerChestArmor = armor;
					armor.Equipped = true;
					Helper.FormatSuccessOutputText();
					Console.WriteLine("You have equipped {0}.", player.PlayerChestArmor.GetName());
					break;
				case "Legs":
					if (player.PlayerLegsArmor != null && player.PlayerLegsArmor.IsEquipped()) {
						UnequipArmor(player, player.PlayerLegsArmor);
					}
					player.PlayerLegsArmor = armor;
					armor.Equipped = true;
					Helper.FormatSuccessOutputText();
					Console.WriteLine("You have equipped {0}.", player.PlayerLegsArmor.GetName());
					break;
			}
		}
	}
}