namespace SMBQ.Battle
{
	public struct BattleData
	{
		//OLD
		public string enemyName;
		public string enemyHero;
		public string[] enemyObjectives;
		public string[] enemyDeck;
		public int level;
		public DeckLevelsData enemyLevels;
		public DeckStarsData enemyStars;
    
		//NEW
		public string battleScene;
		public PlayerData self;
		public PlayerData[] allies;
		public PlayerData[] enemies;


	}

	public struct PlayerData
	{
		public bool ai;
		
		public CardData objective;
		public CardData[] subObjectives;
		
		public CardData[] deck;
		public CardData hero;
		public CardData stronghold;
	}

	public struct CardData
	{
		public string name;
		public int level;
		public int stars;
	}
}
