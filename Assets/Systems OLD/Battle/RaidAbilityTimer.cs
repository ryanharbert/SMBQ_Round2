using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaidAbilityTimer : MonoBehaviour
{
    public Text timerText;
    public Unit u;
    public int index;
    
    public Color textColor;
    
	void Start ()
    {
        timerText.color = textColor;
        timerText.text = "";
    }

    private void Update()
    {
        timerText.text = Mathf.CeilToInt(u.abilities[index].CDTimer) + " sec";
    }
}
