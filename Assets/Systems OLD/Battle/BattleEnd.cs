using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleEnd : MonoBehaviour
{
	public GameObject battleEndObject;

	public GameObject victory;
	public GameObject defeat;
	public GameObject loading;
	public GameObject rewardsGroup;
    public GameObject useLootBag;
    public GameObject raidReward;
	public CardDisplay cardDisplay;
	public Button okayButton;


    public GameObject pvpReward;
    public GameObject pvpChestReward;
    public GameObject pvpChest;
    public GameObject pvpChestSlotsFull;
    public GameObject asyncPointsObject;
    public GameObject livePointsObject;
    public GameObject pvpGoldObject;
    public Text pvpGoldAdded;
    public Text pvpPointsAdded;

    public Text noLootFoundText;
	public Text amountAddedText;
	public Text goldAddedText;
    public Text expAddedText;
	public Text raidChestText;

    //EVENT REWARD
    public Text rankText;
    public GameObject tierRewardDisplay;
    public Text tierRewardQuantityText;
	public Image tierRewardCurrencyImg;
    public Text pointsForNextReward;
    public Text currencyAmountText;
    EventRewardDisplayType pointsDisplayType;
    float increasingNumber;
    int arenaPointIncrease;
    int rank;

    public Text possibleLootText;
    public CardDisplay[] possibleCards;
    public Text loadingText;
    public Text bagsLeftText;
    public Text bagTimerText;
    public GameObject tutorialBagObject;

    public GameObject newItemObject;

	public void WonBattle()
	{
		battleEndObject.SetActive(true);

		victory.SetActive(true);
		defeat.SetActive(false);

		loading.SetActive(true);
		rewardsGroup.SetActive(false);
		okayButton.gameObject.SetActive(false);

        if(Battle.instance.battleType == BattleType.LivePvP)
        {
            loading.SetActive(false);
            okayButton.gameObject.SetActive(true);
        }
	}

    public void WorldBattle(string[] cards)
    {
        if(cards.Length > 1)
        {
            possibleLootText.text = "Possible Loot:";
        }
        else
        {
            possibleLootText.text = "Loot:";
        }

        for(int i = 0; i < possibleCards.Length; i++)
        {
            if(cards.Length > i)
            {
                possibleCards[i].gameObject.SetActive(true);
                possibleCards[i].SetCardDisplay(cards[i]);
            }
            else
            {
                possibleCards[i].gameObject.SetActive(false);
            }
        }


        loading.SetActive(false);
        useLootBag.SetActive(true);
        bagsLeftText.text = "You have " + Data.instance.currency.energy + " Loot Bags left";

		if (!Data.instance.tutorial.steps["Energy"])
        {
            tutorialBagObject.SetActive(true);
            Data.instance.tutorial.steps["Energy"] = true;
            Battle.instance.TutorialFinished();
        }
    }

    private void Update()
    {
        if (!useLootBag.activeSelf)
        {
            return;
        }

        if(Data.instance.currency.energy < Data.instance.currency.energyMax)
        {
            string secondsToRechargeText;
            int minutes = Mathf.FloorToInt(Data.instance.currency.secondsToRecharge / 60);
            int seconds = Mathf.CeilToInt(Data.instance.currency.secondsToRecharge % 60);

            if (seconds == 60)
            {
                seconds = 0;
                minutes += 1;
            }

            if (Data.instance.currency.secondsToRecharge > 59)
            {
                secondsToRechargeText = minutes + "m " + seconds + "s";
            }
            else
            {
                secondsToRechargeText = seconds + "s";
            }
            bagTimerText.text = "Next loot bag in " + secondsToRechargeText;
        }
        else
        {
            bagTimerText.text = "New Loot Bags every 30 min when below " + Data.instance.currency.energyMax;
        }
    }

    public void CollectLoot()
    {
        if(Data.instance.currency.energy > 0)
        {
            Battle.instance.CollectLootWorld(true);
            loading.SetActive(true);
            useLootBag.SetActive(false);
            Data.instance.currency.energy -= 1;
        }
        else
        {
            Warning.instance.Activate("You don't have any loot bags to stash your loot");
        }
    }

    public void IgnoreLoot()
    {
        Battle.instance.CollectLootWorld(false);
        loading.SetActive(true);
        useLootBag.SetActive(false);
        loadingText.text = "Loading...";
    }

	public void LootReceived(CardData cardData, int amountAdded, int goldAdded, int expAdded, bool newItem)
	{
		loading.SetActive(false);
		rewardsGroup.SetActive(true);
		cardDisplay.SetCardDisplay(cardData);
		okayButton.gameObject.SetActive(true);
		amountAddedText.text = "+ " + amountAdded;
        goldAddedText.text = "+ " + goldAdded;
        expAddedText.text = "+ " + expAdded;

        if(newItem)
        {
            newItemObject.SetActive(true);
        }
	}


    //ARENA
    public void AsyncPvp(bool win, int points, int gold, int rank)
    {
        loading.SetActive(false);
        noLootFoundText.gameObject.SetActive(false);

        pvpReward.SetActive(true);

        pvpPointsAdded.text = "+" + points;
        pvpGoldAdded.text = "+" + gold;

        arenaPointIncrease = points;
        this.rank = rank;

        StartCoroutine(ArenaEventRewardDisplay());


        rewardsGroup.SetActive(false);
        okayButton.gameObject.SetActive(true);
    }

    //RAID POINTS

    IEnumerator ArenaEventRewardDisplay()
    {
        RaidPointsSetup();
        do
        {
            RaidPointIncreaseDisplay(Data.instance.currency.asyncPoints, arenaPointIncrease);
            yield return null;
        } while (Data.instance.currency.asyncPoints > increasingNumber);
    }

    void RaidPointsSetup()
    {
        increasingNumber = Data.instance.currency.asyncPoints - arenaPointIncrease;
        if (Data.instance.pvp.CurrentPvPEventTier(Mathf.RoundToInt(increasingNumber)) == -1)
        {
            rankText.gameObject.SetActive(true);
            rankText.text = "Rank #" + rank;
            pointsDisplayType = EventRewardDisplayType.TiersCompleted;
        }
        else if (Data.instance.pvp.CurrentPvPEventTier(Mathf.RoundToInt(increasingNumber)) == Data.instance.pvp.CurrentPvPEventTier(Data.instance.currency.asyncPoints))
        {
            rankText.gameObject.SetActive(false);
            pointsForNextReward.gameObject.SetActive(true);
            pointsForNextReward.text = "Earn " + (Data.instance.pvpEvent.Point[Data.instance.pvp.CurrentPvPEventTier(Data.instance.currency.asyncPoints)].Req - Data.instance.currency.asyncPoints) + " more trophies for next reward";
            pointsDisplayType = EventRewardDisplayType.SameTier;
        }
        else
        {
            rankText.gameObject.SetActive(false);
            pointsDisplayType = EventRewardDisplayType.TierIncrease;
            tierRewardDisplay.gameObject.SetActive(true);
            tierRewardQuantityText.text = "+" + Data.instance.pvpEvent.Point[Data.instance.pvp.CurrentPvPEventTier(Data.instance.currency.asyncPoints - arenaPointIncrease)].Amt;
			if(Data.instance.pvpEvent.Point[Data.instance.pvp.CurrentPvPEventTier(Data.instance.currency.asyncPoints - arenaPointIncrease)].Rew == "GO")
			{
				Data.instance.currency.gold += Data.instance.pvpEvent.Point[Data.instance.pvp.CurrentPvPEventTier(Data.instance.currency.asyncPoints - arenaPointIncrease)].Amt;
				tierRewardCurrencyImg.sprite = Resources.Load<Sprite>("CurrencyOffers/GoldPile");
			}
			else
			{
				Data.instance.currency.stars += Data.instance.pvpEvent.Point[Data.instance.pvp.CurrentPvPEventTier(Data.instance.currency.asyncPoints - arenaPointIncrease)].Amt;
				tierRewardCurrencyImg.sprite = Resources.Load<Sprite>("CurrencyOffers/Star");
			}
        }
        
        currencyAmountText.text = increasingNumber.ToString();
    }


    void RaidPointIncreaseDisplay(int target, int increaseAmount)
    {
        float rate = increaseAmount / 1f;
        increasingNumber = Mathf.MoveTowards(increasingNumber, target, Time.deltaTime * rate);
        if (pointsDisplayType == EventRewardDisplayType.TiersCompleted)
        {
            currencyAmountText.text = Mathf.Floor(increasingNumber).ToString();
        }
        else if (pointsDisplayType == EventRewardDisplayType.SameTier)
        {
            currencyAmountText.text = Mathf.Floor(increasingNumber) + " / " + Data.instance.pvpEvent.Point[Data.instance.pvp.CurrentPvPEventTier(Mathf.RoundToInt(increasingNumber))].Req;
        }
        else
        {
            if (Mathf.Floor(increasingNumber) >= Data.instance.pvpEvent.Point[Data.instance.pvp.CurrentPvPEventTier(Data.instance.currency.asyncPoints - arenaPointIncrease)].Req)
            {
                if (Data.instance.pvp.CurrentPvPEventTier(Data.instance.currency.asyncPoints) == -1)
                {
                    pointsDisplayType = EventRewardDisplayType.TiersCompleted;
                }
                else
                {
                    pointsDisplayType = EventRewardDisplayType.SameTier;
                }
            }
            else
            {
                currencyAmountText.text = Mathf.Floor(increasingNumber) + " / " + Data.instance.pvpEvent.Point[Data.instance.pvp.CurrentPvPEventTier(Mathf.RoundToInt(increasingNumber))].Req;
            }
        }
    }
    //RAID POINTS
    //ARENA


    public void ChestReceived(BattleType battleType)
	{
		loading.SetActive(false);
		if(battleType == BattleType.LivePvP)
		{
			pvpReward.SetActive(true);
            pvpChest.SetActive(true);
            pvpChestSlotsFull.SetActive(false);
            asyncPointsObject.SetActive(false);
            pvpGoldObject.SetActive(false);
            pvpPointsAdded.text = "+" + 10;
        }
		else
		{
			raidReward.SetActive(true);
			raidChestText.text = Data.instance.raidBattle.enemyName + " Chest";
		}

		rewardsGroup.SetActive(false);
		okayButton.gameObject.SetActive(true);
	}

	public void ChestSlotsFull(BattleType battleType)
	{
		loading.SetActive(false);
		if(battleType == BattleType.LivePvP)
        {
            pvpReward.SetActive(true);
            pvpChest.SetActive(false);
            pvpChestSlotsFull.SetActive(true);
            asyncPointsObject.SetActive(false);
            pvpGoldObject.SetActive(false);
            pvpPointsAdded.text = "+" + 10;
        }
		else
        {
            noLootFoundText.gameObject.SetActive(true);
            noLootFoundText.text = "All Raid Reward Slots are Full";
		}

		rewardsGroup.SetActive(false);
		okayButton.gameObject.SetActive(true);
	}

	public void LostBattle()
	{
		battleEndObject.SetActive(true);

		victory.SetActive(false);
		defeat.SetActive(true);
		
		noLootFoundText.gameObject.SetActive(true);
		rewardsGroup.SetActive(false);
		okayButton.gameObject.SetActive(true);

        if (Battle.instance.battleType == BattleType.LivePvP)
        {
            noLootFoundText.gameObject.SetActive(false);
        }
    }

	public void LeaveBattleButton()
    {
        okayButton.gameObject.SetActive(false);
        StartCoroutine("LeaveBattle");
    }

    IEnumerator LeaveBattle()
    {
        if (Photon.Pun.PhotonNetwork.InRoom)
        {
            PunManager.instance.LeaveRoom();
        }

        while(Photon.Pun.PhotonNetwork.IsConnected || Photon.Pun.PhotonNetwork.InRoom)
        {
            yield return null;
        }

        if (Data.instance.battle.enemyName != "Tutorial")
        {
            SceneLoader.ChangeScenes("WorldMap");
        }
        else
        {
            SceneLoader.ChangeScenes("Tutorial");
        }
    }



}
