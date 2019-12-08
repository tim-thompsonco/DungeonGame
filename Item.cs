namespace DungeonGame {
	public class Item : IRoomInteraction {
		public string Name { get; }
		public int ItemValue { get; }

		public Item(string name, int itemValue) {
			this.Name = name;
			this.ItemValue = itemValue;
		}

		public string GetName() {
			return this.Name;
		}
	}
}