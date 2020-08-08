using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNode : Node
{
	public string battleScene;
    public bool roam = true;
    public int roamRadius = 5;
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, roamRadius);
    }
}
