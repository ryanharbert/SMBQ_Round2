using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileFind : MonoBehaviour {

    public Text[] locations;

    public void OnEnable()
    {
        string cardID = CardProfile.instance.card.itemID;

        int i = 0;

        List<string> zoneNames = new List<string>();
        for (int h = 0; h < Data.instance.world.zones.Length; h++)
        {
            if(Data.instance.world.zones[h].PossibleEnemies.Contains(cardID) && !zoneNames.Contains(Data.instance.world.zones[h].Name))
            {
                zoneNames.Add(Data.instance.world.zones[h].Name);
                string island = "";
                foreach(KeyValuePair<string, IslandData> k in Data.instance.world.islands)
                {
                    if(k.Value.Zones.Contains(h))
                    {
                        island = k.Key;
                        break;
                    }
                }
                string s = "World: " + Data.instance.world.zones[h].Name + " on " + Data.instance.world.GetIslandName(island);
                i = SetLocation(i, s);
            }
        }

        for (int h = 0; h < Data.instance.raids.eventRaids.Count; h++)
        {
            ChestData c = Data.instance.raids.allRaids[Data.instance.raids.eventRaids[h]].Chest;
            if (c.pool.Contains(cardID) || c.jackpotPool.Contains(cardID))
            {
                string s = "Raid: " + Data.instance.raids.eventRaids[h];
                i = SetLocation(i, s);
            }
        }

        List<string> shops = new List<string>();
        foreach(KeyValuePair<string, ShopData> k in Data.instance.shop.shops)
        {
			if(k.Key != "Main" && k.Value.ShopPool.Contains(cardID))
			{
				string island = "";
				List<int> zones = null;
				foreach (KeyValuePair<string, IslandData> kk in Data.instance.world.islands)
                {
                    if (kk.Value.Chests.Contains(k.Key))
					{
						island = kk.Key;
						zones = kk.Value.Zones;
						break;
					}
				}

				string zone = "";
				foreach(int h in zones)
				{
					if(Data.instance.world.zones[h].Nodes.Contains(k.Key))
					{
						zone = Data.instance.world.zones[h].Name;
					}
				}

				shops.Add("Shop: " + zone + " on " + Data.instance.world.GetIslandName(island));

				string s = "Chest: " + Data.instance.chests.dict[k.Key].displayName;
				i = SetLocation(i, s);
			}
        }

        foreach(string s in shops)
        {
            i = SetLocation(i, s);
        }

		for(int h = i; h < locations.Length; h++)
		{
			locations[h].gameObject.SetActive(false);
        }
    }

    int SetLocation(int i, string s)
    {
        if(i < locations.Length)
		{
			locations[i].gameObject.SetActive(true);
			locations[i].text = s;
        }
        return i + 1;
    }

    private void OnGUI()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
