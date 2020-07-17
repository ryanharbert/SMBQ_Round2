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
	public CardData playerHero;

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
	public float heroCD;
	public HeroUnit hero;
	public int abilityBeingCast;

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

	public int cardSelected;
	public Vector3 spawnPosition;
	public bool playCard = false;
}
