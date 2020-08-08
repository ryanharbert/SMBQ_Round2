using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Que : MonoBehaviour
{
    public static Que instance;

    public Text searchDesc;

    public GameObject displayObject;

    private void Awake()
    {
        instance = this;
    }

    public void LeaveQue()
    {
        PunManager.instance.LeaveQue();
    }

}
