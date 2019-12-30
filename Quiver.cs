﻿using System;

namespace DungeonGame {
	public class Quiver : IEquipment {
		public bool Equipped { get; set; }
		public int ItemValue { get; set; }
		public int Quantity { get; set; }
		public int MaxQuantity { get; set; }
		public string Name { get; set; }

		public Quiver(string name, int quantity, int maxQuantity, int itemValue) {
			this.Name = name;
			this.Quantity = quantity;
			this.MaxQuantity = maxQuantity;
			this.ItemValue = itemValue;
		}
		
		public string GetName() {
			return this.Name;
		}
		public bool IsEquipped() {
			return this.Equipped;
		}
		public bool HaveArrows() {
			return this.Quantity > 0;
		}
		public void UseArrow() {
			this.Quantity -= 1;
		}

		public void OutOfArrows() {
			Helper.FormatAttackFailText();
			Console.WriteLine("You ran out of arrows! Going hand to hand!");
		}
	}
}