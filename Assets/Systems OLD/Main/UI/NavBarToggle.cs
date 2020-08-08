using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavBarToggle : MonoBehaviour {

	public Toggle toggle;
	public Text toggleText;
	public Image buttonImage;
	public Image toggleIcon;
	public GameObject uiPage;
	public string uiHeader;
    public GameObject notificationGO;
    public Text notificationCountText;
	public int level;
	public bool locked;

    public void Toggle(bool active)
    {
        if (active)
        {
            toggleText.color = NavBar.instance.highlightColor;
            toggleText.fontSize = 55;
        }
        else
        {
            toggleText.color = NavBar.instance.normalColor;
            toggleText.fontSize = 50;
        }
    }
}
