using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelNode : InteractiveNode
{
    public string travelDescription;
    public string nodeDestination;

    public override bool Setup()
    {
        return true;
    }

    public override void EnterRange()
    {
        WorldManager.instance.uiManager.travelUI.Enable();
    }

    public override void ExitRange()
    {
        WorldManager.instance.uiManager.travelUI.Disable();
    }

    //void NodeInteractionsOld(bool open)
    //{
    //    if (open == true)
    //    {
    //        if (playerNode.type == NodeType.Shop)
    //        {
    //            enterShopUI.Enable();
    //        }
    //        else if (playerNode.type == NodeType.Travel)
    //        {
    //        }
    //    }
    //    else
    //    {
    //        if (playerNode.type == NodeType.Shop)
    //        {
    //            enterShopUI.Disable();
    //        }
    //        else if (playerNode.type == NodeType.Travel)
    //        {
    //        }
    //    }

    //}

    //void ShowWorldChestLoot()
    //{
    //    chestLootDisplay.ChestOpening();

    //    List<CardData> cards = new List<CardData>();
    //    int[] amounts = new int[4];
    //    bool[] newCard = new bool[4];
    //    int gold = 500;

    //    for (int i = 4; i < 8; i++)
    //    {
    //        cards.Add(Data.instance.collection.inventory[Data.instance.collection.deckData.deck[i]]);
    //        amounts[i - 4] = 1;
    //        newCard[i - 4] = true;
    //    }

    //    chestLootDisplay.SetChestLootDisplay(cards, amounts, newCard, gold, 0, 0);

    //    currentNode.treasureChest.gameObject.SetActive(false);
    //    currentNode.treasureChest = null;

    //    nodeMovementEnabled = true;
    //}
}
