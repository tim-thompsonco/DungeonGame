using DungeonGame.Helpers;
using DungeonGame.Items;
using DungeonGame.Monsters;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Quests {
	public class Quest {
		public string Name { get; set; }
		public string Dialogue { get; set; }
		public QuestType QuestCategory { get; set; }
		public int? CurrentKills { get; set; }
		public int? RequiredKills { get; set; }
		public int? TargetLevel { get; set; }
		public int? MonstersRemaining { get; set; }
		public MonsterType? MonsterKillType { get; set; }
		public bool QuestCompleted { get; set; }
		public int QuestRewardGold { get; set; }
		public IItem QuestRewardItem { get; set; }
		public string QuestGiver { get; set; }

		public Quest(
			string name, string dialogue, QuestType questCategory, IItem questRewardItem, string questGiver) {
			Name = name;
			Dialogue = dialogue;
			QuestCategory = questCategory;
			QuestGiver = questGiver;
			switch (QuestCategory) {
				case QuestType.KillCount:
					int desiredKills = GameHelper.GetRandomNumber(20, 30);
					CurrentKills = 0;
					RequiredKills = desiredKills;
					QuestRewardGold = (int)RequiredKills * 10;
					break;
				case QuestType.KillMonster:
					int desiredMonsterKills = GameHelper.GetRandomNumber(10, 20);
					CurrentKills = 0;
					RequiredKills = desiredMonsterKills;
					QuestRewardGold = (int)RequiredKills * 10;
					int randomNum = GameHelper.GetRandomNumber(1, 8);
					MonsterKillType = randomNum switch {
						1 => MonsterType.Demon,
						2 => MonsterType.Dragon,
						3 => MonsterType.Elemental,
						4 => MonsterType.Skeleton,
						5 => MonsterType.Spider,
						6 => MonsterType.Troll,
						7 => MonsterType.Vampire,
						8 => MonsterType.Zombie,
						_ => MonsterKillType
					};
					break;
				case QuestType.ClearLevel:
					TargetLevel = GameHelper.GetRandomNumber(1, 10);
					MonstersRemaining = RoomHelper.Rooms.Where(
						room => room.Key.Z == TargetLevel * -1).Count(
						room => room.Value.Monster?.HitPoints > 0);
					QuestRewardGold = (int)MonstersRemaining * 10;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			QuestRewardItem = questRewardItem;
		}

		public void ShowQuest() {
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				Name);
			StringBuilder questBorder = new StringBuilder();
			for (int i = 0; i < Settings.GetGameWidth(); i++) {
				questBorder.Append("=");
			}
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				questBorder.ToString());
			for (int i = 0; i < Dialogue.Length; i += Settings.GetGameWidth()) {
				if (Dialogue.Length - i < Settings.GetGameWidth()) {
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						Dialogue.Substring(i, Dialogue.Length - i));
					continue;
				}
				OutputHelper.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					Dialogue.Substring(i, Settings.GetGameWidth()));
			}
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				questBorder.ToString());
			switch (QuestCategory) {
				case QuestType.KillCount:
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						$"Required Kills: {RequiredKills}");
					break;
				case QuestType.KillMonster:
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						$"Target _Monster: {MonsterKillType}");
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						$"Required Kills: {RequiredKills}");
					break;
				case QuestType.ClearLevel:
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						$"Clear _Level {TargetLevel}");
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				questBorder.ToString());
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"Rewards: ");
			GearHelper.StoreRainbowGearOutput(GearHelper.GetItemDetails(QuestRewardItem));
			OutputHelper.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				$"{QuestRewardGold} gold coins");
		}
		public async void UpdateQuestProgress(Monster monster) {
			await Task.Run(() => {
				switch (QuestCategory) {
					case QuestType.KillCount:
						CurrentKills++;
						if (CurrentKills >= RequiredKills) {
							QuestCompleted = true;
						}

						break;
					case QuestType.KillMonster:
						if (MonsterKillType == monster.MonsterCategory) {
							CurrentKills++;
						}

						if (CurrentKills >= RequiredKills) {
							QuestCompleted = true;
						}

						break;
					case QuestType.ClearLevel:
						MonstersRemaining = RoomHelper.Rooms.Where(
							room => room.Key.Z == TargetLevel * -1).Count(
							room => room.Value.Monster?.HitPoints > 0);
						if (MonstersRemaining == 0) {
							QuestCompleted = true;
						}
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if (!QuestCompleted) {
					return;
				}

				string questSuccess = $"You have completed the quest {Name}! Go turn it in to {QuestGiver} and get your reward.";
				for (int i = 0; i < questSuccess.Length; i += Settings.GetGameWidth()) {
					if (questSuccess.Length - i < Settings.GetGameWidth()) {
						OutputHelper.Display.StoreUserOutput(
							Settings.FormatSuccessOutputText(),
							Settings.FormatDefaultBackground(),
							questSuccess.Substring(i, questSuccess.Length - i));
						continue;
					}
					OutputHelper.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						questSuccess.Substring(i, Settings.GetGameWidth()));
				}
			});
		}
	}
}