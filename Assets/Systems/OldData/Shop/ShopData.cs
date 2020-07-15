using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

[System.Serializable]
public class ShopData
{
    public List<CardAmountData> Contents;
    public List<bool> Purchased;
	public List<string> ShopPool;
}
