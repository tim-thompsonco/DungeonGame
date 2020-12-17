namespace DungeonGame.Items
{
	public class Consumable : IEquipment
	{
		
		public string _Name { get; set; }
		public string _Desc { get; set; }
		public int _ItemValue { get; set; }
		public bool _Equipped { get; set; }
		public int _Weight { get; set; }

		public Consumable() { }
	}
}
