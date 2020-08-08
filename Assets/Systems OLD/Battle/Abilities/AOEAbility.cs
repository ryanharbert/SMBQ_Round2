using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AOEAbility : AbilityType
{
	public float radius;
	public bool hitFlying;
	public int damage;

	//Display
	public GameObject particleEffect;

	public override void Effect()
	{
		if (!triggered)
		{
			if(PhotonNetwork.IsMasterClient)
			{
				Collider[] c = Physics.OverlapSphere(position, radius);

				for (int i = 0; i < c.Length; i++)
				{
					Unit e = c[i].GetComponent<Unit>();
					if (e == null)
						continue;

					if (e.teamID == u.teamID)
						continue;

					if (!hitFlying && e.flying)
						continue;

                    //e.TakeDamage(u.LevelandTypeBonus(damage));
                }
			}

			triggered = true;
            
			DisplayAbility(position);
        }
    }
	
    void DisplayAbility(Vector3 abilityPosition)
	{
		if (particleEffect != null)
		{
			GameObject original = Instantiate(particleEffect, abilityPosition, Quaternion.identity);
			Destroy(original, 4f);
		}
    }

    protected override bool AfterDistanceDecision(BattleState s, Unit u, TargetInfo t, int index)
    {
        if (validPlayArea == ValidPlayArea.OnClick)
        {
            ((HeroUnit)u).photonView.RPC("Cast", RpcTarget.All, u.transform.position, index);
        }
        else
        {
            ((HeroUnit)u).photonView.RPC("Cast", RpcTarget.All, t.targetUnit.transform.position, index);
        }
        return true;
    }
}
