namespace DungeonGame.Effects {
	public interface IChangeDamageEffect {
		int GetChangedDamageFromEffect(int incomingDamage);
		void ProcessChangeDamageRound(int incomingDamageAmount);
	}
}
