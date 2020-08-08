using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

namespace SMBQ.Data
{
    public class Currency
    {
        public Currency()
        {
            Value = 0;
        }
        
        public Currency(int initialValue)
        {
            Value = initialValue;
        }

        public Currency(GetPlayerCombinedInfoResultPayload playerInfo, string key)
        {
            int initialValue;
            playerInfo.UserVirtualCurrency.TryGetValue(key, out initialValue);
            Value = initialValue;
        }

        public Currency(List<StatisticValue> statistics, string key)
        {
            int initialValue = 0;
            foreach (StatisticValue s in statistics)
            {
                if (s.StatisticName == key)
                {
                    initialValue = s.Value;
                }
            }
            Value = initialValue;
        }
        
        public int Value { get; private set; }

        public int ChangeValue(int change)
        {
            Value += change;
            if (Value < 0)
            {
                Debug.LogError("A currency should not be reduced below one. This is likely a server client sync error.");
                Value = 0;
            }
            return Value;
        }
    }
}
