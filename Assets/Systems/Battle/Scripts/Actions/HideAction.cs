using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HideAction : UnitAction
{
    bool hiding = false;

    public override void Trigger(Unit u, TargetInfo t, ActionType type, int index)
    {
        if(!hiding)
        {
            hiding = true;
            u.nav.isStopped = true;
            Battle.state.units.Remove(u);

            u.onTakeDamage.action = u.gameObject.AddComponent<ShieldAction>();
            ((ShieldAction)u.onTakeDamage.action).uses = -1;
            if(index == 0)
            {
                u.abilities[0].CDTimer = -1;
                u.abilities[1].disabled = true;
            }
            else
            {
                u.abilities[1].CDTimer = -1;
                u.abilities[0].disabled = true;
            }
            u.photonView.RPC("TriggerAnimationRPC", Photon.Pun.RpcTarget.All, index + 4);
        }
        else
        {
            hiding = false;
            Destroy(u.onTakeDamage.action);
            if (index == 0)
            {
                u.abilities[1].disabled = false;
            }
            else
            {
                u.abilities[0].disabled = false;
            }

            Battle.state.units.Add(u);
            u.state = 0;
            //u.photonView.RPC("GetUp", Photon.Pun.RpcTarget.All);
            //Invoke("DelayedGetUp", 0.85f);
        }
    }

    //[PunRPC]
    //public void GetUp()
    //{
    //    GetComponent<Unit>().anim.SetTrigger("GetUp");
    //}

    //void DelayedGetUp()
    //{
    //    Unit u = GetComponent<Unit>();
    //    hiding = false;
    //    Destroy(u.onTakeDamage.action);
    //    if (index == 0)
    //    {
    //        u.abilities[1].disabled = false;
    //    }
    //    else
    //    {
    //        u.abilities[0].disabled = false;
    //    }

    //    Battle.state.units.Add(u);
    //    u.state = 0;
    //}
}
