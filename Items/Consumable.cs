using System;

namespace DungeonGame.Items
{
	public class Consumable : IEquipment
	{
		public enum KitLevel
		{
			Light,
			Medium,
			Heavy
		}
		public enum KitType
		{
			Armor,
			Weapon
		}
		public string _Name { get; set; }
		public string _Desc { get; set; }
		public int _ItemValue { get; set; }
		public bool _Equipped { get; set; }
		public KitLevel? _KitStrength { get; set; }
		public KitType? _KitCategory { get; set; }
		public ChangeArmor _ChangeArmor { get; set; }
		public ChangeWeapon _ChangeWeapon { get; set; }
		public int _Weight { get; set; }

		// Default constructor for JSON serialization to work since there isn't 1 main constructor
		public Consumable() { }
		public Consumable(KitLevel kitLevel, KitType kitType, ChangeArmor.KitType kitCategory)
		{
			_KitCategory = kitType;
			_Name = $"{kitLevel.ToString().ToLower()} {kitCategory.ToString().ToLower()} {kitType.ToString().ToLower()} kit";
			_Weight = 1;
			_KitStrength = kitLevel;
			int amount = _KitStrength switch
			{
				KitLevel.Light => 1,
				KitLevel.Medium => 2,
				KitLevel.Heavy => 3,
				_ => throw new ArgumentOutOfRangeException()
			};
			_ItemValue = amount * 10;
			_ChangeArmor = kitCategory switch
			{
				ChangeArmor.KitType.Cloth => new ChangeArmor(amount, ChangeArmor.KitType.Cloth),
				ChangeArmor.KitType.Leather => new ChangeArmor(amount, ChangeArmor.KitType.Leather),
				ChangeArmor.KitType.Plate => new ChangeArmor(amount, ChangeArmor.KitType.Plate),
				_ => throw new ArgumentOutOfRangeException()
			};
			_Desc = $"A single-use {_Name} that increases armor rating by {amount} for one armor item.";
		}
		public Consumable(KitLevel kitLevel, KitType kitType, ChangeWeapon.KitType kitCategory)
		{
			_KitCategory = kitType;
			_Name = $"{kitLevel.ToString().ToLower()} {kitCategory.ToString().ToLower()} {kitType.ToString().ToLower()} kit";
			_Weight = 1;
			_KitStrength = kitLevel;
			int amount = _KitStrength switch
			{
				KitLevel.Light => 1,
				KitLevel.Medium => 2,
				KitLevel.Heavy => 3,
				_ => throw new ArgumentOutOfRangeException()
			};
			_ItemValue = amount * 10;
			_ChangeWeapon = kitCategory switch
			{
				ChangeWeapon.KitType.Grindstone => new ChangeWeapon(amount, ChangeWeapon.KitType.Grindstone),
				ChangeWeapon.KitType.Bowstring => new ChangeWeapon(amount, ChangeWeapon.KitType.Bowstring),
				_ => throw new ArgumentOutOfRangeException()
			};
			_Desc = $"A single-use {_Name} that increases weapon damage by {amount} for one weapon item.";
		}
	}
}
