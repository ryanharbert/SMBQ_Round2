using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class Battle : MonoBehaviourPunCallbacks
{
    public static Battle instance;

	public BattleType battleType;

	public static BattleState state;
	public GameSystem gameSystem;
	public InputSystem inputSystem;
	public EnemySystem enemySystem;
	public HeroSystem heroSystem;
	public ManaSystem manaSystem;
	public UnitSystem unitSystem;
	public ProjectileSystem projectileSystem;

	public GameObject mouseDownAnimation;
	public BattleCardGroup battleCardGroup;
	public BattleEnd battleEnd;

	public GameObject options;

    public RectTransform hoverOverRect;
    public Text hoverOverText;

	public bool setup = false;


	private void Awake()
    {
        instance = this;
        PhotonNetwork.SendRate = 10;
        PhotonNetwork.SerializationRate = 10;
        state = new BattleState();

        hoverOverRect.gameObject.SetActive(false);
    }

	public void Setup()
    {
        if(battleType == BattleType.LivePvP || battleType == BattleType.LiveRaid)
		{
			Init();
		}
        else
        {
			PhotonNetwork.OfflineMode = true;
			CreateRoom();
		}
    }

    void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = false;
        roomOptions.MaxPlayers = 1;
        PhotonNetwork.CreateRoom("Room Name", roomOptions);
    }

    public override void OnCreatedRoom()
    {
        Init();
    }

    private void Init()
    {
	    SetData();
        battleCardGroup.Set();

        gameSystem.Init(state);
        inputSystem.Init(state);
        enemySystem.Init(state);
        heroSystem.Init(state);
        manaSystem.Init(state);
        unitSystem.Init(state);
        projectileSystem.Init(state);

		if(battleType != BattleType.LivePvP && battleType != BattleType.LiveRaid)
		{
			options.SetActive(true);
		}
		else
		{
			options.SetActive(false);
		}
    }

	private void Update()
	{
        if (!setup && state.playerObjectives.Count != 0 && state.enemyObjectives.Count != 0)
            setup = true;

		if(!setup)
			return;

        gameSystem.Run(state);
		inputSystem.Run(state);
		enemySystem.Run(state);
		heroSystem.Run(state);
		manaSystem.Run(state);
		unitSystem.Run(state);
		projectileSystem.Run(state);
	}

	void SetData()
	{
        BattleData battleData = Data.instance.battle;
        if(battleType == BattleType.Raid || battleType == BattleType.LiveRaid)
        {
            battleData = Data.instance.raidBattle;
        }
        else if(battleType == BattleType.AsyncPvP)
        {
            battleData = Data.instance.pvpBattle;
        }
        //RemoveMe
        //Player
        string[] playerDeckNames;
        playerDeckNames = Shuffle(Data.instance.collection.deckData.deck);
        foreach (string n in playerDeckNames)
		{
			state.playerDeck.Add(Data.instance.collection.inventory[n]);
		}
		state.playerStronghold = Data.instance.collection.inventory[Data.instance.collection.deckData.stronghold];
		state.playerHero = Data.instance.collection.inventory[Data.instance.collection.deckData.hero];
        if(battleType == BattleType.Raid || battleType == BattleType.LiveRaid)
        {
            state.battleLayout = Resources.Load<BattleLayout>("BattleLayouts/" + battleData.battleScene + "Raid");
        }
        else
        {
            state.battleLayout = Resources.Load<BattleLayout>("BattleLayouts/" + battleData.battleScene);
        }

        if (battleType != BattleType.LivePvP)
        {
            //Enemy
            string[] enemyDeckNames;
            if (battleData.enemyName != "Mimic")
            {
                if(battleType != BattleType.AsyncPvP)
                {
                    enemyDeckNames = Shuffle(battleData.enemyDeck);
                }
                else
                {

                    for (int i = 0; i < battleData.enemyDeck.Length; i++)
                    {
                        string temp = battleData.enemyDeck[i];
                        int tempLevel = battleData.enemyLevels.deck[i];
                        int tempStar = battleData.enemyStars.deck[i];
                        int randomIndex = UnityEngine.Random.Range(0, battleData.enemyDeck.Length);
                        battleData.enemyDeck[i] = battleData.enemyDeck[randomIndex];
                        battleData.enemyDeck[randomIndex] = temp;
                        battleData.enemyLevels.deck[i] = battleData.enemyLevels.deck[randomIndex];
                        battleData.enemyLevels.deck[randomIndex] = tempLevel;
                        battleData.enemyStars.deck[i] = battleData.enemyStars.deck[randomIndex];
                        battleData.enemyStars.deck[randomIndex] = tempStar;
                    }
                    enemyDeckNames = battleData.enemyDeck;
                }
            }
            else
            {
                string[] mimicDeck = new string[12];
                int index = 0;
                for(int i = 0; i < Data.instance.collection.deckData.deck.Length; i++)
                {
                    mimicDeck[i] = Data.instance.collection.deckData.deck[i];
                    index++;
                }
                for(int i = index; i < mimicDeck.Length; i++)
                {
                    mimicDeck[i] = "Mimic";
                }
                enemyDeckNames = Shuffle(mimicDeck);
            }
            foreach (string n in enemyDeckNames)
            {
                state.enemyDeck.Add(Resources.Load<CardData>("Cards/" + n));
            }
            if(battleType == BattleType.Raid || battleType == BattleType.LiveRaid)
            {
                state.enemyStronghold = Resources.Load<Unit>("EnemyBuildings/" + battleData.enemyObjectives[0]);
                for (int i = 1; i < battleData.enemyObjectives.Length; i++)
                {
                    state.enemyBuildings.Add(Resources.Load<Unit>("EnemyBuildings/" + battleData.enemyObjectives[i]));
                }
                state.enemyLevel = battleData.level;
            }
            else if(battleType != BattleType.AsyncPvP)
            {
                int m = Mathf.Min(3, battleData.enemyObjectives.Length);
                state.enemyStronghold = Resources.Load<Unit>("EnemyBuildings/" + battleData.enemyObjectives[0]);
                for (int i = 1; i < m; i++)
                {
                    state.enemyBuildings.Add(Resources.Load<Unit>("EnemyBuildings/" + battleData.enemyObjectives[i]));
                }
                state.enemyLevel = battleData.level;
            }
            else
            {
                state.enemyStronghold = Resources.Load<CardData>("Cards/" + battleData.enemyObjectives[0]).Unit;
                state.enemyLevelData = battleData.enemyLevels;
                state.enemyStarData = battleData.enemyStars;
            }
        }
	}

	public string[] Shuffle(string[] deck)
	{
		string[] shuffledDeck = deck;

		for (int i = 0; i < shuffledDeck.Length; i++)
		{
			string temp = shuffledDeck[i];
			int randomIndex = UnityEngine.Random.Range(0, shuffledDeck.Length);
			shuffledDeck[i] = shuffledDeck[randomIndex];
			shuffledDeck[randomIndex] = temp;
		}

		return shuffledDeck;
    }

    public void Win()
	{
		battleEnd.WonBattle();
		GetLoot(battleType);
		CheckWorldQuestProgress();

		if (battleType == BattleType.AsyncPvP)
        {
            PlayFab.CloudScriptModels.EntityKey e = new PlayFab.CloudScriptModels.EntityKey() { Id = Data.instance.entityID, Type = Data.instance.entityType };
            PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new PlayFab.CloudScriptModels.ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "endPvPFightv3", GeneratePlayStreamEvent = true, FunctionParameter = new { opponent = Data.instance.pvpBattle.enemyName, win = true } }, PvPAsyncRewards, ChestSlotFilledFailure);
        }
	}

	public void Lose()
    {
        if (Data.instance.battle.enemyName != "Tutorial")
        {
            battleEnd.LostBattle();
        }
        else
        {
            Win();
        }

        if (battleType == BattleType.AsyncPvP)
        {
            PlayFab.CloudScriptModels.EntityKey e = new PlayFab.CloudScriptModels.EntityKey() { Id = Data.instance.entityID, Type = Data.instance.entityType };
            PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new PlayFab.CloudScriptModels.ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "endPvPFightv3", FunctionParameter = new { opponent = Data.instance.pvpBattle.enemyName, win = false } }, PvPAsyncRewards, ChestSlotFilledFailure);
        }

        if(battleType == BattleType.World && Data.instance.battle.enemyName != "Tutorial")
        {
            FrameRate.instance.LossBattle();
        }
    }

	void GetLoot(BattleType type)
	{
		if (battleType == BattleType.World)
		{
            if(Data.instance.battle.enemyName != "Tutorial")
            {
                EnemyData tempE;
                if (Data.instance.battle.enemyName == "Mimic" && !Data.instance.world.Enemies.TryGetValue(Data.instance.world.CurrentPlayerNode, out tempE))
			    {
				    Data.instance.world.worldChests.treasures[Data.instance.world.CurrentPlayerNode] = false;
                    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "collectWorldLoot", FunctionParameter = new { enemy = Data.instance.battle.enemyName, collect = true } }, LootReceived, OnLootFailure);
                }
                else
                {
                    if (Data.instance.world.Enemies.ContainsKey(Data.instance.world.CurrentPlayerNode))
                    {
                        Data.instance.world.Enemies.Remove(Data.instance.world.CurrentPlayerNode);
                    }
                    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "getLootTable", GeneratePlayStreamEvent = true, FunctionParameter = new { enemy = Data.instance.battle.enemyName } }, GetPossibleLoot, OnLootFailure);
                }
            }
            else
            {
                Data.instance.tutorial.steps["BattleComplete"] = true;
                PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "tutorialBattleWinv2", GeneratePlayStreamEvent = true, FunctionParameter = new { enemy = "Tutorial", FrameRate = FrameRate.instance.avgFrameRate } }, LootReceived, OnLootFailure);
            }
        }
		else if (battleType == BattleType.Raid || battleType == BattleType.LiveRaid)
        {
            PlayFab.CloudScriptModels.EntityKey e = new PlayFab.CloudScriptModels.EntityKey() { Id = Data.instance.entityID, Type = Data.instance.entityType };
            PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new PlayFab.CloudScriptModels.ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "fillChestSlotv4", GeneratePlayStreamEvent = true, FunctionParameter = new { level = Data.instance.raidBattle.level, raidName = Data.instance.raidBattle.enemyName } }, ChestSlotFilledSuccess, ChestSlotFilledFailure);
		}
    }

    public void CollectLootWorld(bool collect)
	{
		PlayFab.CloudScriptModels.EntityKey e = new PlayFab.CloudScriptModels.EntityKey() { Id = Data.instance.entityID, Type = Data.instance.entityType };
		if (collect)
        {
            PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new PlayFab.CloudScriptModels.ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "worldBattleLootv2", GeneratePlayStreamEvent = true, FunctionParameter = new { enemy = Data.instance.battle.enemyName, collect = collect } }, LootBagReceived, OnLootFailure);
        }
        else
        {
			PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new PlayFab.CloudScriptModels.ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "worldBattleLootv2", GeneratePlayStreamEvent = true, FunctionParameter = new { enemy = Data.instance.battle.enemyName, collect = collect } }, IgnoreLeaveBattle, OnLootFailure);
        }
    }

    public void IgnoreLeaveBattle(PlayFab.CloudScriptModels.ExecuteCloudScriptResult result)
    {
        battleEnd.LeaveBattleButton();
    }

    void CheckWorldQuestProgress()
    {
        foreach(QuestData q in Data.instance.quests)
        {
			if (battleType == BattleType.World)
			{
				if ((q.Type == "WorldTarget" && q.Target == Data.instance.battle.enemyName) || q.Type == "WorldAny")
				{
					q.Progress++;
				}
			}
			else if (battleType == BattleType.Raid || battleType == BattleType.LiveRaid)
			{
				if (q.Type == "RaidAny")
				{
					q.Progress++;
				}
			}
			else if (battleType == BattleType.AsyncPvP)
			{
				if (q.Type == "AsyncPvP")
				{
					q.Progress++;
				}
			}
		}
	}

	private void GetPossibleLoot(ExecuteCloudScriptResult result)
    {
        if (result.FunctionResult != null)
        {
            string[] cards = PlayFabSimpleJson.DeserializeObject<string[]>(result.FunctionResult.ToString());

            battleEnd.WorldBattle(cards);
        }
        else
        {
            Debug.LogError("Enemy drop pool does NOT exist.");
        }
	}

	private void LootReceived(ExecuteCloudScriptResult result)
	{
		if (result.FunctionResult != null)
		{
			JsonObject jsonResult = (JsonObject)result.FunctionResult;
			object cardObject;
			object amountObject;
			object goldObject;
			object expObject;
			object levelObject;
			jsonResult.TryGetValue("cardAdded", out cardObject);
			jsonResult.TryGetValue("amountOwned", out amountObject);
			jsonResult.TryGetValue("goldAdded", out goldObject);
			jsonResult.TryGetValue("expAdded", out expObject);
			jsonResult.TryGetValue("playerLevel", out levelObject);

			string cardAdded = (string)cardObject;

			int amountAdded = 0;
			int amountOwned = Convert.ToInt32(amountObject);
			int goldAdded = Convert.ToInt32(goldObject);
			int expAdded = Convert.ToInt32(expObject);
			int playerLevel = Convert.ToInt32(levelObject);

			bool newItem = false;
			CardData card;
			if (Data.instance.collection.inventory.TryGetValue(cardAdded, out card))
			{
				amountAdded = amountOwned - 1 - card.amountOwned;
			}
			else if (Data.instance.collection.allCards.TryGetValue(cardAdded, out card))
			{
				amountAdded = amountOwned;
				newItem = true;
			}

			Data.instance.collection.AddCards(cardAdded, amountAdded);

			Data.instance.currency.gold += goldAdded;
			Data.instance.currency.IncreaseExp(expAdded, playerLevel);

			battleEnd.LootReceived(card, amountAdded, goldAdded, expAdded, newItem);
		}
		else
		{
			Debug.LogError("Enemy drop pool does NOT exist.");
		}
	}

	private void LootBagReceived(PlayFab.CloudScriptModels.ExecuteCloudScriptResult result)
	{
		if (result.FunctionResult != null)
		{
			JsonObject jsonResult = (JsonObject)result.FunctionResult;
			object cardObject;
			object amountObject;
			object goldObject;
			object expObject;
			object levelObject;
			jsonResult.TryGetValue("cardAdded", out cardObject);
			jsonResult.TryGetValue("amountOwned", out amountObject);
			jsonResult.TryGetValue("goldAdded", out goldObject);
			jsonResult.TryGetValue("expAdded", out expObject);
			jsonResult.TryGetValue("playerLevel", out levelObject);

			string cardAdded = (string)cardObject;

			int amountAdded = 0;
			int amountOwned = Convert.ToInt32(amountObject);
			int goldAdded = Convert.ToInt32(goldObject);
			int expAdded = Convert.ToInt32(expObject);
			int playerLevel = Convert.ToInt32(levelObject);

            bool newItem = false;
			CardData card;
			if (Data.instance.collection.inventory.TryGetValue(cardAdded, out card))
			{
				amountAdded = amountOwned - 1 - card.amountOwned;
			}
			else if(Data.instance.collection.allCards.TryGetValue(cardAdded, out card))
			{
                amountAdded = amountOwned;
                newItem = true;
			}

            Data.instance.collection.AddCards(cardAdded, amountAdded);

			Data.instance.currency.gold += goldAdded;
			Data.instance.currency.IncreaseExp(expAdded, playerLevel);

			battleEnd.LootReceived(card, amountAdded, goldAdded, expAdded, newItem);
		}
		else
		{
			Debug.LogError("Enemy drop pool does NOT exist.");
		}
	}

	private void OnLootFailure(PlayFabError error)
	{
		Debug.LogError("Here's some debug information:");
		Debug.LogError(error.GenerateErrorReport());
	}

	private void ChestSlotFilledSuccess(PlayFab.CloudScriptModels.ExecuteCloudScriptResult result)
	{
		if (result.FunctionResult != null)
		{
			JsonObject jsonResult = (JsonObject)result.FunctionResult;
			object slotObject;
			object indexObject;
            jsonResult.TryGetValue("slotData", out slotObject);
			jsonResult.TryGetValue("slotIndex", out indexObject);

            ChestSlotData c = PlayFabSimpleJson.DeserializeObject<ChestSlotData>((string)slotObject);
			int index = PlayFabSimpleJson.DeserializeObject<int>((string)indexObject);

            if (index != -1)
			{
				if(battleType == BattleType.Raid || battleType == BattleType.LiveRaid)
				{
					Data.instance.raids.chestSlots[index] = c;
					Data.instance.raids.SetChestSlot(Data.instance.raids.chestSlots[index]);
					battleEnd.ChestReceived(battleType);
				}
			}
			else
			{
				battleEnd.ChestSlotsFull(battleType);
			}
        }
    }

    private void PvPAsyncRewards(PlayFab.CloudScriptModels.ExecuteCloudScriptResult result)
    {
        if (result.FunctionResult != null)
        {
            JsonObject jsonResult = (JsonObject)result.FunctionResult;
            object pointsObject;
            object goldObject;
            object rankObject;
            jsonResult.TryGetValue("points", out pointsObject);
            jsonResult.TryGetValue("gold", out goldObject);
            jsonResult.TryGetValue("rank", out rankObject);

            int points = PlayFabSimpleJson.DeserializeObject<int>((string)pointsObject);
            int gold = PlayFabSimpleJson.DeserializeObject<int>((string)goldObject);
            int rank = PlayFabSimpleJson.DeserializeObject<int>((string)rankObject);

            bool win = true;

            if(points > 5)
            {
                win = true;
            }
            else
            {
                win = false;
            }

            Data.instance.currency.asyncPoints += points;
            Data.instance.currency.gold += gold;

            battleEnd.AsyncPvp(win, points, gold, rank);
        }
    }

    private void ChestSlotFilledFailure(PlayFabError error)
	{
		Debug.LogError("Here's some debug information:");
		Debug.LogError(error.GenerateErrorReport());
    }

    public void TutorialFinished()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "tutorialEnergyFinishedv2", FunctionParameter = new { FrameRate = FrameRate.instance.avgFrameRate } }, TutorialStepFinished, GetDataFailure);
    }

    public void TutorialStepFinished(ExecuteCloudScriptResult result)
    {

    }

    public void GetDataFailure(PlayFabError error)
    {
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
}
