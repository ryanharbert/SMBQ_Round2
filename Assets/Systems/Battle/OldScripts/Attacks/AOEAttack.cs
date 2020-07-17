using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AOEAttack : BasicAttack
{
	public float radius;
	public bool hitFlying;
	public GameObject displayObject;

	protected override void DealDamage(BattleState s, Unit u)
    {
        RaycastHit hit;
        Vector3 rayDirection = ((u.targetInfo.targetUnit.transform.position - u.transform.position).normalized);
        Ray ray = new Ray(u.transform.position, rayDirection);

        Vector3 targetLocation = u.targetInfo.targetUnit.transform.position;
        if (u.targetInfo.targetUnit.capsule != null && u.targetInfo.targetUnit.capsule.Raycast(ray, out hit, 10f))
        {
            targetLocation = hit.point;
        }

        u.photonView.RPC("DisplayAOE", RpcTarget.All, targetLocation);
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
            
            //e.TakeDamage(u.attackDamage);
		}
	}

    [PunRPC]
    void DisplayAOE(Vector3 position)
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
