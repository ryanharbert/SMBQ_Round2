using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAttackAction : UnitAction
{
    public float damageMult;

    public override void Channel(BattleState s, Unit u)
    {
        if(!triggered && u.targetInfo.targetUnit != null && !UnitSystem.CurrentEnemyInRange(Battle.state, u, u.targetInfo.targetUnit))
        {
            u.transform.position = Vector3.MoveTowards(u.transform.position, u.targetInfo.targetUnit.transform.position, Time.deltaTime * 5);
        }

        base.Channel(s, u);
    }

    public override void Trigger(Unit u, TargetInfo t, ActionType type, int index)
    {
        t.targetUnit.TakeDamage(new DamageData() { amount = Mathf.RoundToInt(u.attackDamage * damageMult), source = u, type = DamageType.Melee });
    }
}
