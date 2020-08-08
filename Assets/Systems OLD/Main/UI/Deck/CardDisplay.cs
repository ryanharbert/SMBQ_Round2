using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CardDisplay : MonoBehaviour
{
	public Image cardImage;
	public Text manaText;
	public Text ownedAmountText;
	public Text levelText;
	public Slider uIProgressBar;
	public Image fillImage;
    public Image typeBuffImage;
    public Image heroGlow;

    public Image cardFrame;
    public Image iconFrame;
    public Image fillBackground;
	public Image typeBackground;
    public Image upgradeIcon;

	public Image typeIcon;

    //STARS
    public GameObject starFrame;
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    //NEW STUFF
    public TextMeshProUGUI levelTMP;
    public TextMeshProUGUI manaTMP;
    public TextMeshProUGUI ownedAmountTMP;
    public Image newBackground;
    public Image newType;
    public Image newTypeBackground;
    public Image partialProgressBar;
    public Image upgradeReadyBar;

    [HideInInspector] public Color32 upgradeColor = new Color32(255, 163, 0, 255);

    public Text cardName;
    public Text description;
	public Image descriptionBackground;

	public CardData cardData;

	public virtual void SetCardDisplay(CardData card)
	{
		cardData = card;

        cardImage.sprite = cardData.cardDisplay;
		if(manaText != null)
			manaText.text = cardData.ManaCost.ToString();
		
		if(levelText != null)
			levelText.text = "Level " + cardData.level;
		
		if(uIProgressBar != null)
		{
            if(!cardData.MaxLevel)
            {
                if(ownedAmountText != null)
                {
                    ownedAmountText.text = cardData.amountOwned + " / " + cardData.AmountNeeded;

                    float perc;
                    if (cardData.amountOwned >= cardData.AmountNeeded)
                    {
                        fillImage.sprite = Resources.Load<Sprite>("UI/UnitFillOrange");
                        perc = 1.0f;
                        if (upgradeIcon != null)
                            upgradeIcon.color = upgradeColor;
                    }
                    else
                    {
                        fillImage.sprite = Resources.Load<Sprite>("UI/UnitFillTeal");
                        perc = (float)cardData.amountOwned / (float)cardData.AmountNeeded;
                        if (upgradeIcon != null)
                            upgradeIcon.color = Color.white;
                    }
                    uIProgressBar.value = perc;
                }

                if (ownedAmountTMP != null)
                {
                    if(cardData.amountOwned < cardData.AmountNeeded)
                    {
                        ownedAmountTMP.text = cardData.amountOwned + " / " + cardData.AmountNeeded;
                        upgradeReadyBar.gameObject.SetActive(false);
                        partialProgressBar.gameObject.SetActive(true);
                        uIProgressBar.value = (float)cardData.amountOwned / (float)cardData.AmountNeeded;
                    }
                    else
                    {
                        ownedAmountTMP.text = "Upgrade";
                        upgradeReadyBar.gameObject.SetActive(true);
                        partialProgressBar.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                ownedAmountText.text = cardData.amountOwned.ToString();
                uIProgressBar.value = 0;
                upgradeIcon.color = Color.white;
            }
        }

        if (cardName != null)
        {
            cardName.text = cardData.displayName;
        }
        if (description != null)
        {
            description.text = cardData.shortDesc;
        }


        switch (cardData.faction)
        {
            case Faction.Growth:
                if (newBackground != null)
                {
                    newBackground.color = Data.instance.values.growthColor;
                }
                if (newTypeBackground != null)
                {
                    newTypeBackground.color = Data.instance.values.growthColor;
                }
                if (descriptionBackground != null)
                {
                    descriptionBackground.color = Data.instance.values.growthColor;
                }
                if (cardFrame != null)
                {
                    cardFrame.sprite = Resources.Load<Sprite>("UI/GrowthFrame");
                }
                if (iconFrame != null)
                {
                    iconFrame.sprite = Resources.Load<Sprite>("UI/GrowthIconFrame");
                }
                if (fillBackground != null)
                {
                    fillBackground.sprite = Resources.Load<Sprite>("UI/GrowthFillBackground");
				}
				if (typeBackground != null)
				{
					typeBackground.sprite = Resources.Load<Sprite>("UI/GrowthTypeBackground");
				}
				break;
            case Faction.Ruin:
                if (newBackground != null)
                {
                    newBackground.color = Data.instance.values.ruinColor;
                }
                if (newTypeBackground != null)
                {
                    newTypeBackground.color = Data.instance.values.ruinColor;
                }
                if (descriptionBackground != null)
                {
                    descriptionBackground.color = Data.instance.values.ruinColor;
                }
                if (cardFrame != null)
                {
                    cardFrame.sprite = Resources.Load<Sprite>("UI/RuinFrame");
                }
                if (iconFrame != null)
                {
                    iconFrame.sprite = Resources.Load<Sprite>("UI/RuinIconFrame");
                }
                if (fillBackground != null)
                {
                    fillBackground.sprite = Resources.Load<Sprite>("UI/RuinFillBackground");
				}
				if (typeBackground != null)
				{
					typeBackground.sprite = Resources.Load<Sprite>("UI/RuinTypeBackground");
				}
				break;
            case Faction.Power:
                if (newBackground != null)
                {
                    newBackground.color = Data.instance.values.powerColor;
                }
                if (newTypeBackground != null)
                {
                    newTypeBackground.color = Data.instance.values.powerColor;
                }
                if (descriptionBackground != null)
                {
                    descriptionBackground.color = Data.instance.values.powerColor;
                }
                if (cardFrame != null)
                {
                    cardFrame.sprite = Resources.Load<Sprite>("UI/PowerFrame");
                }
                if (iconFrame != null)
                {
                    iconFrame.sprite = Resources.Load<Sprite>("UI/PowerIconFrame");
                }
                if (fillBackground != null)
                {
                    fillBackground.sprite = Resources.Load<Sprite>("UI/PowerFillBackground");
				}
				if (typeBackground != null)
				{
					typeBackground.sprite = Resources.Load<Sprite>("UI/PowerTypeBackground");
				}
				break;
            default:
                Debug.Log("Card Type is not supported");
                break;
        }

        //NEW
        if (levelTMP != null)
        {
            levelTMP.text = "Level " + cardData.level;
        }
        if (manaTMP != null)
        {
            manaTMP.text = cardData.ManaCost.ToString();
        }
        //NEW

		if (typeIcon != null)
		{
			switch (card.type)
			{
				case CardType.Hero:
					typeIcon.sprite = Resources.Load<Sprite>("UI/TypeHero");
					break;
				case CardType.Stronghold:
					typeIcon.sprite = Resources.Load<Sprite>("UI/TypeStronghold");
					break;
				case CardType.Melee:
					typeIcon.sprite = Resources.Load<Sprite>("UI/TypeMelee");
					break;
				case CardType.Ranged:
					typeIcon.sprite = Resources.Load<Sprite>("UI/TypeRanged");
					break;
				case CardType.Flying:
					typeIcon.sprite = Resources.Load<Sprite>("UI/TypeFlying");
					break;
			}
		}

        if(starFrame != null)
        {
            switch (card.starLevel)
            {
                case 0:
                    starFrame.SetActive(false);
                    break;
                case 1:
                    starFrame.SetActive(true);
                    star1.SetActive(true);
                    star2.SetActive(false);
                    star3.SetActive(false);
                    break;
                case 2:
                    starFrame.SetActive(true);
                    star1.SetActive(true);
                    star2.SetActive(true);
                    star3.SetActive(false);
                    break;
                case 3:
                    starFrame.SetActive(true);
                    star1.SetActive(true);
                    star2.SetActive(true);
                    star3.SetActive(true);
                    break;
            }
        }
	}

	public virtual void SetCardDisplay(string cardName)
	{
		CardData card;
		if (Data.instance.collection.inventory.TryGetValue(cardName, out card))
		{
			SetCardDisplay(card);
		}
		else
		{
			CardData newCard = Resources.Load("Cards/" + cardName) as CardData;
			newCard.itemID = cardName;
			newCard.level = 1;
			newCard.amountOwned = 0;
			SetCardDisplay(newCard);
		}
	}

	public void OpenCardProfile()
	{
        if(cardData != null)
        {
            CardProfile.instance.SetCardProfile(cardData);
        }
	}
}
