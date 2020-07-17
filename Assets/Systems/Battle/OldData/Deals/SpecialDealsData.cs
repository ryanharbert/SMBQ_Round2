using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

[System.Serializable]
public class SpecialDealsData
{
    public Dictionary<string, List<DealScheduleData>> dealSchedule;
    public Dictionary<string, DealData> deals;
    public bool NewUserPurchased;
    public long NewUserTimeStamp;
    public bool CurrentDealPurchased;
    public string CurrentDealName;

    public int day;
    public int month;
    public string currentDeal;
    public DateTime dealChange;

    public void Login(GetPlayerCombinedInfoResultPayload playerInfo)
    {
        string dealScheduleJson;

        if (playerInfo.TitleData.TryGetValue("DealSchedule", out dealScheduleJson))
        {
            dealSchedule = PlayFabSimpleJson.DeserializeObject<Dictionary<string, List<DealScheduleData>>>(dealScheduleJson);
        }

        string dealsJson;

        if (playerInfo.TitleData.TryGetValue("Deals", out dealsJson))
        {
            deals = PlayFabSimpleJson.DeserializeObject<Dictionary<string, DealData>>(dealsJson);
        }

        UserDataRecord userDataRecord;

        if (playerInfo.UserReadOnlyData.TryGetValue("Deals", out userDataRecord))
        {
            JsonUtility.FromJsonOverwrite(userDataRecord.Value, this);
        }
    }

    public DealData GetCurrentDeal()
    {
        if (!Data.instance.deals.NewUserPurchased && new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(Data.instance.deals.NewUserTimeStamp) > DateTime.UtcNow)
        {
            dealChange = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(Data.instance.deals.NewUserTimeStamp);
            currentDeal = "NewUser";
            return deals[currentDeal];
        }
        else
        {
            month = DateTime.UtcNow.Month;
            day = DateTime.UtcNow.Day;
            List<DealScheduleData> currentMonthDeals = dealSchedule[DateTime.UtcNow.Month.ToString()];

            if (day < currentMonthDeals[0].Start)
            {
                dealChange = new DateTime(DateTime.UtcNow.Year, month, currentMonthDeals[0].Start, 0, 0, 0, DateTimeKind.Utc);
                currentDeal = "";
                return null;
            }
            else if (day >= currentMonthDeals[currentMonthDeals.Count - 1].End || (day >= currentMonthDeals[currentMonthDeals.Count - 1].Start && currentMonthDeals[currentMonthDeals.Count - 1].Deal == CurrentDealName && CurrentDealPurchased))
            {
                dealChange = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.AddMonths(1).Month, dealSchedule[DateTime.UtcNow.AddMonths(1).Month.ToString()][0].Start, 0, 0, 0, DateTimeKind.Utc);
                currentDeal = "";
                return null;
            }
            else
            {
                for (int i = 0; i < currentMonthDeals.Count; i++)
                {
                    if (currentMonthDeals[i].Start <= DateTime.UtcNow.Day && currentMonthDeals[i].End > DateTime.UtcNow.Day)
                    {
                        dealChange = new DateTime(DateTime.UtcNow.Year, month, currentMonthDeals[i].End, 0, 0, 0, DateTimeKind.Utc);
                        currentDeal = currentMonthDeals[i].Deal;
                        if(currentDeal == CurrentDealName && CurrentDealPurchased)
                        {
                            dealSchedule[DateTime.UtcNow.Month.ToString()].RemoveAt(i);
                            return GetCurrentDeal();
                        }
                        else
                        {
                            return IslandValueMult(deals[currentDeal]);
                        }
                    }
                    else if (day >= currentMonthDeals[i].End && day < currentMonthDeals[i + 1].Start)
                    {
                        dealChange = new DateTime(DateTime.UtcNow.Year, month, currentMonthDeals[i + 1].Start, 0, 0, 0, DateTimeKind.Utc);
                        currentDeal = "";
                        return null;
                    }
                }
            }
        }
        return null;
    }

    DealData IslandValueMult(DealData d)
    {
        float m = 1;
        foreach (string s in Data.instance.world.VisitedIslands)
        {
            m = Mathf.Max(m, Data.instance.world.islands[s].DealMult);
        }

        DealData deal = new DealData();
        deal.DisplayName = d.DisplayName;
        deal.Description = d.Description;
        deal.Cards = new DealCardData[d.Cards.Length];
        for(int i = 0; i < deal.Cards.Length; i++)
        {
            deal.Cards[i] = new DealCardData();
            deal.Cards[i].Name = d.Cards[i].Name;
            deal.Cards[i].Amount = Mathf.RoundToInt(m * d.Cards[i].Amount);
        }
        deal.Gold = Mathf.RoundToInt(m * d.Gold);
        deal.Stamina = d.Stamina;
        deal.Scrolls = d.Scrolls;
        deal.Price = d.Price;

        return deal;
    }
}
