namespace DungeonGame.Items.Gems {
	public class Gem : IItem {
		public string Desc { get; set; }
		public bool Equipped { get; set; }
		public int ItemValue { get; set; }
		public string Name { get; set; }
		public int Weight { get; set; } = 1;
		private GemLevel _gemLevel;
		private readonly GemType _gemType;

		public Gem(int level, GemType gemType) {
			_gemType = gemType;
			ItemValue = level * 20;
			SetGemLevel(level);
			SetGemName();
			Desc = $"A {Name} that is worth some money to the right vendor.";
		}

		private void SetGemLevel(int level) {
			if (level <= 3) {
				_gemLevel = GemLevel.Chipped;
			} else if (level <= 6) {
				_gemLevel = GemLevel.Dull;
			} else {
				_gemLevel = GemLevel.Normal;
			}
		}

		private void SetGemName() {
			// Gem naming format is "<gem type>" for normal gem
			// Gem naming format is "<gem level> <gem type>" for chipped or dull gems
			Name = _gemLevel == GemLevel.Normal ? _gemType.ToString().ToLower() : $"{_gemLevel} {_gemType}".ToLower();
		}
	}
}
