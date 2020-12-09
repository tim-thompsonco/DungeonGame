namespace DungeonGame
{
	public interface IEquipment : IName
	{
		bool _Equipped { get; set; }
		int _ItemValue { get; set; }
		int _Weight { get; set; }
		string _Desc { get; set; }
	}
}