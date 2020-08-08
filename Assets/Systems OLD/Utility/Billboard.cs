using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
    Camera cam;

	Canvas canvas;

	private void Awake()
	{
        cam = Camera.main;
		if (cam != null)
		{
			transform.rotation = cam.transform.rotation;
		}

		canvas = GetComponent<Canvas>();
		if (canvas != null && cam != null)
		{
			canvas.worldCamera = cam;
		}
	}

	private void Update()
	{
		if (cam != null)
		{
			transform.rotation = cam.transform.rotation;
		}
        else
        {
            cam = Camera.main;
        }
	}
}
