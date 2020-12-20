﻿using DungeonGame.Controllers;
using DungeonGame.Items;
using DungeonGame.Monsters;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Quests {
	public class Quest {
		public enum QuestType {
			KillCount,
			KillMonster,
			ClearLevel
		}
		public string _Name { get; set; }
		public string _Dialogue { get; set; }
		public QuestType _QuestCategory { get; set; }
		public int? _CurrentKills { get; set; }
		public int? _RequiredKills { get; set; }
		public int? _TargetLevel { get; set; }
		public int? _MonstersRemaining { get; set; }
		public Monster.MonsterType? _MonsterKillType { get; set; }
		public bool _QuestCompleted { get; set; }
		public int _QuestRewardGold { get; set; }
		public IItem _QuestRewardItem { get; set; }
		public string _QuestGiver { get; set; }

		public Quest(
			string name, string dialogue, QuestType questCategory, IItem questRewardItem, string questGiver) {
			_Name = name;
			_Dialogue = dialogue;
			_QuestCategory = questCategory;
			_QuestGiver = questGiver;
			switch (_QuestCategory) {
				case QuestType.KillCount:
					int desiredKills = GameController.GetRandomNumber(20, 30);
					_CurrentKills = 0;
					_RequiredKills = desiredKills;
					_QuestRewardGold = (int)_RequiredKills * 10;
					break;
				case QuestType.KillMonster:
					int desiredMonsterKills = GameController.GetRandomNumber(10, 20);
					_CurrentKills = 0;
					_RequiredKills = desiredMonsterKills;
					_QuestRewardGold = (int)_RequiredKills * 10;
					int randomNum = GameController.GetRandomNumber(1, 8);
					_MonsterKillType = randomNum switch {
						1 => Monster.MonsterType.Demon,
						2 => Monster.MonsterType.Dragon,
						3 => Monster.MonsterType.Elemental,
						4 => Monster.MonsterType.Skeleton,
						5 => Monster.MonsterType.Spider,
						6 => Monster.MonsterType.Troll,
						7 => Monster.MonsterType.Vampire,
						8 => Monster.MonsterType.Zombie,
						_ => _MonsterKillType
					};
					break;
				case QuestType.ClearLevel:
					_TargetLevel = GameController.GetRandomNumber(1, 10);
					_MonstersRemaining = RoomController._Rooms.Where(
						room => room.Key._Z == _TargetLevel * -1).Count(
						room => room.Value._Monster?._HitPoints > 0);
					_QuestRewardGold = (int)_MonstersRemaining * 10;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			_QuestRewardItem = questRewardItem;
		}

		public void ShowQuest() {
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				_Name);
			StringBuilder questBorder = new StringBuilder();
			for (int i = 0; i < Settings.GetGameWidth(); i++) {
				questBorder.Append("=");
			}
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				questBorder.ToString());
			for (int i = 0; i < _Dialogue.Length; i += Settings.GetGameWidth()) {
				if (_Dialogue.Length - i < Settings.GetGameWidth()) {
					OutputController.Display.StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						_Dialogue.Substring(i, _Dialogue.Length - i));
					continue;
				}
				OutputController.Display.StoreUserOutput(
					Settings.FormatGeneralInfoText(),
					Settings.FormatDefaultBackground(),
					_Dialogue.Substring(i, Settings.GetGameWidth()));
			}
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				questBorder.ToString());
			switch (_QuestCategory) {
				case QuestType.KillCount:
					OutputController.Display.StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						$"Required Kills: {_RequiredKills}");
					break;
				case QuestType.KillMonster:
					OutputController.Display.StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						$"Target _Monster: {_MonsterKillType}");
					OutputController.Display.StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						$"Required Kills: {_RequiredKills}");
					break;
				case QuestType.ClearLevel:
					OutputController.Display.StoreUserOutput(
						Settings.FormatGeneralInfoText(),
						Settings.FormatDefaultBackground(),
						$"Clear _Level {_TargetLevel}");
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				questBorder.ToString());
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				"Rewards: ");
			GearController.StoreRainbowGearOutput(GearController.GetItemDetails(_QuestRewardItem));
			OutputController.Display.StoreUserOutput(
				Settings.FormatGeneralInfoText(),
				Settings.FormatDefaultBackground(),
				$"{_QuestRewardGold} gold coins");
		}
		public async void UpdateQuestProgress(Monster monster) {
			await Task.Run(() => {
				switch (_QuestCategory) {
					case QuestType.KillCount:
						_CurrentKills++;
						if (_CurrentKills >= _RequiredKills) {
							_QuestCompleted = true;
						}

						break;
					case QuestType.KillMonster:
						if (_MonsterKillType == monster._MonsterCategory) {
							_CurrentKills++;
						}

						if (_CurrentKills >= _RequiredKills) {
							_QuestCompleted = true;
						}

						break;
					case QuestType.ClearLevel:
						_MonstersRemaining = RoomController._Rooms.Where(
							room => room.Key._Z == _TargetLevel * -1).Count(
							room => room.Value._Monster?._HitPoints > 0);
						if (_MonstersRemaining == 0) {
							_QuestCompleted = true;
						}
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if (!_QuestCompleted) {
					return;
				}

				string questSuccess = $"You have completed the quest {_Name}! Go turn it in to {_QuestGiver} and get your reward.";
				for (int i = 0; i < questSuccess.Length; i += Settings.GetGameWidth()) {
					if (questSuccess.Length - i < Settings.GetGameWidth()) {
						OutputController.Display.StoreUserOutput(
							Settings.FormatSuccessOutputText(),
							Settings.FormatDefaultBackground(),
							questSuccess.Substring(i, questSuccess.Length - i));
						continue;
					}
					OutputController.Display.StoreUserOutput(
						Settings.FormatSuccessOutputText(),
						Settings.FormatDefaultBackground(),
						questSuccess.Substring(i, Settings.GetGameWidth()));
				}
			});
		}
	}
}