using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChargeMove : UnitMove
{
    public GameObject displayObject;
    public float chargeDelay;

    [HideInInspector] public bool charging;
    [HideInInspector] public float chargeTimer;

    public override void Execute(BattleState s, Unit u)
    {
        if (!charging)
        {
            if (chargeDelay < chargeTimer)
            {
                u.photonView.RPC("DisplayCharge", RpcTarget.All, true);
                charging = true;
                u.nav.speed = 5f;
            }
            else
            {
                chargeTimer += Time.deltaTime;
            }
        }

        base.Execute(s, u);
    }

    public override void Exit(BattleState s, Unit u)
    {
        if(chargeTimer != 0)
        {
            chargeTimer = 0;
        }

        if (charging)
        {
            u.photonView.RPC("DisplayCharge", RpcTarget.All, false);
            charging = false;
            u.nav.speed = 3.4f;
        }
    }

    [PunRPC]
    void DisplayCharge(bool active)
    {
        displayObject.SetActive(active);
    }
}
