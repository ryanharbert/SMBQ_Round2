using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleportTarget : MonoBehaviour
{
	public string islandId;
	public Text islandName;
	public Image buttonImage;
	public Button button;
	public GameObject lockObject;

	public void TeleportButton()
	{
		TeleportWorldMap.instance.Teleport(islandId);
	}
}
