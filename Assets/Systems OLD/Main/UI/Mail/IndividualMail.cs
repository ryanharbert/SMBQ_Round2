using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class IndividualMail : MonoBehaviour
{
	public Text title;
	public Text desc;
	public GameObject newMail;
	public Button openButton;
	public Button deleteButton;

	int index;
    Mail mail;

	public void Set(MailData data, int index, Mail mail)
	{
		this.index = index;
        this.mail = mail;

		title.text = data.Title;
		desc.text = data.Desc;
		newMail.SetActive(data.New);

		if(data.Preset != null || data.Chest != null)
		{
			deleteButton.gameObject.SetActive(false);
			openButton.gameObject.SetActive(true);
		}
		else
		{
			deleteButton.gameObject.SetActive(true);
			openButton.gameObject.SetActive(false);
		}
	}

	public void OpenMail()
	{
		openButton.gameObject.SetActive(false);
        ChestLootDisplay.instance.ChestOpening();
        Data.instance.mail.OpenMail(index, OpenCallback);
    }

	public void DeleteMail()
	{
		deleteButton.gameObject.SetActive(false);
        Data.instance.mail.DeleteMail(index, DeleteCallback);
	}

	void OpenCallback()
	{
        mail.SetMailDisplay();
	}

	void DeleteCallback()
	{
        mail.SetMailDisplay();
        mail.HandleNewMail();
	}
}
