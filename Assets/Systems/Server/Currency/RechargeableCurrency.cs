using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Serialization;
using PlayFab.ClientModels;

namespace SMBQ.Data
{
    [Serializable]
    public class RechargeableCurrency : Currency
    {
        readonly int max;
        readonly float startingSec;
        readonly DateTime timeStamp;
        readonly int rechargeRate;
        
        float secondsToRecharge;
        int increases;
        
        public RechargeableCurrency(int initialValue, int rechargeRate) : base(initialValue)
        {
            max = 10;
            startingSec = 0;
            timeStamp = DateTime.UtcNow;
            this.rechargeRate = rechargeRate;
        }

        public RechargeableCurrency(GetPlayerCombinedInfoResultPayload playerInfo, string key, int rechargeRate) : base(playerInfo, key)
        {
            ChangeValue(-1);
            VirtualCurrencyRechargeTime rechargeTime;
            playerInfo.UserVirtualCurrencyRechargeTimes.TryGetValue("LB", out rechargeTime);
            max = rechargeTime.RechargeMax - 1;
            startingSec = rechargeRate - rechargeTime.SecondsToRecharge;
            timeStamp = DateTime.UtcNow;
            this.rechargeRate = rechargeRate;
        }

        internal void Recharge()
        {
            if (Value < max)
            {
                TimeSpan t = DateTime.UtcNow - timeStamp;
                float f = startingSec + (float) t.TotalSeconds;
                if (Mathf.FloorToInt(f / rechargeRate) > increases)
                {
                    ChangeValue(1);
                    increases++;
                }

                secondsToRecharge = Mathf.CeilToInt(rechargeRate - (f % rechargeRate));
            }
        }

        public string TimerText()
        {
            if (Value < max)
            {
                string secondsToRechargeText = "";
                int minutes = Mathf.FloorToInt(secondsToRecharge / 60);
                int seconds = Mathf.CeilToInt(secondsToRecharge % 60);

                if (seconds == 60)
                {
                    seconds = 0;
                    minutes += 1;
                }

                if (secondsToRecharge > 59)
                {
                    secondsToRechargeText = minutes + "m " + seconds + "s";
                }
                else
                {
                    secondsToRechargeText = seconds + "s";
                }

                return secondsToRechargeText;
            }
            else
            {
                return string.Empty;
            }
        }

        public string RatioText()
        {
            return Value + " / " + max;
        }
    }
}
