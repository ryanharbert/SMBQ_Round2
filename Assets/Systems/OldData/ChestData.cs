using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChestData
{
	public string displayName;
	public int price;
	public int amount;
	public int gold;
	public List<string> pool;
	public List<string> jackpotPool;
	public int jackpotChance;
    public int starChance;
    public int starMin;
    public int starMax;
}
