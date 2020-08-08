using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestLootDisplay : MonoBehaviour
{
    public static ChestLootDisplay instance;

    //RAIDPOINTS DISPLAY
    public Text pointsForNextReward;
    public CardDisplay tierRewardDisplay;
    public Text tierRewardQuantityText;
    public GameObject summaryEventReward;

    EventRewardDisplayType pointsDisplayType;

    //PER ITEM DISPLAY
    public GameObject perItemDisplay;
    public GameObject newItem;
	public Text lootNameText;
	public CardDisplay cardDisplay;
	public Text lootQuantityText;
    public GameObject currencyAmountObject;
    public Text currencyLabelText;
    public Text currencyAmountText;
    public GameObject gemIcon;
    public GameObject goldIcon;
    public GameObject pointsIcon;
    public GameObject starsIcon;
    public GameObject cardProgressObject;
	public Text stacksLeftText;

    float increasingNumber;
    int stacksLeftInChest = -1;

    //SUMMARY DISPLAY
    public GameObject summaryDisplayObject;
    public RectTransform summaryCardsRect;
    public CardDisplay[] summaryCardDisplays;
    public Text[] summaryCardQuantityTexts;

    //CHEST ANIMATION
    public GameObject chestLootDisplayObject;
    public GameObject chestDisplayCamera;
    public Animator chestAnim;
	public GameObject chestObjects;

    //LOOT FROM CHEST
	List<CardData> cards;
	int[] amounts;
    bool[] newCard;
	int goldIncrease;
    int gemIncrease;
    int starIncrease;
    int raidPointIncrease;
    int rank;

    private void Awake()
    {
        instance = this;
    }

    public void ChestOpening()
	{
		chestLootDisplayObject.SetActive(true);
		perItemDisplay.SetActive(false);
		summaryDisplayObject.SetActive(false);
        chestDisplayCamera.SetActive(true);
		chestAnim.SetBool("Open", true);
	}

    public void SetChestLootDisplay(List<CardData> cards, int[] amounts, bool[] newCard, int goldIncrease, int gemIncrease, int starIncrease)
    {
        SetChestLootDisplay(cards, amounts, newCard, goldIncrease, gemIncrease, starIncrease, 0, 0);
    }

    public void SetChestLootDisplay(List<CardData> cards, int[] amounts, bool[] newCard, int goldIncrease, int gemIncrease, int starIncrease, int raidPointIncrease, int rank)
	{
		this.cards = cards;
		this.amounts = amounts;
        this.newCard = newCard;
		this.goldIncrease = goldIncrease;
        this.gemIncrease = gemIncrease;
        this.starIncrease = starIncrease;
        this.raidPointIncrease = raidPointIncrease;
        this.rank = rank;

        StartCoroutine(LootDisplay());
    }

    IEnumerator LootDisplay()
    {
        perItemDisplay.SetActive(true);
        ResetPerItemDisplay();
        if (raidPointIncrease > 0)
        {
            UpdateStacksLeft();
            RaidPointsSetup();
            do
            {
                RaidPointIncreaseDisplay(Data.instance.currency.raidPoints, raidPointIncrease);
                yield return null;
            } while (!Input.GetMouseButtonDown(0));
            pointsIcon.SetActive(false);
            pointsForNextReward.gameObject.SetActive(false);
            tierRewardDisplay.gameObject.SetActive(false);
        }

        if (starIncrease > 0)
        {
            UpdateStacksLeft();
            StarsSetup();
            do
            {
                CurrencyIncreaseDisplay(Data.instance.currency.stars, starIncrease);
                yield return null;
            } while (!Input.GetMouseButtonDown(0));
            starsIcon.SetActive(false);
        }

        if (gemIncrease > 0)
        {
            UpdateStacksLeft();
            GemSetup();
            do
            {
                CurrencyIncreaseDisplay(Data.instance.currency.gems, gemIncrease);
                yield return null;
            } while (!Input.GetMouseButtonDown(0));
            gemIcon.SetActive(false);
        }

        if(goldIncrease > 0)
        {
            UpdateStacksLeft();
            GoldSetup();
            do
            {
                CurrencyIncreaseDisplay(Data.instance.currency.gold, goldIncrease);
                yield return null;
            } while (!Input.GetMouseButtonDown(0));
            goldIcon.SetActive(false);
        }

        if(cards.Count > 0)
        {
            currencyAmountObject.SetActive(false);
            cardProgressObject.SetActive(true);
            cardDisplay.typeBackground.gameObject.SetActive(true);

            for(int i = 0; i < cards.Count; i++)
            {
                UpdateStacksLeft();
                if (newCard[i])
                {
                    newItem.SetActive(true);
                }
                else
                {
                    newItem.SetActive(false);
                }
                lootNameText.text = cards[i].displayName;
                cardDisplay.SetCardDisplay(cards[i]);
                lootQuantityText.text = "x" + amounts[i];
                increasingNumber = cards[i].amountOwned - amounts[i];
                if (Mathf.Floor(increasingNumber) < cards[i].AmountNeeded)
                {
                    cardDisplay.fillImage.sprite = Resources.Load<Sprite>("UI/UnitFillTeal");
                }
                if (stacksLeftInChest == 0)
                {
                    chestObjects.SetActive(false);
                }
                do
                {
                    CardIncreaseDisplay(cards[i], amounts[i]);
                    yield return null;
                } while (!Input.GetMouseButtonDown(0));
            }
        }

        summaryDisplayObject.SetActive(true);
        perItemDisplay.SetActive(false);
        SetSummaryDisplay();
        do
        {
            yield return null;
        } while (!Input.GetMouseButtonDown(0));

        stacksLeftInChest = -1;
        chestLootDisplayObject.SetActive(false);
        chestObjects.SetActive(true);
        chestAnim.SetBool("Open", false);
        chestDisplayCamera.SetActive(false);
    }

    //RAID POINTS
    void RaidPointIncreaseDisplay(int target, int increaseAmount)
    {
        if (target > increasingNumber)
        {
            float rate = increaseAmount / 1f;
            increasingNumber = Mathf.MoveTowards(increasingNumber, target, Time.deltaTime * rate);
        }
        if (pointsDisplayType == EventRewardDisplayType.TiersCompleted)
        {
            currencyAmountText.text = Mathf.Floor(increasingNumber).ToString();
        }
        else if (pointsDisplayType == EventRewardDisplayType.SameTier)
        {
            currencyAmountText.text = Mathf.Floor(increasingNumber) + " / " + Data.instance.raids.currentEvent.Point[Data.instance.raids.CurrentEventTier(Mathf.RoundToInt(increasingNumber))].Req;
        }
        else
        {
            if (Mathf.Floor(increasingNumber) >= Data.instance.raids.currentEvent.Point[Data.instance.raids.CurrentEventTier(Data.instance.currency.raidPoints - raidPointIncrease)].Req)
            {
                if (Data.instance.raids.CurrentEventTier(Data.instance.currency.raidPoints) == -1)
                {
                    pointsDisplayType = EventRewardDisplayType.TiersCompleted;
                    currencyLabelText.text = "Your Trophies";
                }
                else
                {
                    pointsDisplayType = EventRewardDisplayType.SameTier;
                    currencyLabelText.text = "Next Reward:";
                }
            }
            else
            {
                currencyAmountText.text = Mathf.Floor(increasingNumber) + " / " + Data.instance.raids.currentEvent.Point[Data.instance.raids.CurrentEventTier(Mathf.RoundToInt(increasingNumber))].Req;
            }
        }

    }

    void RaidPointsSetup()
    {
        increasingNumber = Data.instance.currency.raidPoints - raidPointIncrease;
        if (Data.instance.raids.CurrentEventTier(Mathf.RoundToInt(increasingNumber)) == -1)
        {
            currencyLabelText.text = "Your Trophies";
            lootNameText.text = "Rank #" + rank;
            pointsDisplayType = EventRewardDisplayType.TiersCompleted;
        }
        else if (Data.instance.raids.CurrentEventTier(Mathf.RoundToInt(increasingNumber)) == Data.instance.raids.CurrentEventTier(Data.instance.currency.raidPoints))
        {
            currencyLabelText.text = "Next Reward:";
            lootNameText.text = "";
            pointsForNextReward.gameObject.SetActive(true);
            pointsForNextReward.text = "Earn " + (Data.instance.raids.currentEvent.Point[Data.instance.raids.CurrentEventTier(Data.instance.currency.raidPoints)].Req - Data.instance.currency.raidPoints) + " more trophies for next reward";
            pointsDisplayType = EventRewardDisplayType.SameTier;
        }
        else
        {
            currencyLabelText.text = "Next Reward:";
            lootNameText.text = "";
            pointsDisplayType = EventRewardDisplayType.TierIncrease;
            tierRewardDisplay.gameObject.SetActive(true);
            tierRewardQuantityText.text = "x" + Data.instance.raids.currentEvent.Point[Data.instance.raids.CurrentEventTier(Data.instance.currency.raidPoints - raidPointIncrease)].Amt;
			if (Data.instance.raids.currentEvent.Point[Data.instance.raids.CurrentEventTier(Data.instance.currency.raidPoints - raidPointIncrease)].Rew == "GO")
			{
				Data.instance.currency.gold += Data.instance.raids.currentEvent.Point[Data.instance.raids.CurrentEventTier(Data.instance.currency.raidPoints - raidPointIncrease)].Amt;
				tierRewardDisplay.cardImage.sprite = Resources.Load<Sprite>("CurrencyOffers/GoldPile");
			}
			else
			{
				Data.instance.currency.stars += Data.instance.raids.currentEvent.Point[Data.instance.raids.CurrentEventTier(Data.instance.currency.raidPoints - raidPointIncrease)].Amt;
				tierRewardDisplay.cardImage.sprite = Resources.Load<Sprite>("CurrencyOffers/Star");
			}
        }

        cardDisplay.cardImage.sprite = Resources.Load<Sprite>("CurrencyOffers/RaidPoints");
        cardDisplay.cardFrame.sprite = Resources.Load<Sprite>("UI/DefaultFrame");
        cardDisplay.typeBackground.gameObject.SetActive(false);
        lootQuantityText.text = "x" + raidPointIncrease;
        currencyAmountText.text = increasingNumber.ToString();
        pointsIcon.SetActive(true);
    }
    //RAID POINTS

    //CURRENCY
    void CurrencyIncreaseDisplay(int target, int increaseAmount)
    {
        if(target > increasingNumber)
        {
            float rate = increaseAmount / 1f;
            increasingNumber = Mathf.MoveTowards(increasingNumber, target, Time.deltaTime * rate);
            currencyAmountText.text = Mathf.Floor(increasingNumber).ToString();
        }
    }

    void StarsSetup()
    {
        currencyLabelText.text = "Your Stars";
        lootNameText.text = "Stars";
        cardDisplay.cardImage.sprite = Resources.Load<Sprite>("CurrencyOffers/Star");
        cardDisplay.cardFrame.sprite = Resources.Load<Sprite>("UI/DefaultFrame");
        cardDisplay.typeBackground.gameObject.SetActive(false);
        lootQuantityText.text = "x" + starIncrease;
        increasingNumber = Data.instance.currency.stars - starIncrease;
        currencyAmountText.text = increasingNumber.ToString();
        starsIcon.SetActive(true);
    }

    void GemSetup()
    {
        currencyLabelText.text = "Your Gems";
        lootNameText.text = "Gems";
        cardDisplay.cardImage.sprite = Resources.Load<Sprite>("CurrencyOffers/GemPile");
        cardDisplay.cardFrame.sprite = Resources.Load<Sprite>("UI/DefaultFrame");
        cardDisplay.typeBackground.gameObject.SetActive(false);
        lootQuantityText.text = "x" + gemIncrease;
        increasingNumber = Data.instance.currency.gems - gemIncrease;
        currencyAmountText.text = increasingNumber.ToString();
        gemIcon.SetActive(true);
    }

    void GoldSetup()
    {
        currencyLabelText.text = "Your Gold";
        lootNameText.text = "Gold";
        cardDisplay.cardImage.sprite = Resources.Load<Sprite>("CurrencyOffers/GoldPile");
        cardDisplay.cardFrame.sprite = Resources.Load<Sprite>("UI/DefaultFrame");
        cardDisplay.typeBackground.gameObject.SetActive(false);
        lootQuantityText.text = "x" + goldIncrease;
        increasingNumber = Data.instance.currency.gold - goldIncrease;
        currencyAmountText.text = increasingNumber.ToString();
        goldIcon.SetActive(true);
    }
    //CURRENCY

    //CARD
    void CardIncreaseDisplay(CardData card, int amountIncrease)
    {
        if (increasingNumber < card.amountOwned)
        {
            float rate = amountIncrease / 1f;
            increasingNumber = Mathf.Max(0f, Mathf.MoveTowards(increasingNumber, card.amountOwned, Time.deltaTime * rate));
            if (Mathf.Floor(increasingNumber) < card.AmountNeeded)
            {
                cardDisplay.uIProgressBar.value = increasingNumber / card.AmountNeeded;
            }
            else
            {
                cardDisplay.uIProgressBar.value = 1.0f;
                if (cardDisplay.fillImage.sprite.name != "UnitFillOrange")
                {
                    cardDisplay.fillImage.sprite = Resources.Load<Sprite>("UI/UnitFillOrange");
                }
            }
            cardDisplay.ownedAmountText.text = Mathf.Floor(increasingNumber) + " / " + card.AmountNeeded;
        }
    }
    //CARD

    void ResetPerItemDisplay()
    {
        newItem.SetActive(false);
        goldIcon.SetActive(false);
        gemIcon.SetActive(false);
        starsIcon.SetActive(false);
        pointsIcon.SetActive(false);
        currencyAmountObject.SetActive(true);
        cardProgressObject.SetActive(false);

        stacksLeftInChest = amounts.Length;
        if (raidPointIncrease > 0)
        {
            stacksLeftInChest++;
		}
		if (starIncrease > 0)
		{
			stacksLeftInChest++;
		}
		if (gemIncrease > 0)
        {
            stacksLeftInChest++;
        }
        if (goldIncrease > 0)
        {
            stacksLeftInChest++;
        }
    }

    void UpdateStacksLeft()
    {
        stacksLeftInChest--;
        stacksLeftText.text = stacksLeftInChest + "x Left";
    }

    void SetSummaryDisplay()
	{
        int index = 0;

        if(Data.instance.raids.CurrentEventTier(Data.instance.currency.raidPoints) != Data.instance.raids.CurrentEventTier(Data.instance.currency.raidPoints - raidPointIncrease))
        {
            summaryEventReward.SetActive(true);
            summaryCardQuantityTexts[index].text = "x" + Data.instance.raids.currentEvent.Point[Data.instance.raids.CurrentEventTier(Data.instance.currency.raidPoints - raidPointIncrease)].Amt;
			if (Data.instance.raids.currentEvent.Point[Data.instance.raids.CurrentEventTier(Data.instance.currency.raidPoints - raidPointIncrease)].Rew == "GO")
			{
				summaryCardDisplays[index].cardImage.sprite = Resources.Load<Sprite>("CurrencyOffers/GoldPile");
			}
			else
			{
				summaryCardDisplays[index].cardImage.sprite = Resources.Load<Sprite>("CurrencyOffers/Star");
			}
            summaryCardDisplays[index].cardFrame.sprite = Resources.Load<Sprite>("UI/DefaultFrame");
            index++;
        }
        else
        {
            if(summaryEventReward != null)
            {
                summaryEventReward.SetActive(false);
            }
        }

        if (raidPointIncrease > 0)
        {
            summaryCardQuantityTexts[index].text = "x" + raidPointIncrease;
            summaryCardDisplays[index].cardImage.sprite = Resources.Load<Sprite>("CurrencyOffers/RaidPoints");
            summaryCardDisplays[index].cardFrame.sprite = Resources.Load<Sprite>("UI/DefaultFrame");
            index++;
        }

        if (starIncrease > 0)
        {
            summaryCardQuantityTexts[index].text = "x" + starIncrease;
            summaryCardDisplays[index].cardImage.sprite = Resources.Load<Sprite>("CurrencyOffers/Star");
            summaryCardDisplays[index].cardFrame.sprite = Resources.Load<Sprite>("UI/DefaultFrame");
            index++;
        }

        if (gemIncrease > 0)
        {
            summaryCardQuantityTexts[index].text = "x" + gemIncrease;
            summaryCardDisplays[index].cardImage.sprite = Resources.Load<Sprite>("CurrencyOffers/GemPile");
            summaryCardDisplays[index].cardFrame.sprite = Resources.Load<Sprite>("UI/DefaultFrame");
            index++;
        }

        if (goldIncrease > 0)
        {
            summaryCardQuantityTexts[index].text = "x" + goldIncrease;
            summaryCardDisplays[index].cardImage.sprite = Resources.Load<Sprite>("CurrencyOffers/GoldPile");
            summaryCardDisplays[index].cardFrame.sprite = Resources.Load<Sprite>("UI/DefaultFrame");
            index++;
        }

        for (int i = index; i < summaryCardDisplays.Length; i++)
		{
            if (i < (cards.Count + index))
            {
                summaryCardDisplays[i].gameObject.SetActive(true);
                summaryCardDisplays[i].SetCardDisplay(cards[i - index]);
                summaryCardQuantityTexts[i].text = "x" + amounts[i - index];
            }
            else
			{
				summaryCardDisplays[i].gameObject.SetActive(false);
			}
		}
	}
}
