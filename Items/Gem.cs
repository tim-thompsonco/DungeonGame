namespace DungeonGame.Items
{
	public class Gem : IItem
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
		private enum GemLevel
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
		private readonly GemType _GemType;
		private GemLevel _GemLevel;

		public Gem(int level, GemType gemType)
		{
			_GemType = gemType;
			_Weight = 1;
			_ItemValue = level * 20;
			SetGemLevel(level);
			SetGemName();
			_Desc = $"A {_Name} that is worth some money to the right vendor.";
		}

		private void SetGemLevel(int level)
		{
			if (level <= 3)
			{
				_GemLevel = GemLevel.Chipped;
			}
			else if (level <= 6)
			{
				_GemLevel = GemLevel.Dull;
			}
			// If gem level is not chipped or dull, then it is normal
			else
			{
				_GemLevel = GemLevel.Normal;
			}
		}

		private void SetGemName()
		{
			// Gem naming format is "<gem type>" for normal gem
			if (_GemLevel == GemLevel.Normal)
			{
				_Name = _GemType.ToString().ToLower();
			}
			// Gem naming format is "<gem level> <gem type>" for chipped or dull gems
			else
			{
				_Name = $"{_GemLevel} {_GemType}".ToLower();
			}
		}
	}
}
