using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame {
	public class Quest {
		public enum QuestType {
			KillCount,
			KillMonster,
			ClearLevel
		}
		public string Name { get; set; }
		public string Dialogue { get; set; }
		public QuestType QuestCategory { get; set; }
		public int? CurrentKills { get; set; }
		public int? RequiredKills { get; set; }
		public int? TargetLevel { get; set; }
		public int? MonstersRemaining { get; set; }
		public Monster.MonsterType? MonsterKillType { get; set; }
		public bool QuestCompleted { get; set; }
		public int QuestRewardGold { get; set; }
		public IEquipment QuestRewardItem { get; set; }

		public Quest(
			string name, string dialogue, QuestType questCategory, IEquipment questRewardItem) {
			this.Name = name;
			this.Dialogue = dialogue;
			this.QuestCategory = questCategory;
			switch (this.QuestCategory) {
				case QuestType.KillCount:
					var desiredKills = GameHandler.GetRandomNumber(20, 30);
					this.CurrentKills = 0;
					this.RequiredKills = desiredKills;
					this.QuestRewardGold = (int)this.RequiredKills * 10;
					break;
				case QuestType.KillMonster:
					var desiredMonsterKills = GameHandler.GetRandomNumber(10, 20);
					this.CurrentKills = 0;
					this.RequiredKills = desiredMonsterKills;
					this.QuestRewardGold = (int)this.RequiredKills * 10;
					var randomNum = GameHandler.GetRandomNumber(1, 8);
					this.MonsterKillType = randomNum switch {
						1 => Monster.MonsterType.Demon,
						2 => Monster.MonsterType.Dragon,
						3 => Monster.MonsterType.Elemental,
						4 => Monster.MonsterType.Skeleton,
						5 => Monster.MonsterType.Spider,
						6 => Monster.MonsterType.Troll,
						7 => Monster.MonsterType.Vampire,
						8 => Monster.MonsterType.Zombie,
						_ => this.MonsterKillType
					};
					break;
				case QuestType.ClearLevel:
					this.TargetLevel = GameHandler.GetRandomNumber(1, 10);
					this.MonstersRemaining = RoomHandler.Rooms.Where(
						room => room.Key.Z == this.TargetLevel * -1).Count(
						room => room.Value.Monster?.HitPoints > 0);
					this.QuestRewardGold = (int)this.MonstersRemaining * 10;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			this.QuestRewardItem = questRewardItem;
		}

		public void ShowQuest() {
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				this.Name);
			var questBorder = new StringBuilder();
			for (var i = 0; i < Settings.GetGameWidth(); i++) {
				questBorder.Append("=");
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				questBorder.ToString());
			for (var i = 0; i < this.Dialogue.Length; i += Settings.GetGameWidth()) {
				if (this.Dialogue.Length - i < Settings.GetGameWidth()) {
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatRoomOutputText(), 
						Settings.FormatDefaultBackground(), 
						this.Dialogue.Substring(i, this.Dialogue.Length - i));
					continue;
				}
				OutputHandler.Display.StoreUserOutput(
					Settings.FormatRoomOutputText(), 
					Settings.FormatDefaultBackground(), 
					this.Dialogue.Substring(i, Settings.GetGameWidth()));
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				questBorder.ToString());
			switch (this.QuestCategory) {
				case QuestType.KillCount:
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						"Required Kills: " + this.RequiredKills);
					break;
				case QuestType.KillMonster:
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						"Target Monster: " + this.MonsterKillType);
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						"Required Kills: " + this.RequiredKills);
					break;
				case QuestType.ClearLevel:
					OutputHandler.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						"Clear Level " + this.TargetLevel);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				questBorder.ToString());
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				"Gold Reward: ");
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				this.QuestRewardGold + " gold coins");
			OutputHandler.Display.StoreUserOutput(
				Settings.FormatSuccessOutputText(),
				Settings.FormatDefaultBackground(),
				"Item Reward: ");
			var itemName = GearHandler.GetItemDetails(this.QuestRewardItem);
			var itemInfo = new StringBuilder(itemName);
			GearHandler.StoreRainbowGearOutput(itemInfo.ToString());
		}
		public async void UpdateQuestProgress(Monster monster) {
			await Task.Run(() => {
				switch (this.QuestCategory) {
					case QuestType.KillCount:
						this.CurrentKills++;
						if (this.CurrentKills >= this.RequiredKills) this.QuestCompleted = true;
						break;
					case QuestType.KillMonster:
						if (this.MonsterKillType == monster.MonsterCategory) this.CurrentKills++;
						if (this.CurrentKills >= this.RequiredKills) this.QuestCompleted = true;
						break;
					case QuestType.ClearLevel:
						this.MonstersRemaining = RoomHandler.Rooms.Where(
							room => room.Key.Z == this.TargetLevel * -1).Count(
							room => room.Value.Monster?.HitPoints > 0);
						if (this.MonstersRemaining == 0) {
							this.QuestCompleted = true;
						}
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			});
		}
	}
}