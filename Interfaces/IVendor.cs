using System.Collections.Generic;

namespace DungeonGame {
	public interface IVendor : IRoomInteraction {
		string Name { get; set; }
		string Desc { get; set; }
		string BuySellType { get; set; }
		List<IEquipment> VendorItems { get; set; }

		void DisplayGearForSale(Player player);
		void BuyItem(Player player, string[] userInput, IEquipment buyItem, int index);
		void BuyItem(Player player, string[] userInput, IEquipment buyItem, int index, string inputName);
		void BuyItemCheck(Player player, string[] userInput);
		void SellItem(Player player, string[] userInput, IEquipment sellItem, int index);
		void SellItemCheck(Player player, string[] userInput, UserOutput output);
		void RepairItem(Player player, string[] userInput);
		void RestorePlayer(Player player);
		void RepopulateHealerPotion(string inputName);
	}
}