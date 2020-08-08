using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldProgressArrow : MonoBehaviour
{
	private void Update()
	{
	//	if(WorldManager.instance.nodes == null || WorldManager.instance.nodes.Count == 0)
	//	{
	//		return;
	//	}
		
	//	int zoneID = Data.instance.world.GetCurrentZoneID();
	//	ZoneData z = Data.instance.world.zones[zoneID];
	//	string node = z.Last;
	//	if(node == Data.instance.world.CurrentPlayerNode || node == Data.instance.world.PreviousPlayerNode)
	//	{
	//		if(zoneID + 1 < Data.instance.world.zones.Length)
	//		{
	//			string newNode = Data.instance.world.zones[zoneID + 1].Last;
	//			if(WorldManager.instance.nodes.ContainsKey(newNode))
	//			{
	//				node = newNode;
	//			}
	//		}
	//	}

 //       Node targetNode;
 //       if(WorldManager.instance.nodes.TryGetValue(node, out targetNode))
 //       {
 //           Vector3 targetPosLocal = Camera.main.transform.InverseTransformPoint(targetNode.transform.position);
 //           float targetAngle = -Mathf.Atan2(targetPosLocal.x, targetPosLocal.y) * Mathf.Rad2Deg - 90;
 //           transform.localEulerAngles = new Vector3(0, 0, targetAngle);
 //       }
	}


}
