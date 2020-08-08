using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EditorCard : CardDisplay, IComparable<EditorCard>
{	
	public GameObject parentObject;
	public DeckEditorPlacement placement;

    public GameObject disabledOverlay;
    public GameObject validDeckSlot;
    public GameObject manaObject;
    public GameObject levelObject;
    public GameObject starsObject;

    public bool empty = false;

    public void EmptySlot(Color32 color)
    {
        Debug.Log(name + " empty");
        empty = true;

        newBackground.color = new Color32(207, 132, 110, 255);
        cardImage.gameObject.SetActive(false);
        newTypeBackground.gameObject.SetActive(false);
        uIProgressBar.gameObject.SetActive(false);
        starsObject.gameObject.SetActive(false);
        levelObject.gameObject.SetActive(false);
        manaObject.gameObject.SetActive(false);
        cardData = null;
    }

    public override void SetCardDisplay(string cardName)
    {
        validDeckSlot.SetActive(false);
        //disabledOverlay.SetActive(false);

        if (cardName == "")
        {
            if(!DeckEditor.instance.stronghold.empty && placement == DeckEditorPlacement.DeckRestricted)
            {
                EmptySlot(GameValues.GetFactionColor(DeckEditor.instance.stronghold.cardData.faction));
            }
            else
            {
                EmptySlot(new Color32(207, 132, 110, 255));
            }
        }
        else
        {
            empty = false;

            cardImage.gameObject.SetActive(true);
            newTypeBackground.gameObject.SetActive(true);
            uIProgressBar.gameObject.SetActive(true);
            starsObject.gameObject.SetActive(true);
            levelObject.gameObject.SetActive(true);
            manaObject.gameObject.SetActive(true);

            base.SetCardDisplay(cardName);
        }

        if(DeckEditor.instance.cardGoingToDeck != null && (placement == DeckEditorPlacement.DeckRestricted || placement == DeckEditorPlacement.DeckAny))
        {
            if (DeckEditor.instance.cardGoingToDeck.cardData.type == CardType.Hero && cardData.type != CardType.Hero)
            {
                //disabledOverlay.SetActive(true);
            }
            else if (DeckEditor.instance.cardGoingToDeck.cardData.type != CardType.Hero && (cardData.type == CardType.Hero || cardData.type == CardType.Stronghold))
            {
                //disabledOverlay.SetActive(true);
            }
            else if (placement == DeckEditorPlacement.DeckRestricted && DeckEditor.instance.stronghold.empty)
            {
                //disabledOverlay.SetActive(true);
            }
            else if (placement == DeckEditorPlacement.DeckRestricted && DeckEditor.instance.cardGoingToDeck.cardData.faction != DeckEditor.instance.stronghold.cardData.faction)
            {
                //disabledOverlay.SetActive(true);
            }
            else if(empty)
            {
                validDeckSlot.SetActive(true);
            }
        }
    }

    public void OnClick()
	{
		if (DeckEditor.instance.cardGoingToDeck != null)
		{
            if (DeckEditor.instance.cardGoingToDeck.cardData.type == CardType.Hero && cardData.type != CardType.Hero)
            {
                Warning.instance.Activate("You must select a hero slot");
            }
            else if (DeckEditor.instance.cardGoingToDeck.cardData.type != CardType.Hero && (cardData.type == CardType.Hero || cardData.type == CardType.Stronghold))
            {
                Warning.instance.Activate("You must select a card slot");
            }
            else if (DeckEditor.instance.cardGoingToDeck == this)
            {

            }
            else if (placement == DeckEditorPlacement.DeckRestricted && DeckEditor.instance.stronghold.empty)
            {
                Warning.instance.Activate("You must select a stronghold before filling faction restricted slots");
            }
            else if (placement == DeckEditorPlacement.DeckRestricted && DeckEditor.instance.cardGoingToDeck.cardData.faction != DeckEditor.instance.stronghold.cardData.faction)
            {
                Warning.instance.Activate("This slot must be the same faction as your stronghold");
            }
            else if(placement == DeckEditorPlacement.DeckRestricted && cardData.type == CardType.Hero)
            {
                WorldManager.instance.SwitchHero(DeckEditor.instance.cardGoingToDeck.cardData.itemID);
                DeckEditor.instance.SelectCardToReplace(this);
            }
            else
            {
                DeckEditor.instance.SelectCardToReplace(this);
            }
		}
		else
		{
			CardProfile.instance.SetCardProfile(this);
		}
    }

    public void Use()
    {
        if (cardData.type == CardType.Stronghold)
        {
            DeckEditor.instance.deckEditorRect.anchoredPosition = new Vector2(DeckEditor.instance.deckEditorRect.rect.x, 0f);
            EditorCard cardGoingToDeck = this;
            CardData cardLeavingDeck = DeckEditor.instance.stronghold.cardData;
            DeckEditor.instance.stronghold.SetCardDisplay(cardGoingToDeck.cardData);
            cardGoingToDeck.SetCardDisplay(cardLeavingDeck);

            for(int i = 0; i < 4; i++)
            {
                if(DeckEditor.instance.deck[i].cardData.faction != DeckEditor.instance.stronghold.cardData.faction)
                {
                    DeckEditor.instance.deck[i].SetCardDisplay("");
                }
            }
            if(DeckEditor.instance.heroes[0].cardData.faction != DeckEditor.instance.stronghold.cardData.faction)
            {
                DeckEditor.instance.heroes[0].SetCardDisplay("");
            }

            DeckEditor.instance.factionBorder.color = GameValues.GetFactionColor(DeckEditor.instance.stronghold.cardData.faction);
            DeckEditor.instance.factionLabel.color = GameValues.GetFactionColor(DeckEditor.instance.stronghold.cardData.faction);
            DeckEditor.instance.factionTop.color = GameValues.GetFactionColor(DeckEditor.instance.stronghold.cardData.faction);

            DeckEditor.instance.RefreshDisplay();
        }
        else
        {
            DeckEditor.instance.SelectCardForDeck(this);
        }
    }

    public int CompareTo(EditorCard editorSpell)
	{
		int thisName = 0;
		int otherName = 0;

		int.TryParse(gameObject.name, out thisName);
		int.TryParse(editorSpell.name, out otherName);
		

		return (thisName - otherName);
	}
}
