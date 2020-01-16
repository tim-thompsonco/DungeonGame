using System.Collections.Generic;

namespace DungeonGame {
	public interface IVendor : IRoomInteraction {
		string Name { get; set; }
		string Desc { get; set; }
		string BuySellType { get; set; }
		List<IEquipment> VendorItems { get; set; }

		void DisplayGearForSale(Player player, UserOutput output);
		void BuyItem(Player player, string[] userInput, IEquipment buyItem, int index, UserOutput output);
		void BuyItem(Player player, string[] userInput, IEquipment buyItem, int index, string inputName, UserOutput output);
		void BuyItemCheck(Player player, string[] userInput, UserOutput output);
		void SellItem(Player player, string[] userInput, IEquipment sellItem, int index, UserOutput output);
		void SellItemCheck(Player player, string[] userInput, UserOutput output);
		void RepairItem(Player player, string[] userInput, UserOutput output);
		void RestorePlayer(Player player, UserOutput output);
		void RepopulateHealerPotion(string inputName);
	}
}