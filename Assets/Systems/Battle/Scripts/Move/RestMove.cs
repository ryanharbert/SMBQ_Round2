using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RestMove : UnitMove
{
    public bool resting;
    public GameObject displayObject;
    public UnitAction healAction;
    public int tickRate;

    float tickTimer;

    private void Awake()
    {
        resting = false;
        displayObject.SetActive(false);
    }

    public override void Execute(BattleState s, Unit u)
    {
        if(u.targetInfo.distance > (u.aggroRange + u.radius) && u.health < u.maxHealth)
        {
            if(!resting)
            {
                tickTimer = 0;
                resting = true;
                u.nav.SetDestination(u.transform.position);
                u.photonView.RPC("DisplayRest", RpcTarget.All, true);
            }

            tickTimer -= Time.deltaTime;

            if(tickTimer < 0)
            {
                healAction.Trigger(u, null, ActionType.Ability, 0);
                tickTimer = tickRate;
            }
        }
        else
        {
            if (resting)
            {
                resting = false;
                u.photonView.RPC("DisplayRest", RpcTarget.All, false);
            }
            base.Execute(s, u);
        }
    }

    public override void Exit(BattleState s, Unit u)
    {
        if(resting)
        {
            resting = false;
            u.photonView.RPC("DisplayRest", RpcTarget.All, false);
        }
    }

    [PunRPC]
    void DisplayRest(bool rest)
    {
        if(rest)
        {
            displayObject.SetActive(true);
            GetComponent<Unit>().anim.SetTrigger("Rest");
        }
        else
        {
            displayObject.SetActive(false);
        }
    }
}
