using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class PassiveType : MonoBehaviour
{
	public int cooldown;

    protected Unit u;

    protected virtual void Awake()
    {
        u = GetComponent<Unit>();
    }

    private void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            UseOrUpdateTime();
        }
    }

    public void UseOrUpdateTime()
	{
	}

	public virtual void Use(BattleState s, Unit u)
	{
		if (u.type == UnitType.Building && u.passiveAnim != null)
        {
            u.photonView.RPC("TriggerAnimationRPC", Photon.Pun.RpcTarget.All, 3);
        }
	}
}
