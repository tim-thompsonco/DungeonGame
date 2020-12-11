namespace DungeonGame.Items
{
	public class Gem : IEquipment
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

		public Gem(int level, GemType gemType)
		{
			_GemCategory = gemType;
			_Weight = 1;
			_ItemValue = level * 20;
			SetGemName(level, gemType);
			_Desc = $"A {_Name} that is worth some money to the right vendor.";
		}

		private void SetGemName(int level, GemType gemType)
		{
			if (level <= 3)
			{
				SetChippedGemName(gemType);
			}
			else if (level <= 6)
			{
				SetDullGemName(gemType);
			}
			else
			{
				SetNormalGemName(gemType);
			}
		}

		private void SetChippedGemName(GemType gemType)
		{
			_GemStrength = GemLevel.Chipped;
			_Name = $"{GemLevel.Chipped} {gemType}".ToLower();
		}

		private void SetDullGemName(GemType gemType)
		{
			_GemStrength = GemLevel.Dull;
			_Name = $"{GemLevel.Dull} {gemType}".ToLower();
		}

		private void SetNormalGemName(GemType gemType)
		{
			_GemStrength = GemLevel.Normal;
			_Name = gemType.ToString().ToLower();
		}
	}
}
