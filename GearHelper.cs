using System;
using System.Linq;
using System.Text;

namespace DungeonGame {
	public static class GearHelper {
		public static void EquipInitialGear(Player player) {
			EquipWeapon(player, player.Inventory[0] as Weapon);
			EquipArmor(player, player.Inventory[1] as Armor);
			EquipArmor(player, player.Inventory[2] as Armor);
			EquipArmor(player, player.Inventory[3] as Armor);
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
		public static void EquipItem(Player player, string[] input) {
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
				if (itemFound == true && input[0] == "equip") {
					if (Helper.IsWearable(item) && item.IsEquipped() == false) {
						switch (itemType) {
							case "Weapon":
								EquipWeapon(player, (Weapon)item);
								break;
							case "Armor":
								EquipArmor(player, (Armor)item);
								break;
						}
						return;
					}
					else if (Helper.IsWearable(item)) {
						Helper.FormatFailureOutputText();
						Console.WriteLine("You have already equipped that.");
						return;
					}
					Helper.FormatFailureOutputText();
					Console.WriteLine("You can't equip that!");
					return;
				}
				else if (itemFound == true && input[0] == "unequip") {
					if (!Helper.IsWearable(item)) return;
					switch (itemType) {
						case "Weapon":
							UnequipWeapon(player, (Weapon)item);
							break;
						case "Armor":
							UnequipArmor(player, (Armor)item);
							break;
					}
					return;
				}
			}
			Helper.FormatFailureOutputText();
			Console.WriteLine("You don't have {0} in your inventory!", inputName);
		}
		public static void UnequipWeapon(Player player, Weapon weapon) {
			if (weapon.IsEquipped() == false) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You have already unequipped {0}.", weapon.GetName());
				return;
			}
			weapon.Equipped = false;
			Helper.FormatSuccessOutputText();
			Console.WriteLine("You have unequipped {0}.", player.PlayerWeapon.GetName());
			player.PlayerWeapon = null;
		}
		public static void EquipWeapon(Player player, Weapon weapon) {
			if (weapon.IsEquipped() == true) {
				Helper.FormatFailureOutputText();
				Console.WriteLine("You have already equipped {0}.", weapon.GetName());
				return;
			}
			if (player.PlayerWeapon != null && player.PlayerWeapon.Equipped == true) {
				UnequipWeapon(player, player.PlayerWeapon);
			}
			player.PlayerWeapon = weapon;
			weapon.Equipped = true;
			Helper.FormatSuccessOutputText();
			Console.WriteLine("You have equipped {0}.", player.PlayerWeapon.GetName());
		}
		public static void UnequipArmor(Player player, Armor armor) {
			if (armor.IsEquipped() == false) {
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
				default:
					break;
			}
		}
		public static void EquipArmor(Player player, Armor armor) {
			if (armor.IsEquipped() == true) {
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
					if (player.PlayerHeadArmor != null && player.PlayerHeadArmor.Equipped == true) {
						UnequipArmor(player, player.PlayerHeadArmor);
					}
					player.PlayerHeadArmor = armor;
					armor.Equipped = true;
					Helper.FormatSuccessOutputText();
					Console.WriteLine("You have equipped {0}.", player.PlayerHeadArmor.GetName());
					break;
				case "Chest":
					if (player.PlayerChestArmor != null && player.PlayerChestArmor.Equipped == true) {
						UnequipArmor(player, player.PlayerChestArmor);
					}
					player.PlayerChestArmor = armor;
					armor.Equipped = true;
					Helper.FormatSuccessOutputText();
					Console.WriteLine("You have equipped {0}.", player.PlayerChestArmor.GetName());
					break;
				case "Legs":
					if (player.PlayerLegsArmor != null && player.PlayerLegsArmor.Equipped == true) {
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