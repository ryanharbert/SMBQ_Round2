using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreantHeal : MonoBehaviour {

	public Unit u;
	public GameObject buffDisplay;

	private void Update()
	{
		if((u.health >= u.maxHealth || u.health <= 0) && buffDisplay.activeSelf)
		{
			buffDisplay.SetActive(false);
		}
		else if(!buffDisplay.activeSelf)
		{
			buffDisplay.SetActive(true);
		}
	}
}
