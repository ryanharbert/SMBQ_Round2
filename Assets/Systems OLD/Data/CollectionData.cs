using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

[System.Serializable]
public class CollectionData
{
    public DeckData deck;
    public int deckID;
    public DeckData[] decks;
    public List<string> cardCollection = new List<string>();
    public List<string> strongholdCollection = new List<string>();
    public List<string> heroCollection = new List<string>();
	public List<string> cardNotOwned = new List<string>();
	public List<string> strongholdNotOwned = new List<string>();
	public List<string> heroNotOwned = new List<string>();
	public Dictionary<string, CardData> inventory = new Dictionary<string, CardData>();
    public Dictionary<string, CardData> allCards = new Dictionary<string, CardData>();

    bool waitingForLoginData;

    #region Login
    public IEnumerator Login(GetPlayerCombinedInfoResultPayload playerInfo)
    {
        UserDataRecord userDataRecord;

        if (playerInfo.UserData.TryGetValue("Decks", out userDataRecord))
        {
            Dictionary<string, object> deckDict = PlayFabSimpleJson.DeserializeObject<Dictionary<string, object>>(userDataRecord.Value);

            decks = PlayFabSimpleJson.DeserializeObject<DeckData[]>(deckDict["decks"].ToString());
            deckID = System.Convert.ToInt32(deckDict["id"]);

            deck = decks[deckID];
        }

        foreach (ItemInstance item in playerInfo.UserInventory)
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

                List<string> deckList = new List<string>(deck.deck);
                if (deck.stronghold != item.ItemId && card.type == CardType.Stronghold)
                {
                    strongholdCollection.Add(item.ItemId);
                }
                else if (deck.heroes[0] != item.ItemId && card.type == CardType.Hero)
                {
                    heroCollection.Add(item.ItemId);
                }
                else if (!deckList.Contains(item.ItemId) && card.type != CardType.Hero && card.type != CardType.Stronghold)
                {
                    cardCollection.Add(item.ItemId);
                }
            }
        }

        waitingForLoginData = true;
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest { CatalogVersion = "Cards" }, SetNotOwnedCards, GetDataFailure);
        while(waitingForLoginData)
        {
            yield return null;
        }
    }

    void SetNotOwnedCards(GetCatalogItemsResult getCatalogItemsResult)
    {
        foreach (CatalogItem item in getCatalogItemsResult.Catalog)
        {
            CardData card = Resources.Load("Cards/" + item.ItemId) as CardData;

            if (card != null)
            {
                if (!Data.instance.collection.inventory.ContainsKey(item.ItemId))
                {
                    card.itemID = item.ItemId;
                    card.displayName = item.DisplayName;
                    card.level = 1;
                    card.starLevel = 0;
                    card.amountOwned = 0;

                    if (card.type == CardType.Hero)
                    {
                        heroNotOwned.Add(item.ItemId);
                    }
                    else if (card.type == CardType.Stronghold)
                    {
                        strongholdNotOwned.Add(item.ItemId);
                    }
                    else
                    {
                        cardNotOwned.Add(item.ItemId);
                    }
                }

                allCards.Add(item.ItemId, card);
            }
        }
        waitingForLoginData = false;
    }

    void GetDataFailure(PlayFabError error)
    {
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
    #endregion

    public void SetData(string stronghold, List<string> heroes, List<string> deck, List<string> strongholdCollection, List<string> heroCollection, List<string> cardCollection)
	{
        this.decks[deckID].stronghold = stronghold;
        this.decks[deckID].heroes = heroes.ToArray();
        this.decks[deckID].deck = deck.ToArray();
        this.strongholdCollection = strongholdCollection;
        this.heroCollection = heroCollection;
		this.cardCollection = cardCollection;

		SaveDeckData(this);
	}

	void SaveDeckData(CollectionData data)
	{
        Dictionary<string, object> decksJson = new Dictionary<string, object>();
        decksJson.Add("id", deckID);
        decksJson.Add("decks", decks);
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
		{
			Data = new Dictionary<string, string>() {
			{"Decks", JsonUtility.ToJson(decksJson)},
		}
		},
		result => Debug.Log("Successfully updated user data"),
		error => {
			Debug.Log("Got error setting SpellData");
			Debug.Log(error.ErrorDetails);
		});
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
