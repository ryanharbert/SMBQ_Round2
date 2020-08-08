using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour {

	public static Warning instance;
	
	public Text warningText;

	private void Awake()
	{
		instance = this;
	}

	public void Activate(string text)
	{
		CancelInvoke();
		warningText.text = text;
		warningText.enabled = true;
		Invoke("Disable", 2f);
	}

	public void Disable()
	{
		warningText.enabled = false;
	}
}
