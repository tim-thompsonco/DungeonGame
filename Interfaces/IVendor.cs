using System.Collections.Generic;

namespace DungeonGame {
	public interface IVendor : IRoomInteraction {
    string Name { get; set; }
    string Desc { get; set; }
    List<IEquipment> VendorItems { get; set; }

    void DisplayGearForSale();
  }
}