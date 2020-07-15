using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDecayPassive : PassiveType
{
    public int percLoss;

    public override void Use(BattleState s, Unit u)
    {
        base.Use(s, u);
        
        u.health -= Mathf.RoundToInt(u.maxHealth * percLoss * 0.01f);
    }
}
