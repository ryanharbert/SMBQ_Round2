using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData
{
	public Dictionary<string, string[]> LinkedNodes;

	public string Island;
	public List<string> VisitedIslands;
    public long TeleportTime;
	public string CurrentPlayerNode;
	public string PreviousPlayerNode;
	public Dictionary<string, EnemyData> Enemies = new Dictionary<string, EnemyData>();
	public WorldChestData worldChests;
    public Dictionary<string, string> travelNodes;
    public ZoneData[] zones;
	public Dictionary<string, IslandData> islands = new Dictionary<string, IslandData>();

	public void Login(GetPlayerCombinedInfoResultPayload playerInfo)
	{
		string linkedNodesJson;

		if (playerInfo.TitleData.TryGetValue("Nodes", out linkedNodesJson))
		{
			LinkedNodes = PlayFabSimpleJson.DeserializeObject<Dictionary<string, string[]>>(linkedNodesJson);
		}

		string zoneJson;

		if (playerInfo.TitleData.TryGetValue("Zones", out zoneJson))
		{
			zones = PlayFabSimpleJson.DeserializeObject<ZoneData[]>(zoneJson);
		}

		string islandsJson;

		if (playerInfo.TitleData.TryGetValue("Islands", out islandsJson))
		{
			islands = PlayFabSimpleJson.DeserializeObject<Dictionary<string, IslandData>>(islandsJson);
		}


		UserDataRecord userDataRecord;

		if (playerInfo.UserReadOnlyData.TryGetValue("NodeData", out userDataRecord))
		{
			JsonUtility.FromJsonOverwrite(userDataRecord.Value, this);
			Dictionary<string, object> dict = PlayFabSimpleJson.DeserializeObject<Dictionary<string, object>>(userDataRecord.Value);

			Island = (string)dict["Island"];
			CurrentPlayerNode = (string)dict["CurrentPlayerNode"];
			PreviousPlayerNode = (string)dict["PreviousPlayerNode"];
			Enemies = PlayFabSimpleJson.DeserializeObject<Dictionary<string, EnemyData>>(dict["Nodes"].ToString());
		}

		if (playerInfo.UserReadOnlyData.TryGetValue("WorldChests", out userDataRecord))
		{
			worldChests = PlayFabSimpleJson.DeserializeObject<WorldChestData>(userDataRecord.Value);
		}
	}

    public string GetCurrentIslandName()
    {
        return GetIslandName(Island);
	}

	public string GetIslandName(string islandId)
	{
		if (islandId == "BigRockIsle")
		{
			return "Big Rock Isle";
		}
		else if (islandId == "CoffeeMountainCove")
		{
			return "Coffee Mountain Cove";
		}
		else if (islandId == "DustyPlateaus")
		{
			return "Dusty Plateaus";
		}
		else if (islandId == "MoltenGorge")
		{
			return "Molten Gorge";
        }
        else if (islandId == "BileBogBluff")
        {
            return "Bile Bog Bluff";
        }
        return "Big Rock Isle";
	}

	public ZoneData GetCurrentZone()
	{
		ZoneData zone = null;
		string currentNode = CurrentPlayerNode;
		EnemyData e;

		if (Enemies.TryGetValue(CurrentPlayerNode, out e))
		{
			currentNode = PreviousPlayerNode;
		}

		foreach (ZoneData z in zones)
		{
			for(int i = 0; i < z.Nodes.Count; i++)
			{
				if(z.Nodes[i] == currentNode)
				{
					zone = z;
					break;
				}
			}

			for (int i = 0; i < z.OtherNodes.Length; i++)
			{
				if (z.OtherNodes[i] == currentNode)
				{
					zone = z;
					break;
				}
			}

			if (zone != null)
			{
				break;
			}
		}

		return zone;
	}

	public int GetCurrentZoneID()
	{
		ZoneData zone = null;
		string currentNode = CurrentPlayerNode;
		EnemyData e;

		if (Enemies.TryGetValue(CurrentPlayerNode, out e))
		{
			currentNode = PreviousPlayerNode;
		}

		for (int h = 0; h < zones.Length; h++)
		{
			for (int i = 0; i < zones[h].Nodes.Count; i++)
			{
				if (zones[h].Nodes[i] == currentNode)
				{
					return h;
				}
			}

			for (int i = 0; i < zones[h].OtherNodes.Length; i++)
			{
				if (zones[h].OtherNodes[i] == currentNode)
				{
					return h;
				}
			}

			if (zone != null)
			{
				break;
			}
		}
		return 0;
	}
}
