using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class Mail : MonoBehaviour
{
	public List<IndividualMail> mailList;

	public Text newMailCountText;
	public Text currentPage;

	public GameObject pagingButtons;

	int pageIndex = 0;

    private void Start()
    {
        Data.instance.mail.newMailReceived += SetMailDisplay;
    }

    private void OnDestroy()
    {
        Data.instance.mail.newMailReceived -= SetMailDisplay;
    }

    private void OnEnable()
	{
		SetMailDisplay();
		HandleNewMail();
		NavBar.instance.mail.notificationGO.SetActive(false);
	}

	private void OnDisable()
	{
		foreach (MailData m in Data.instance.mail.mailList)
		{
			if (m.New)
			{
				m.New = false;
			}
		}
	}

	public void SetMailDisplay()
	{
		int constant = pageIndex * 10;
		for(int i = 0; i < mailList.Count; i++)
		{
			if(Data.instance.mail.mailList.Count > (i + constant))
			{
				mailList[i].gameObject.SetActive(true);
				mailList[i].Set(Data.instance.mail.mailList[Data.instance.mail.mailList.Count - 1 - i - constant], Data.instance.mail.mailList.Count - 1 - i - constant, this);
			}
			else
			{
				mailList[i].gameObject.SetActive(false);
			}
		}
		
		if(Data.instance.mail.mailList.Count < 11)
		{
			pagingButtons.SetActive(false);
			if(Data.instance.mail.mailList.Count < 1)
			{
				currentPage.text = "No Messages";
			}
			else if(Data.instance.mail.mailList.Count == 1)
			{
				currentPage.text = Data.instance.mail.mailList.Count + " Message";
			}
			else
			{
				currentPage.text = Data.instance.mail.mailList.Count + " Messages";
			}
		}
		else
		{
			pagingButtons.SetActive(true);
			currentPage.text = (constant + 1) + " - " + Mathf.Min(Data.instance.mail.mailList.Count, (constant + 10)) + "  (" + Data.instance.mail.mailList.Count + " Total)";
		}
	}

	public void HandleNewMail()
	{
        int newMailCount = Data.instance.mail.newMailCount;
		if (newMailCount > 0)
		{
			newMailCountText.text = newMailCount + " New Message";
			if (newMailCount > 1)
			{
				newMailCountText.text += "s";
			}

            Data.instance.mail.MarkMailRead();
		}
		else
		{
			newMailCountText.text = "";
		}
	}

	public void PageLeft()
	{
		pageIndex++;
		if((pageIndex * 10 + 1) > Data.instance.mail.mailList.Count)
		{
			pageIndex = 0;
		}
		SetMailDisplay();
	}

	public void PageRight()
	{
		pageIndex--;
		if (pageIndex < 0)
		{
			pageIndex = Mathf.FloorToInt((Data.instance.mail.mailList.Count - 1) / 10f);
		}
		SetMailDisplay();
	}

}
