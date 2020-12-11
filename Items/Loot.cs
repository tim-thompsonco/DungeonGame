namespace DungeonGame
{
	public class Loot : IEquipment
	{
		public enum GemType
		{
			Ruby,
			Emerald,
			Diamond,
			Sapphire,
			Amethyst,
			Topaz
		}
		public enum GemLevel
		{
			Chipped,
			Dull,
			Normal
		}
		public string _Name { get; set; }
		public string _Desc { get; set; }
		public bool _Equipped { get; set; }
		public int _ItemValue { get; set; }
		public int _Weight { get; set; }
		public GemType _GemCategory { get; set; }
		public GemLevel _GemStrength { get; set; }

		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public Loot() { }
		public Loot(string name, int level, int weight)
		{
			_Name = name;
			_Weight = weight;
			_ItemValue = 15 + ((level - 1) * 5);
			_Desc = $"A {_Name} that is worth some money to the right vendor.";
		}
		public Loot(int level, GemType gemType)
		{
			_GemCategory = gemType;
			_Weight = 1;
			_ItemValue = level * 20;
			string name;
			if (level <= 3)
			{
				_GemStrength = GemLevel.Chipped;
				name = $"{GemLevel.Chipped} {gemType}";
			}
			else if (level <= 6)
			{
				_GemStrength = GemLevel.Dull;
				name = $"{GemLevel.Dull} {gemType}";
			}
			else
			{
				_GemStrength = GemLevel.Normal;
				name = gemType.ToString();
			}
			_Name = name.ToLower();
			_Desc = $"A {_Name} that is worth some money to the right vendor.";
		}
	}
}