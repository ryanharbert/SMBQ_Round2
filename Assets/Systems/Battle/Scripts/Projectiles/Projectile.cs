using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public GameObject displayObject;
	public bool lob = false;
	public float lobHeight;

	//Data
	public float speed;
	public GameObject impactDisplayObject;

	//State
	[HideInInspector] public float range;
	[HideInInspector] public Unit enemy;
	[HideInInspector] public Unit source;
	[HideInInspector] public int damage;
	[HideInInspector] public float sourceHeight = 1;
	[HideInInspector] public float enemyHeight = 1;
	[HideInInspector] public Vector3 sourcePos;
	[HideInInspector] public Vector3 enemyPos;
    [HideInInspector] public int teamID;

    public void Impact()
	{
        if(Photon.Pun.PhotonNetwork.IsMasterClient)
        {
            DealDamage();
        }

		DisplayImpact();
	}

	protected virtual void DealDamage()
	{
        enemy.TakeDamage(new DamageData() { amount = damage, source = null, type = DamageType.Ranged });
    }

	void DisplayImpact()
	{
		if (impactDisplayObject != null)
		{
			GameObject display = Instantiate(impactDisplayObject, displayObject.transform.position, Quaternion.identity);
			Destroy(display, 2f);
		}
	}
}