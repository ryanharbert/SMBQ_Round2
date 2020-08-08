using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DeckEditor : MonoBehaviour
{
	public static DeckEditor instance;

    public List<EditorCard> heroes = new List<EditorCard>();
    public EditorCard stronghold;
    public List<EditorCard> deck = new List<EditorCard>();
	public List<EditorCard> cardCollection = new List<EditorCard>();
    public List<EditorCard> heroCollection = new List<EditorCard>();
    public List<EditorCard> strongholdCollection = new List<EditorCard>();
	public List<EditorCard> cardNotOwned = new List<EditorCard>();
	public List<EditorCard> heroNotOwned = new List<EditorCard>();
	public List<EditorCard> strongholdNotOwned = new List<EditorCard>();

    public RectTransform[] rects;

    public GameObject collectionObject;
    public GameObject cardsCollectionObject;
    public GameObject strongholdCollectionObject;
    public GameObject heroCollectionObject;
	public GameObject collectionFilterObject;
    public RectTransform deckEditorRect;

	public Toggle cardToggle;
	public Toggle heroToggle;
	public Toggle strongholdToggle;

	public Toggle powerToggle;
	public Toggle ruinToggle;
	public Toggle growthToggle;

	public bool showGrowth = true;
    public bool showRuin = true;
    public bool showPower = true;

    public Image factionBorder;
    public Image factionTop;
    public Text factionLabel;

	public GameObject addCardToDeckObject;
	public CardDisplay cardDisplayGoingToDeck;
	[HideInInspector] public EditorCard cardGoingToDeck;
	[HideInInspector] public bool cardSelected = false;
	

	private void Awake()
	{
		instance = this;
    }

	private void OnEnable()
	{
		Open();
		if(TutorialWorldMap.instance != null)
		{
			TutorialWorldMap.instance.DeckEditor();
		}
	}

	private void OnDisable()
	{
		Close();
		if (TutorialWorldMap.instance != null)
		{
			TutorialWorldMap.instance.clickOnArchers.SetActive(false);
			TutorialWorldMap.instance.archerArrow.SetActive(false);
			TutorialWorldMap.instance.clickOnDeck.SetActive(true);
		}
	}

	public void Open()
	{
		powerToggle.isOn = true;
		ruinToggle.isOn = true;
		growthToggle.isOn = true;

        CollectionData c = Data.instance.collection;
		stronghold = CreateCardDisplays(new List<EditorCard>() { stronghold }, new List<string>() { c.deck.stronghold }, false)[0];
        heroes = CreateCardDisplays(heroes, new List<string>(c.deck.heroes), false);
        deck = CreateCardDisplays(deck, new List<string>(c.deck.deck), false);
        cardCollection = CreateCardDisplays(cardCollection, new List<string>(c.cardCollection), true);
        //heroCollection = CreateCardDisplays(heroCollection, new List<string>(c.heroCollection), true);
        //strongholdCollection = CreateCardDisplays(strongholdCollection, new List<string>(c.strongholdCollection), true);
        //cardNotOwned = CreateCardDisplays(cardNotOwned, new List<string>(c.cardNotOwned), true);
        //heroNotOwned = CreateCardDisplays(heroNotOwned, new List<string>(c.heroNotOwned), true);
        //strongholdNotOwned = CreateCardDisplays(strongholdNotOwned, new List<string>(c.strongholdNotOwned), true);

        factionBorder.color = GameValues.GetFactionColor(stronghold.cardData.faction);
        factionLabel.color = GameValues.GetFactionColor(stronghold.cardData.faction);
        factionTop.color = GameValues.GetFactionColor(stronghold.cardData.faction);

        if (cardToggle.isOn)
		{
			CardCollection(true);
			HeroCollection(false);
			StrongholdCollection(false);
		}
		else if(heroToggle.isOn)
		{
			CardCollection(false);
			HeroCollection(true);
			StrongholdCollection(false);
		}
		else if(strongholdToggle.isOn)
		{
			CardCollection(false);
			HeroCollection(false);
			StrongholdCollection(true);
		}
        
        Invoke("ResetCanvases", 0.2f);
    }

	public void Close()
	{
		RefreshDisplay();
    }

	public List<EditorCard> CreateCardDisplays(List<EditorCard> editorCards, List<string> cardNames, bool filters)
	{
        for (int i = 0; i < editorCards.Count; i++)
		{
			bool showCard = true;

            if (cardNames.Count > i)
			{
                if(editorCards[i].placement == DeckEditorPlacement.DeckAny || editorCards[i].placement == DeckEditorPlacement.DeckRestricted)
                {
                    editorCards[i].SetCardDisplay(cardNames[i]);
                }
                else if (Data.instance.collection.allCards.ContainsKey(cardNames[i]))
				{
					editorCards[i].SetCardDisplay(cardNames[i]);
                    if (filters == true)
                    {
                        switch (editorCards[i].cardData.faction)
                        {
                            case Faction.Growth:
                                showCard = showGrowth;
                                break;
                            case Faction.Ruin:
                                showCard = showRuin;
                                break;
                            case Faction.Power:
                                showCard = showPower;
                                break;
                            default:
                                showCard = true;
                                break;
                        }
                    }
                    else
                    {
                        showCard = true;
                    }
                }

                if (showCard)
                {
                    if (editorCards[i].parentObject == null)
                    {
                        editorCards[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        editorCards[i].parentObject.SetActive(true);
                    }
                }
                else
                {
                    if (editorCards[i].parentObject == null)
                    {
                        editorCards[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        editorCards[i].parentObject.SetActive(false);
                    }
                }
            }
            else
            {
                if (editorCards[i].parentObject == null)
                {
                    editorCards[i].gameObject.SetActive(false);
                }
                else
                {
                    editorCards[i].parentObject.SetActive(false);
                }
            }
		}

		return editorCards;
	}

	public void SelectCardForDeck(EditorCard e)
	{
		cardGoingToDeck = e;
		
		collectionObject.SetActive(false);
		collectionFilterObject.SetActive(false);
		addCardToDeckObject.SetActive(true);
		cardDisplayGoingToDeck.SetCardDisplay(e.cardData);
		deckEditorRect.anchoredPosition = new Vector2(deckEditorRect.rect.x, 0f);
	}

	public void SelectCardToReplace(EditorCard ec)
	{
		CardData cardLeavingDeck = ec.cardData;
		
		ec.SetCardDisplay(cardGoingToDeck.cardData);

        //List<EditorCard> list = CreateCardDisplays(new List<EditorCard>() { cardGoingToDeck }, new List<string>() { cardLeavingDeck.itemID }, true);
		cardGoingToDeck.SetCardDisplay(cardLeavingDeck);

		RefreshDisplay();
	}

	public void UpdateCardData()
    {
        List<string> heroNames = new List<string>();
        foreach (EditorCard e in deck)
        {
            heroNames.Add(e.cardData.itemID);
        }

        List<string> deckNames = new List<string>();
		foreach (EditorCard deckName in deck)
		{
			deckNames.Add(deckName.cardData.itemID);
		}

		List<string> collectionNames = new List<string>();
		foreach (EditorCard collectionName in cardCollection)
		{
            if(collectionName.cardData != null)
            {
                collectionNames.Add(collectionName.cardData.itemID);
            }
        }

        List<string> strongholdCollectionNames = new List<string>();
        foreach (EditorCard strongholdName in strongholdCollection)
        {
            if (strongholdName.cardData != null)
            {
                strongholdCollectionNames.Add(strongholdName.cardData.itemID);
            }
        }

        List<string> heroCollectionNames = new List<string>();
        foreach (EditorCard heroName in heroCollection)
        {
            if (heroName.cardData != null)
            {
                heroCollectionNames.Add(heroName.cardData.itemID);
            }
        }

        Data.instance.collection.SetData(stronghold.cardData.itemID, heroNames, deckNames, strongholdCollectionNames, heroCollectionNames, collectionNames);

    }

    public void CardCollection(bool on)
    {
        cardsCollectionObject.SetActive(on);

        ResetCanvases();
    }

    public void StrongholdCollection(bool on)
    {
        strongholdCollectionObject.SetActive(on);

        ResetCanvases();
    }

    public void HeroCollection(bool on)
    {
        heroCollectionObject.SetActive(on);

        ResetCanvases();
    }

    public void GrowthFilter(bool on)
    {
        showGrowth = on;
        CollectionData c = Data.instance.collection;
        cardCollection = CreateCardDisplays(cardCollection, new List<string>(c.cardCollection), true);
        heroCollection = CreateCardDisplays(heroCollection, new List<string>(c.heroCollection), true);
        strongholdCollection = CreateCardDisplays(strongholdCollection, new List<string>(c.strongholdCollection), true);
		cardNotOwned = CreateCardDisplays(cardNotOwned, new List<string>(c.cardNotOwned), true);
		heroNotOwned = CreateCardDisplays(heroNotOwned, new List<string>(c.heroNotOwned), true);
		strongholdNotOwned = CreateCardDisplays(strongholdNotOwned, new List<string>(c.strongholdNotOwned), true);

        ResetCanvases();
    }

    public void RuinFilter(bool on)
    {
        showRuin = on;
        CollectionData c = Data.instance.collection;
        cardCollection = CreateCardDisplays(cardCollection, new List<string>(c.cardCollection), true);
        heroCollection = CreateCardDisplays(heroCollection, new List<string>(c.heroCollection), true);
        strongholdCollection = CreateCardDisplays(strongholdCollection, new List<string>(c.strongholdCollection), true);
		cardNotOwned = CreateCardDisplays(cardNotOwned, new List<string>(c.cardNotOwned), true);
		heroNotOwned = CreateCardDisplays(heroNotOwned, new List<string>(c.heroNotOwned), true);
		strongholdNotOwned = CreateCardDisplays(strongholdNotOwned, new List<string>(c.strongholdNotOwned), true);

        ResetCanvases();
    }

    public void PowerFilter(bool on)
    {
        showPower = on;
        CollectionData c = Data.instance.collection;
        cardCollection = CreateCardDisplays(cardCollection, new List<string>(c.cardCollection), true);
        heroCollection = CreateCardDisplays(heroCollection, new List<string>(c.heroCollection), true);
        strongholdCollection = CreateCardDisplays(strongholdCollection, new List<string>(c.strongholdCollection), true);
		cardNotOwned = CreateCardDisplays(cardNotOwned, new List<string>(c.cardNotOwned), true);
		heroNotOwned = CreateCardDisplays(heroNotOwned, new List<string>(c.heroNotOwned), true);
		strongholdNotOwned = CreateCardDisplays(strongholdNotOwned, new List<string>(c.strongholdNotOwned), true);

        ResetCanvases();
    }

    void ResetCanvases()
    {
        foreach (RectTransform r in rects)
        {
            if (r.gameObject.activeInHierarchy)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(r);
            }
        }
    }

    public void RefreshDisplay()
	{
		cardGoingToDeck = null;
		
		collectionObject.SetActive(true);
		collectionFilterObject.SetActive(true);
		addCardToDeckObject.SetActive(false);
	}

}
