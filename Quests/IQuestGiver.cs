using System.Collections.Generic;

namespace DungeonGame.Quests
{
	public interface IQuestGiver : IName
	{
		List<Quest> _AvailableQuests { get; set; }

		void OfferQuest(Player player, string[] input);
		void CompleteQuest(Player player, string[] input);
		void PopulateQuests(Player player);
		void ShowQuestList(Player player);
	}
}