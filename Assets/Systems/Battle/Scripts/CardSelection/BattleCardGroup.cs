using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCardGroup : MonoBehaviour {

	public BattleCard[] battleCards;
	public BattleHero battleHero;

	public static BattleCard selectedCard;

	public void Set()
	{
		foreach (BattleCard bc in battleCards)
		{
			bc.cardData = Battle.state.playerDeck[bc.index];
			bc.cardDisplay.SetCardDisplay(bc.cardData.itemID);
		}
	}

	public static void SelectCard(BattleCard bc)
	{
		if (selectedCard != null)
		{
			selectedCard.Toggle(false);
		}
		selectedCard = bc;
		selectedCard.Toggle(true);
	}

    public static void CardPlayed(BattleState s)
    {
        if (selectedCard.index < 4)
        {
            selectedCard.cardDisplay.SetCardDisplay(s.playerDeck[s.cardSelected].itemID);
            selectedCard.cardData = s.playerDeck[s.cardSelected];
        }
        selectedCard.Toggle(false);
        selectedCard = null;
    }
}
