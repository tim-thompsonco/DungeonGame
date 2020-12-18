namespace DungeonGame.Items
{
	public class Loot : IItem
	{
		public string _Name { get; set; }
		public string _Desc { get; set; }
		public bool _Equipped { get; set; }
		public int _ItemValue { get; set; }
		public int _Weight { get; set; }

		public Loot(string name, int level, int weight)
		{
			_Name = name;
			_Weight = weight;
			// Item value starts at 15 for level 1 then scales up an additional 5 per level
			_ItemValue = 15 + ((level - 1) * 5);
			_Desc = $"A {_Name} that is worth some money to the right vendor.";
		}
	}
}