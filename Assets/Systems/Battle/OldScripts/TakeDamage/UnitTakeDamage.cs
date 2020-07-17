using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitTakeDamage
{
    public bool takeDamage;
    public bool typeRestricted;
    public DamageType damageType;
    public UnitAction action;
    
    public void Execute(Unit u, DamageData damage)
    {
        if(!typeRestricted || damageType == damage.type)
        {
            TargetInfo t = new TargetInfo();
            t.targetUnit = damage.source;
            if (t.targetUnit != null)
            {
                t.position = t.targetUnit.transform.position;
            }
            action.Trigger(u, t, ActionType.OnTakeDamage, 0);
            if (takeDamage)
            {
                u.health -= damage.amount;
            }
        }
        else
        {
            u.health -= damage.amount;
        }
    }
}
