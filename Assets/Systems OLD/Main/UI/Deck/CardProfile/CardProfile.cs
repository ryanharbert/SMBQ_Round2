using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class CardProfile : MonoBehaviour
{

	public static CardProfile instance;

	public GameObject displayObject;

	public NavBarToggle	starPowerToggle;
	public NavBarToggle abilitiesToggle;
	public NavBarToggle infoToggle;
	public NavBarToggle findToggle;

	public GameObject starPowerObject;
	public GameObject abilitiesObject;
	public GameObject infoObject;
	public GameObject findObject;

	public Text cardNameText;
	public Text descText;
	public CardDisplay cardDisplay;
    public GameObject manaObject;

    public RectTransform tabTopBorder;
    public Text tabTitleText;

	public RectTransform bottomBorder;

	public Text levelUpGoldCost;
	public Button levelUpButton;
	public Image levelUpButtonImage;
	public Button useButton;
    public Text cantUpgradeWarning;

    public Text strongholdBonusDesc;

    public CardUpgradeDisplay cardUpgradeDisplay;

	[HideInInspector] public EditorCard editorCard;
	[HideInInspector] public CardData card;
	[HideInInspector] public Color textColor;

	public static bool serverUpgraded = false;

	private void Awake()
	{
		instance = this;
		textColor = levelUpGoldCost.color;
    }

	public void SetCardProfile(CardData cardData)
	{
		EditorCard e = new EditorCard();
		e.cardData = cardData;
		e.placement = DeckEditorPlacement.Unowned;
		SetCardProfile(e);
	}

	public void SetCardProfile(EditorCard ec)
	{
		editorCard = ec;
		card = ec.cardData;
        displayObject.SetActive(true);

        starPowerToggle.toggle.isOn = true;
        TabBorderSet();

        //Card Info
        cardNameText.text = card.displayName;
		cardDisplay.SetCardDisplay(ec.cardData);
        descText.text = cardDisplay.cardData.description;
        if(card.type == CardType.Stronghold || card.type == CardType.Hero)
        {
            manaObject.SetActive(false);
            if (card.type == CardType.Hero)
            {
                abilitiesToggle.gameObject.SetActive(true);
                strongholdBonusDesc.gameObject.SetActive(false);
            }
            else if (card.type == CardType.Stronghold)
            {
                abilitiesToggle.gameObject.SetActive(false);
                strongholdBonusDesc.gameObject.SetActive(true);
                switch (card.faction)
                {
                    case Faction.Growth:
                        strongholdBonusDesc.text = "Growth cards get 10% stat bonus";
                        strongholdBonusDesc.color = Data.instance.values.growthColor;
                        break;
                    case Faction.Ruin:
                        strongholdBonusDesc.text = "Ruin cards get 10% stat bonus";
                        strongholdBonusDesc.color = Data.instance.values.ruinColor;
                        break;
                    case Faction.Power:
                        strongholdBonusDesc.text = "Power cards get 10% stat bonus";
                        strongholdBonusDesc.color = Data.instance.values.powerColor;
                        break;
                    default:
                        Debug.Log("Card Type is not supported");
                        break;
                }
            }
        }
        else
        {
            manaObject.SetActive(true);
            abilitiesToggle.gameObject.SetActive(false);
            strongholdBonusDesc.gameObject.SetActive(false);
        }

        //Inventory Placement
        if (ec.placement == DeckEditorPlacement.Collection)
		{
			useButton.gameObject.SetActive(true);
			useButton.onClick.RemoveAllListeners();
			useButton.onClick.AddListener(Use);
			levelUpButton.gameObject.SetActive(true);
			bottomBorder.sizeDelta = new Vector2(170, bottomBorder.sizeDelta.y);
        }
        else if (ec.placement == DeckEditorPlacement.Unowned)
        {
            useButton.gameObject.SetActive(false);
            levelUpButton.gameObject.SetActive(false);
            bottomBorder.sizeDelta = new Vector2(1060, bottomBorder.sizeDelta.y);
        }
        else
		{
			useButton.gameObject.SetActive(false);
			levelUpButton.gameObject.SetActive(true);
			bottomBorder.sizeDelta = new Vector2(610, bottomBorder.sizeDelta.y);
		}

        //Upgrade State
        if (card.MaxLevel)
        {
            levelUpButton.onClick.RemoveAllListeners();
            levelUpButton.onClick.AddListener(CantUpgradeWarningOn);
            cantUpgradeWarning.text = "Max Level Reached";
            levelUpButtonImage.color = new Color32(255, 255, 255, 90);
            levelUpGoldCost.color = Color.red;
        }
        else if (!card.Upgradeable)
		{
			levelUpButton.onClick.RemoveAllListeners();
			levelUpButton.onClick.AddListener(CantUpgradeWarningOn);
			cantUpgradeWarning.text = "You need " + (card.AmountNeeded - card.amountOwned) + " more cards to Level Up";
			levelUpButtonImage.color = new Color32(255, 255, 255, 90);
			levelUpGoldCost.color = textColor;

			if (TutorialWorldMap.instance != null && (ec.placement != DeckEditorPlacement.DeckAny || ec.placement != DeckEditorPlacement.DeckRestricted))
			{
				TutorialWorldMap.instance.Use();
				useButton.interactable = true;
			}
		}
        else if (card.level == (Data.instance.currency.playerLevel + 5))
        {
            levelUpButton.onClick.RemoveAllListeners();
            levelUpButton.onClick.AddListener(CantUpgradeWarningOn);
            cantUpgradeWarning.text = "Earn experience from fights to increase your Player Level to " + (Data.instance.currency.playerLevel + 1);
            levelUpButtonImage.color = new Color32(255, 255, 255, 90);
            levelUpGoldCost.color = textColor;
        }
        else if(Data.instance.currency.gold < Data.instance.values.upgradeGoldCost[card.level - 1])
		{
			levelUpButton.onClick.RemoveAllListeners();
			levelUpButton.onClick.AddListener(CantUpgradeWarningOn);
			cantUpgradeWarning.text = "Not Enough Gold";
			levelUpGoldCost.color = Color.red;
		}
		else
		{
			levelUpButton.onClick.RemoveAllListeners();
			levelUpButton.onClick.AddListener(Upgrade);
			levelUpButtonImage.color = new Color32(255, 255, 255, 255);
			levelUpGoldCost.color = textColor;

			if (TutorialWorldMap.instance != null)
			{
				Debug.Log("Tutorial");
				TutorialWorldMap.instance.LevelUp();
				useButton.interactable = false;
			}
		}
        if(!card.MaxLevel)
        {
            levelUpGoldCost.text = card.GoldNeeded.ToString();
        }
        else
        {
            levelUpGoldCost.text = "Max Level";
        }
	}

    //BUTTON FUNCTIONS
	public void Use()
	{
		if (TutorialWorldMap.instance != null)
		{
			TutorialWorldMap.instance.DisableTutorial();
		}
        editorCard.Use();
		displayObject.SetActive(false);
	}

	public void CantUpgradeWarningOn()
	{
		cantUpgradeWarning.enabled = true;
		Invoke("CantUpgradeWarningOff", 2.5f);
	}

	public void CantUpgradeWarningOff()
	{
		cantUpgradeWarning.enabled = false;
	}

	public void Upgrade()
	{
		cardUpgradeDisplay.Setup(editorCard.cardData);
        serverUpgraded = false;
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest(){FunctionName = "upgradeCard", FunctionParameter = new {card = editorCard.cardData.itemID}}, CardUpgraded, OnUpgradeFailure);
	}

	private void CardUpgraded(ExecuteCloudScriptResult result)
	{
		if(result.FunctionResult != null)
		{
			CardData card;
			if (Data.instance.collection.inventory.TryGetValue(result.FunctionResult.ToString(), out card))
			{
                Data.instance.currency.gold -= card.GoldNeeded;
				card.amountOwned -= card.AmountNeeded;
				card.level += 1;
			}

			editorCard.SetCardDisplay(card);
			SetCardProfile(editorCard);

			serverUpgraded = true;
            NavBar.instance.SetDeckNotification();
        }
	}

    public void ResetCardProfile()
    {
        editorCard.SetCardDisplay(card);
        SetCardProfile(editorCard);
    }

	private void OnUpgradeFailure(PlayFabError error)
	{
		Debug.LogError("Here's some debug information:");
		Debug.LogError(error.GenerateErrorReport());
	}
    //BUTTON FUNCTIONS

    //TOGGLES
    public void StarPower(bool on)
    {
        ToggleGameObject(starPowerObject, on, starPowerToggle);
    }

    public void Abilities(bool on)
    {
        ToggleGameObject(abilitiesObject, on, abilitiesToggle);
    }

    public void Info(bool on)
    {
        ToggleGameObject(infoObject, on, infoToggle);
    }

    public void Find(bool on)
    {
        ToggleGameObject(findObject, on, findToggle);
    }

    public void ToggleGameObject(GameObject go, bool active, NavBarToggle toggle)
    {
        if (active)
        {
            go.SetActive(true);
            toggle.toggleText.color = NavBar.instance.highlightColor;
            toggle.toggleText.fontSize = 70;
            TabBorderSet();
        }
        else
        {
            go.SetActive(false);
            toggle.toggleText.color = NavBar.instance.normalColor;
            toggle.toggleText.fontSize = 60;
        }
    }

    void TabBorderSet()
    {
        if (!abilitiesToggle.toggle.isOn)
        {
            tabTopBorder.anchoredPosition = new Vector2(212, 0);
            tabTopBorder.sizeDelta = new Vector2(1039, tabTopBorder.sizeDelta.y);
            if(starPowerToggle.toggle.isOn)
            {
                tabTitleText.text = "";
            }
            else if(infoToggle.toggle.isOn)
            {
                tabTitleText.text = "Info";
            }
            else
            {
                tabTitleText.text = "Find";
            }
        }
        else
        {
            tabTopBorder.anchoredPosition = new Vector2(347, 0);
            tabTopBorder.sizeDelta = new Vector2(905, tabTopBorder.sizeDelta.y);
            tabTitleText.text = "Abilities";
        }
    }
    //TOGGLES
}
