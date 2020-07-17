using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IslandData
{
	public List<int> Zones;
	public string[] OtherNodes;
	public string FirstNode;
	public List<string> Chests;
    public float DealMult;
}

