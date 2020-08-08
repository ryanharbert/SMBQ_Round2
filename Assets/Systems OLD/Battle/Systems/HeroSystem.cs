using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSystem : BaseSystem {

	public override void Init(BattleState s)
	{
        if(Data.instance.battle.enemyName != "Tutorial")
        {
            s.heroCD[0] = 15;
            s.heroCD[1] = 15;
        }
        else
        {
            s.heroCD[0] = 5;
            s.heroCD[1] = 5;
        }
	}

	public override void Run(BattleState s)
	{
        for (int i = 0; i < s.heroCD.Length; i++)
        {
            if (s.heroCD[i] > 0)
            {
                s.heroCD[i] -= Time.deltaTime;
            }
        }
	}
}
