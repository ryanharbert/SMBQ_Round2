using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Warning : MonoBehaviour
{
    private static Warning _instance;
    
    [SerializeField] TextMeshProUGUI warningText;
    [SerializeField] private float fadeDelay = 0.5f;

    private IEnumerator clearRoutine;

    private void Awake()
    {
        _instance = this;
    }

    public static void Display(string text)
    {
        _instance.StopCoroutine(_instance.clearRoutine);
        //warningText.transform.position = Input.mousePosition;
        
        _instance.warningText.text = text;

        _instance.clearRoutine = _instance.ClearWarningText();
        _instance.StartCoroutine(_instance.clearRoutine);
    }

    IEnumerator ClearWarningText()
    {
        yield return new WaitForSeconds(fadeDelay);
        
        warningText.text = "";
    }
}
