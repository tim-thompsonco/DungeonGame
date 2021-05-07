namespace DungeonGame.Items {
	public interface IItem : IName {
		string Desc { get; set; }
		int ItemValue { get; set; }
		int Weight { get; set; }
	}
}
