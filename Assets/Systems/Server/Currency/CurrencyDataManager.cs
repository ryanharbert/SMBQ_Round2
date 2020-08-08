using System;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

namespace SMBQ.Data
{
	[Serializable]
	public class CurrencyDataManager
	{
		[Header("Currency")]
		public Currency gold;
		public Currency gems;
		public Currency stars;

		[Header("Statistics")]
		public Currency raidPoints;
		public Currency asyncPoints;

		[Header("Rechargeable Currency")]
		public RechargeableCurrency energy;
		public RechargeableCurrency scrolls;

		private const int energyRechargeRate = 3600;
		private const int scrollsRechargeRate = 2880;

		internal void Login(GetPlayerCombinedInfoResultPayload playerInfo)
		{
			if (Data.Instance.offline)
			{
				gold = new Currency(100000);
				gems = new Currency(100000);
				stars = new Currency(100000);
				
				raidPoints = new Currency(0);
				asyncPoints = new Currency(0);
				
				energy = new RechargeableCurrency(1000, energyRechargeRate);
				energy = new RechargeableCurrency(1000, scrollsRechargeRate);
			}
			else
			{
				gold = new Currency(playerInfo, "GO");
				gems = new Currency(playerInfo, "DI");
				stars = new Currency(playerInfo, "SP");
				
				raidPoints = new Currency(playerInfo.PlayerStatistics, "AsyncPoints");
				asyncPoints = new Currency(playerInfo.PlayerStatistics, "RaidPoints");
				
				energy = new RechargeableCurrency(playerInfo,"LB", energyRechargeRate);
				energy = new RechargeableCurrency(playerInfo,"SC", scrollsRechargeRate);
			}
		}

		internal void Update()
		{
			energy.Recharge();
		}
	}
}
