using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;

[System.Serializable]
public class GameValues
{
	public int[] upgradeCost;
	public int[] upgradeGoldCost;
    public int[] levelUpCost;

    public Color32 ruinColor = new Color32(31, 133, 176, 255);
    public Color32 growthColor = new Color32(70, 194, 0, 255);
    public Color32 powerColor = new Color32(217, 22, 26, 255);

    public void Login(GetPlayerCombinedInfoResultPayload playerInfo)
	{
		string upgradeCostJson;

		if (playerInfo.TitleData.TryGetValue("UpgradeCost", out upgradeCostJson))
		{
			upgradeCost = PlayFabSimpleJson.DeserializeObject<int[]>(upgradeCostJson);
		}

        upgradeGoldCost = new int[upgradeCost.Length];
        levelUpCost = new int[upgradeCost.Length];

        for(int i = 0; i < upgradeCost.Length; i++)
        {
            upgradeGoldCost[i] = upgradeCost[i] * 10;
            levelUpCost[i] = upgradeCost[i] * 5;
        }
	}

    public static Color GetFactionColor(Faction order)
    {
        Color color = Color.white;
        switch (order)
        {
            case Faction.Growth:
                color = new Color32(13, 93, 0, 255);
                break;
            case Faction.Ruin:
                color = new Color32(0, 0, 255, 255);
                break;
            case Faction.Power:
                color = new Color32(217, 0, 0, 255);
                break;
            default:
                Debug.Log("Type Bonus is not supported");
                break;
        }
        return color;
    }

    public int GetLevelUpCost(int level)
    {
        return levelUpCost[level - 1];
    }
}
