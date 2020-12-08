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
		public GemType GemCategory { get; set; }
		public GemLevel GemStrength { get; set; }

		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public Loot() { }
		public Loot(string name, int level, int weight)
		{
			this._Name = name;
			this._Weight = weight;
			this._ItemValue = 15 + (level - 1) * 5;
			this._Desc = "A " + this._Name + " that is worth some money to the right vendor.";
		}
		public Loot(int level, GemType gemType)
		{
			this.GemCategory = gemType;
			this._Weight = 1;
			this._ItemValue = level * 20;
			string name;
			if (level <= 3)
			{
				this.GemStrength = GemLevel.Chipped;
				name = GemLevel.Chipped + " " + gemType;
			}
			else if (level <= 6)
			{
				this.GemStrength = GemLevel.Dull;
				name = GemLevel.Dull + " " + gemType;
			}
			else
			{
				this.GemStrength = GemLevel.Normal;
				name = gemType.ToString();
			}
			this._Name = name.ToLower();
			this._Desc = "A " + this._Name + " that is worth some money to the right vendor.";
		}
	}
}