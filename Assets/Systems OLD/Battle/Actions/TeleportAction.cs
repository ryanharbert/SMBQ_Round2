using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TeleportAction : UnitAction
{
    public GameObject displayObject;

    public override void Trigger(Unit u, TargetInfo t, ActionType type, int index)
    {
        u.photonView.RPC("AbilityDisplayPosition", RpcTarget.All, type, index, transform.position);

        transform.position = t.position;
        u.nav.Warp(t.position);
        
        u.photonView.RPC("AbilityDisplayPosition", RpcTarget.All, type, index, transform.position);
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

        if (!Photon.Pun.PhotonNetwork.IsMasterClient)
        {
            Unit u = GetComponent<Unit>();
			u.transform.position = position;
			u.nav.Warp(position);
			u.masterPosition = position;
        }
    }
}
