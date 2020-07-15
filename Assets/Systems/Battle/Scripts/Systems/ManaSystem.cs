using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaSystem : BaseSystem
{
	public override void Init(BattleState s)
	{
		s.maxMana = 10;

		s.currentMana = 7;
		s.manaProgress = 0;
		s.manaRate = 0.4f;
        if(Battle.instance.battleType == BattleType.World && s.enemyLevel < 3)
        {
            s.enemyManaRate = 0.3f;
        }
        else if (Battle.instance.battleType == BattleType.World && s.enemyLevel < 5)
        {
            s.enemyManaRate = 0.37f;
        }
        else
        {
            s.enemyManaRate = s.manaRate;
        }
}

	public override void Run(BattleState s)
	{
		if(s.manaProgress < 1)
		{
			s.manaProgress += Time.deltaTime * s.manaRate;
		}
		if(s.manaProgress >= 1 && s.currentMana < s.maxMana)
		{
			s.currentMana++;
			s.manaProgress = 0;
		}
	}
}
