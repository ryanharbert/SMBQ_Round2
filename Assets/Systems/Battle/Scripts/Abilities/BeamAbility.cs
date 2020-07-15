using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BeamAbility : AbilityType {

	public float tickRate;
	public Transform hitBox;
	public int tickDamage;
	public bool hitFlying;

	//Display
	public GameObject displayObject;
    public Transform hitAreaDisplay;

	protected float secondaryTimer;

    private void Start()
    {
        displayObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
        Vector3 lookPos = Battle.state.spawnPosition - transform.position;
        lookPos.y = 0;
        if(lookPos != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            hitAreaDisplay.rotation = Quaternion.Slerp(hitAreaDisplay.rotation, rotation, 1f);
        }
    }
    
	public override void StartCast(Vector3 abilityPosition)
	{
		secondaryTimer = 0;
		base.StartCast(abilityPosition);
	}

	public override void Effect()
	{
		if (durationTimer > castDuration)
        {
			AbilityDisplay(false);
        }
        
		if (!triggered)
		{
			triggered = true;
			AbilityDisplay(true);
		}

		secondaryTimer += Time.deltaTime;

		if(secondaryTimer > tickRate)
		{
			secondaryTimer = 0;

			if(PhotonNetwork.IsMasterClient)
			{
				Collider[] c = Physics.OverlapBox(hitBox.position, new Vector3(hitBox.localScale.x, 2f, hitBox.localScale.z) / 2, u.transform.rotation);

				for (int i = 0; i < c.Length; i++)
				{
					Unit e = c[i].GetComponent<Unit>();
					if (e == null)
						continue;

					if (e.teamID == u.teamID)
						continue;

					if (!hitFlying && e.flying)
						continue;
                    
                    //e.TakeDamage(u.LevelandTypeBonus(tickDamage));
                }
			}
		}
	}
	
    public void AbilityDisplay(bool active)
    {
        displayObject.SetActive(active);
    }

    protected override bool AfterDistanceDecision(BattleState s, Unit u, TargetInfo t, int index)
    {
        ((HeroUnit)u).photonView.RPC("Cast", RpcTarget.All, t.targetUnit.transform.position, index);
        return true;
    }
}
