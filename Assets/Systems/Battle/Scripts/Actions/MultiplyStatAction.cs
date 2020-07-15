using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiplyStatAction : UnitAction
{
    public bool increase;
    public bool damage;
    public float damageMult;
    public bool health;
    public float healthMult;

    public override void Trigger(Unit u, TargetInfo t, ActionType type, int index)
    {
        if (increase)
        {
            if(damage)
            {
                u.attackDamage = Mathf.RoundToInt(u.attackDamage * damageMult);
            }

            if(health)
            {
                u.health = Mathf.RoundToInt(u.health * healthMult);
            }
        }
        else
        {
            if (damage)
            {
                u.attackDamage = Mathf.RoundToInt(u.attackDamage / damageMult);
            }

            if (health)
            {
                u.health = Mathf.RoundToInt(u.health / healthMult);
            }
        }
    }
}
