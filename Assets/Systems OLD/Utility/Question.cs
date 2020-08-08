using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Question : MonoBehaviour {

    public static Question instance;

	public GameObject displayObject;

    public Text headerText;
    public Text questionText;

    public Button yesButton;
    public Button noButton;

    private void Awake()
    {
        instance = this;
    }

    public void SetQuestion(string header, string question, UnityAction action)
    {
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(action);
        yesButton.onClick.AddListener(YesButton);
        headerText.text = header;
        questionText.text = question;

        displayObject.SetActive(true);
    }

    public void YesButton()
    {
        displayObject.SetActive(false);
    }
}
