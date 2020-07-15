using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BeamAction : UnitAction
{
    public float tickRate;
    public Transform hitBox;
    public int tickDamage;
    public bool hitFlying;

    //Display
    public GameObject displayObject;
    public Transform hitAreaDisplay;

    protected float tickTimer;

    private void Start()
    {
        displayObject.SetActive(false);
    }

    protected void Update()
    {
        if(hitAreaDisplay != null)
        {
            Vector3 lookPos = Battle.state.spawnPosition - transform.position;
            lookPos.y = 0;
            if (lookPos != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                hitAreaDisplay.rotation = Quaternion.Slerp(hitAreaDisplay.rotation, rotation, 1f);
            }
        }
    }

    public override void Execute(Unit u, TargetInfo t, int i)
    {
        tickTimer = 0;
        base.Execute(u, t, i);
    }

    public override void Effect(Unit u)
    {
        if (durationTimer > duration)
        {
            u.photonView.RPC("AbilityDisplayBool", RpcTarget.All, ActionType.Ability, index, false);
        }

        if (!triggered)
        {
            triggered = true;
            u.photonView.RPC("AbilityDisplayBool", RpcTarget.All, ActionType.Ability, index, true);
        }

        tickTimer += Time.deltaTime;

        if (tickTimer > tickRate)
        {
            tickTimer = 0;
            
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
                
                e.TakeDamage(new DamageData() { amount = u.LevelandTypeBonus(tickDamage), source = u, type = DamageType.Ranged });
            }
        }
    }

    public override void BoolDisplay(bool active)
    {
        displayObject.SetActive(active);
    }
}
