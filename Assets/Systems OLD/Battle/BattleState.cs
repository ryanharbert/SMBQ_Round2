using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleState
{
	public float gameTimer;
	public bool gameOver;
	public float gameLength;

	public List<CardData> playerDeck = new List<CardData>();
	public CardData playerStronghold;
	public CardData[] playerHero = new CardData[2];

    public List<CardData> enemyDeck = new List<CardData>();
	public Unit enemyStronghold;
	public List<Unit> enemyBuildings = new List<Unit>();
	public int enemyLevel;
    public DeckLevelsData enemyLevelData;
    public DeckStarsData enemyStarData;
    public BattleLayout battleLayout;
	public float enemyHeroCD;
	public HeroUnit enemyHero;
	public CardData enemyHeroCard;

	public List<Unit> units = new List<Unit>();
	public List<Projectile> projectiles = new List<Projectile>();
	public List<Unit> playerObjectives = new List<Unit>();
	public List<Unit> enemyObjectives = new List<Unit>();
	public Unit yourObjective;

	public int enemyDeckIndex;
	public float enemyManaProgress;
	public int enemyCurrentMana;

	public int teamID;

	public bool autoPlay;

	//Hero
	public float[] heroCD = new float[2];
    public HeroUnit[] hero = new HeroUnit[2];

	//Mana
	public int currentMana;
	public float manaProgress;
	public int maxMana;
	public float manaRate;
    public float enemyManaRate;

	//Input Objects
	public Transform unitRotation;
    public SpawnArea playerSpawnArea;
    public SpawnArea enemySpawnArea;
    public SpawnArea anywhereSpawnArea;
	public GameObject mouseDownAnimation;
	public Collider spawnCollider;

	public RaycastHit hit;
	public Ray ray;

    public BattleCard selectedCard;
	public Vector3 spawnPosition;
	public bool playCard = false;

    public static BattleState Test(List<string> deck, string stronghold, List<string> heroes, List<string> enemyDeck, string enemyStronghold)
    {
        BattleState test = new BattleState();

        test.playerDeck = new List<CardData>();
        for(int i = 0; i < deck.Count; i++)
        {
            test.playerDeck.Add(Resources.Load<CardData>("Cards/" + deck[i]));
        }
        test.playerStronghold = Resources.Load<CardData>("Cards/" + stronghold);
        test.playerHero = new CardData[2];
        test.playerHero[0] = Resources.Load<CardData>("Cards/" + heroes[0]);
        test.playerHero[1] = Resources.Load<CardData>("Cards/" + heroes[1]);
        test.battleLayout = Resources.Load<BattleLayout>("BattleLayouts/" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        test.enemyDeck = new List<CardData>();
        for (int i = 0; i < enemyDeck.Count; i++)
        {
            test.enemyDeck.Add(Resources.Load<CardData>("Cards/" + enemyDeck[i]));
        }
        test.enemyStronghold = Resources.Load<Unit>("EnemyBuildings/" + enemyStronghold);
        test.enemyLevel = 1;

        return test;
    }
    public static BattleState Test()
    {
        List<string> deck = new List<string>();
        for (int i = 0; i < 6; i++)
        {
            deck.Add("Guards");
        }

        List<string> heroes = new List<string>();
        heroes[0] = "QueenWorm";
        heroes[1] = "MightyMage";

        return Test(deck, "TreeFort", heroes, deck, "TreeFortBld");
    }
}