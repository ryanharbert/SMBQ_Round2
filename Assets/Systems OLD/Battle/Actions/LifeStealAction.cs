using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeStealAction : UnitAction
{
    public int healPerc;

    public override void Trigger(Unit u, TargetInfo t, ActionType type, int index)
    {
        t.targetUnit.TakeDamage(new DamageData() { amount = u.attackDamage, source = u, type = DamageType.Melee });
        u.health += Mathf.RoundToInt(u.attackDamage * healPerc * 0.01f);
        if(u.health > u.maxHealth)
        {
            u.health = u.maxHealth;
        }
    }
}
