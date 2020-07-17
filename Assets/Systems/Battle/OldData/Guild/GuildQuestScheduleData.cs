using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GuildQuestScheduleData
{
	public string Target;
	public int Start;
	public int End;
	public int[] Amounts;
	public int[] Rewards;
}
