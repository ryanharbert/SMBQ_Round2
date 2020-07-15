using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AOEAction : UnitAction
{
    public bool onSelf;
    public bool useAttackDamage = true;
    public int amount;
    public float radius;
    public bool hitFlying;
	public bool lifeSteal;
    public GameObject displayObject;

    public override void Trigger(Unit u, TargetInfo t, ActionType type, int index)
    {
        Vector3 targetLocation;
        if(!onSelf)
        {
            targetLocation = t.targetUnit.transform.position;
            RaycastHit hit;
            Vector3 rayDirection = ((t.targetUnit.transform.position - u.transform.position).normalized);
            Ray ray = new Ray(u.transform.position, rayDirection);

            if (t.targetUnit.capsule != null && t.targetUnit.capsule.Raycast(ray, out hit, 10f))
            {
                targetLocation = hit.point;
            }
        }
        else
        {
            targetLocation = u.transform.position;
        }
        if(displayObject != null)
        {
            u.photonView.RPC("AbilityDisplayPosition", RpcTarget.All, type, index, targetLocation);
        }
        Collider[] c = Physics.OverlapSphere(targetLocation, radius);

        for (int i = 0; i < c.Length; i++)
        {
            Unit e = c[i].GetComponent<Unit>();
            if (e == null)
                continue;

            if (e.teamID == u.teamID)
                continue;

            if (!hitFlying && e.flying)
                continue;

            if (useAttackDamage)
            {
                e.TakeDamage(new DamageData() { amount = u.attackDamage, source = u, type = DamageType.Ranged });
				if(lifeSteal)
				{
					u.health += u.attackDamage;
					if (u.health > u.maxHealth)
					{
						u.health = u.maxHealth;
					}
				}
			}
            else
            {
                e.TakeDamage(new DamageData() { amount = u.LevelandTypeBonus(amount), source = u, type = DamageType.Ranged });
				if (lifeSteal)
				{
					u.health += u.LevelandTypeBonus(amount);
					if (u.health > u.maxHealth)
					{
						u.health = u.maxHealth;
					}
				}
			}
        }
    }
    
    public override void PositionDisplay(Vector3 position)
    {
        if (displayObject != null)
        {
            GameObject display = Instantiate(displayObject);
            display.transform.position = position;
            display.transform.rotation = Quaternion.identity;
            Destroy(display, 2f);
        }
    }
}
