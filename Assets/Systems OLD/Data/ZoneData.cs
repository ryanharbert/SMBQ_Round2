using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ZoneData
{
	public string Name;
	public int MaxEnemies;
	public List<string> PossibleEnemies;
	public int[] PossibleLevels;
	public List<string> Nodes;
	public string[] OtherNodes;
	public string Last;
}
