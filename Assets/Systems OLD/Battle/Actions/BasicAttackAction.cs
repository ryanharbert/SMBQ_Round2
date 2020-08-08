using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttackAction : UnitAction
{
    public override void Trigger(Unit u, TargetInfo t, ActionType type, int index)
    {
        t.targetUnit.TakeDamage(new DamageData() { amount = u.attackDamage, source = u, type = DamageType.Melee });
    }
}
