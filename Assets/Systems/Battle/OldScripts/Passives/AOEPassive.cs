using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEPassive : PassiveType
{
    public bool hitFlying = false;
    public bool hitBuilding = false;
    public int damage;
    public float radius;

    public override void Use(BattleState s, Unit u)
    {
        base.Use(s, u);

        Collider[] c = Physics.OverlapSphere(u.transform.position, radius);

        for (int i = 0; i < c.Length; i++)
        {
            Unit e = c[i].GetComponent<Unit>();
            if (e == null)
                continue;

            if (e.teamID == u.teamID)
                continue;

            if (!hitFlying && e.flying)
                continue;

            if (e.type == UnitType.Building)
                continue;
            
            //e.TakeDamage(u.LevelandTypeBonus(damage));
        }
    }
}
