using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEProjectile : Projectile
{
	public float radius;
	public bool hitFlying;
    public bool damageEnemy;
    public bool healAlly;
    public UnitAction enemyAction;
    public UnitAction friendlyAction;

	protected override void DealDamage()
	{
		Collider[] c = Physics.OverlapSphere(transform.position, radius);

		for(int i = 0; i < c.Length; i++)
		{
			Unit e = c[i].GetComponent<Unit>();
			if(e == null)
				continue;

            if (!hitFlying && e.flying)
                continue;

            if (e.teamID != teamID && damageEnemy)
            {
                if(enemyAction == null)
                {
                    e.TakeDamage(new DamageData() { amount = damage, source = null, type = DamageType.Ranged });
                }
                else
                {
                    enemyAction.Trigger(e, new TargetInfo() { targetUnit = e, position = e.transform.position }, ActionType.Projectile, 0);
                }
            }
            else if(healAlly)
            {
                if(friendlyAction != null)
                {
                    friendlyAction.Trigger(e, new TargetInfo() { targetUnit = e, position = e.transform.position }, ActionType.Projectile, 0);
                }
                else
                {
                    if (e.health > 0 && (e.health < e.maxHealth || damage < 0))
                    {
                        e.health += damage;
                    }
                }
            }

        }
	}
}
