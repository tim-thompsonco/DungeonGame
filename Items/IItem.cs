namespace DungeonGame.Items {
	public interface IItem : IName {
		string _Desc { get; set; }
		int _ItemValue { get; set; }
		int _Weight { get; set; }
	}
}
