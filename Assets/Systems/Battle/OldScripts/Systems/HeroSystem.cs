using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSystem : BaseSystem {

	public override void Init(BattleState s)
	{
        if(Data.instance.battle.enemyName != "Tutorial")
        {
            s.heroCD = 15;
        }
        else
        {
            s.heroCD = 5;
        }
	}

	public override void Run(BattleState s)
	{
		if(s.heroCD > 0)
		{
			s.heroCD -= Time.deltaTime;
		}
	}
}
