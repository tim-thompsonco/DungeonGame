using System.Collections.Generic;

namespace DungeonGame {
	public interface IQuestGiver {
		List<Quest> AvailableQuests { get; set; }

		void OfferQuest(Player player, string[] input);
		void CompleteQuest(Player player, string[] input);
		void ShowQuestList();
	}
}