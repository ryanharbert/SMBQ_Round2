using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

[System.Serializable]
public class CollectionData
{
    public DeckData deckData;
	public List<string> cardCollection = new List<string>();
    public List<string> strongholdCollection = new List<string>();
    public List<string> heroCollection = new List<string>();
	public List<string> cardNotOwned = new List<string>();
	public List<string> strongholdNotOwned = new List<string>();
	public List<string> heroNotOwned = new List<string>();
	public Dictionary<string, CardData> inventory = new Dictionary<string, CardData>();
    public Dictionary<string, CardData> allCards = new Dictionary<string, CardData>();
    public static Faction typeBonus = Faction.None;

	public List<CardData> GetDeck()
	{
		List<CardData> deckList = new List<CardData>();

        CardData heroCardData;
        if (inventory.TryGetValue(deckData.hero, out heroCardData))
        {
            deckList.Add(heroCardData);
        }

		foreach (string spellName in deckData.deck)
		{
			CardData cardData;
			if(inventory.TryGetValue(spellName, out cardData))
			{
				deckList.Add(cardData);
			}
	    }

        return deckList;
	}

	public void SetData(string stronghold, string hero, List<string> deck, List<string> strongholdCollection, List<string> heroCollection, List<string> cardCollection)
	{
        deckData.stronghold = stronghold;
        deckData.hero = hero;
        deckData.deck = deck.ToArray();
        this.strongholdCollection = strongholdCollection;
        this.heroCollection = heroCollection;
		this.cardCollection = cardCollection;

		SendCollectionData(this);
	}

	void SendCollectionData(CollectionData data)
	{
		PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
		{
			Data = new Dictionary<string, string>() {
			{"Decks", JsonUtility.ToJson(deckData)},
		}
		},
		result => Debug.Log("Successfully updated user data"),
		error => {
			Debug.Log("Got error setting SpellData");
			Debug.Log(error.ErrorDetails);
		});
	}
	
	public void Login(GetPlayerCombinedInfoResultPayload playerInfo)
	{
		UserDataRecord userDataRecord;

		if (playerInfo.UserData.TryGetValue("Decks", out userDataRecord))
        {
            JsonUtility.FromJsonOverwrite(userDataRecord.Value, deckData);
		}

		foreach(ItemInstance item in playerInfo.UserInventory)
		{
            CardData card = Resources.Load("Cards/" + item.ItemId) as CardData;

            if (card != null)
            {
                card.itemID = item.ItemId;
                card.displayName = item.DisplayName;
                if (item.CustomData != null)
                {
                    string level = "";
                    if (item.CustomData.TryGetValue("Level", out level))
                    {
                        card.level = int.Parse(level);
                    }
                    else
                    {
                        card.level = 1;
                    }

                    string star = "";
                    if (item.CustomData.TryGetValue("Star", out star))
                    {
                        card.starLevel = int.Parse(star);
                    }
                    else
                    {
                        card.starLevel = 0;
                    }
                }
                else
                {
                    card.level = 1;
					card.starLevel = 0;
				}
                card.amountOwned = item.RemainingUses.GetValueOrDefault() - 1;

                inventory.Add(item.ItemId, card);

                List<string> deckList = new List<string>(deckData.deck);
                if (deckData.stronghold != item.ItemId && card.type == CardType.Stronghold)
                {
                    strongholdCollection.Add(item.ItemId);
                }
                else if (deckData.hero != item.ItemId && card.type == CardType.Hero)
                {
                    heroCollection.Add(item.ItemId);
                }
                else if (!deckList.Contains(item.ItemId) && card.type != CardType.Hero && card.type != CardType.Stronghold)
                {
                    cardCollection.Add(item.ItemId);
                }

                if (card.type == CardType.Stronghold && card.itemID == deckData.stronghold)
                {
                    typeBonus = card.faction;
                }
            }
        }
        
	}

    public CardData AddCards(string cardAdded, int amount)
    {

        CardData card;
        if (Data.instance.collection.inventory.TryGetValue(cardAdded, out card))
        {
            card.amountOwned += amount;
        }
        else
        {
            card = Data.instance.collection.allCards[cardAdded];
            Data.instance.collection.inventory.Add(cardAdded, Data.instance.collection.allCards[cardAdded]);
            Data.instance.collection.inventory[cardAdded].amountOwned = amount - 1;

            if (Data.instance.collection.inventory[cardAdded].type == CardType.Stronghold)
            {
                strongholdCollection.Add(cardAdded);
				strongholdNotOwned.Remove(cardAdded);
			}
            else if (Data.instance.collection.inventory[cardAdded].type == CardType.Hero)
            {
                heroCollection.Add(cardAdded);
				heroNotOwned.Remove(cardAdded);
			}
            else
            {
                cardCollection.Add(cardAdded);
				cardNotOwned.Remove(cardAdded);
			}
        }
        if(NavBar.instance != null)
        {
            NavBar.instance.SetDeckNotification();
        }
        return card;
    }
}
