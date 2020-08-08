using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavBar : MonoBehaviour
{
    public static NavBar instance;

	public LookZoomOut zoomOut;

    public ToggleGroup navToggleGroup;

	public Button world;

	public NavBarToggle deck;
	public NavBarToggle shop;
    public NavBarToggle quest;
	public NavBarToggle guild;
	public NavBarToggle pvp;
	public NavBarToggle raid;
	public NavBarToggle chat;
	public NavBarToggle mail;
	public NavBarToggle settings;
    public NavBarToggle teleport;

	public NavBarToggle[] toggles;

	public GameObject popUpObject;
	public Text popUpHeaderText;

	public Color highlightColor;
	public Color normalColor;
	public Color levelLockedColor;
	public int highlightSize;
	public int normalSize;

	public void Start()
	{
        instance = this;

		EnableNavBar ();
		popUpObject.SetActive(false);
        SetNotifications();

        Data.instance.mail.newMailReceived += SetMailNotification;
    }

    private void OnDestroy()
    {
        Data.instance.mail.newMailReceived -= SetMailNotification;
    }

    void GameObjectToggle(NavBarToggle navBarToggle, bool on)
	{
		if (!navBarToggle.locked)
		{
			if (on)
			{
				navBarToggle.uiPage.SetActive(true);
				popUpHeaderText.text = navBarToggle.uiHeader.ToUpper();
				popUpObject.SetActive(true);
			}
			else
			{
				if (!navToggleGroup.AnyTogglesOn())
				{
					popUpObject.SetActive(false);
				}

				navBarToggle.uiPage.SetActive(false);
			}
		}
		else if(on)
		{
			Warning.instance.Activate(navBarToggle.toggleText.text + " unlocks at level " + navBarToggle.level);
			popUpObject.SetActive(false);
		}
	}

    public void ToggleNavBar(bool on)
    {
        if(on)
        {
            EnableNavBar();
        }
        else
        {
            DisableNavBar();
        }
    }

	void DisableNavBar()
	{
		world.interactable = false;
		foreach(NavBarToggle t in toggles)
		{
			t.toggle.interactable = false;
		}
    }

	void EnableNavBar()
	{
		world.interactable = true;
		foreach (NavBarToggle t in toggles)
		{
			t.toggle.interactable = true;

			if(t.level > Data.instance.currency.playerLevel)
			{
				t.toggleText.color = levelLockedColor;
				t.buttonImage.color = new Color32(255, 255, 255, 130);
				t.locked = true;
			}
			else
			{
				t.locked = false;
			}
		}
    }

	public void CloseButton()
    {
		foreach(Toggle t in navToggleGroup.ActiveToggles())
		{
			t.isOn = false;
		}
    }

	public void World()
	{
		CloseButton();
		zoomOut.LookButton();
    }

	public void Deck(bool on)
	{
		GameObjectToggle(deck, on);
	}

	public void Shop(bool on)
	{
		GameObjectToggle(shop, on);
    }

    public void Quest(bool on)
    {
        GameObjectToggle(quest, on);
    }

    public void Guild(bool on)
	{
		GameObjectToggle(guild, on);
	}

	public void PvP(bool on)
	{
		GameObjectToggle(pvp, on);
	}

	public void Raid(bool on)
	{
		GameObjectToggle(raid, on);
	}

	public void Chat(bool on)
	{
		GameObjectToggle(chat, on);
	}

	public void Mail(bool on)
	{
		GameObjectToggle(mail, on);
	}

	public void Settings(bool on)
	{
		GameObjectToggle(settings, on);
        popUpObject.SetActive(false);
    }

    public void Teleport(bool on)
    {
        GameObjectToggle(teleport, on);
        popUpObject.SetActive(false);
    }

    public void SetNotifications()
    {
        SetDeckNotification();
        SetShopNotification();
        SetQuestNotification();
        SetRaidNotification();
		SetMailNotification();
    }

    public void SetDeckNotification()
    {
        //DECK
        int i = 0;
        foreach (KeyValuePair<string, CardData> c in Data.instance.collection.allCards)
        {
            if (c.Value.Upgradeable && Data.instance.currency.gold >= c.Value.GoldNeeded)
            {
                i++;
            }
        }
        if (i > 0)
        {
            deck.notificationGO.SetActive(true);
            deck.notificationCountText.text = i.ToString();
        }
        else
        {
            deck.notificationGO.SetActive(false);
        }
    }

    public void SetShopNotification()
    {
        if (shop != null)
            return;

        if (!Data.instance.shop.shops["Main"].Purchased[0])
        {
            shop.notificationGO.SetActive(true);
            shop.notificationCountText.text = " !";
        }
        else
        {
            shop.notificationGO.SetActive(false);
        }
    }

    public void SetQuestNotification()
    {
        //QUESTS
        int i = 0;
        foreach (QuestData q in Data.instance.quest.quests)
        {
            if (q.Completed())
            {
                i++;
            }
        }
        if(Data.instance.quest.lastQuestTimeStamp.Day != System.DateTime.UtcNow.Day)
        {
            quest.notificationGO.SetActive(true);
            quest.notificationCountText.text = " !";
        }
        else if (i > 0)
        {
            quest.notificationGO.SetActive(true);
            quest.notificationCountText.text = i.ToString();
        }
        else
        {
            quest.notificationGO.SetActive(false);
        }
    }

    public void SetRaidNotification()
    {
        //RAID
        bool chestUnlocked = false;
        bool slotsEmpty = true;
        bool chestUnlocking = false;
        foreach (ChestSlotData c in Data.instance.raids.chestSlots)
        {
            if (c.TimeStamp != 0 && (c.dateTime - System.DateTime.UtcNow).TotalSeconds < 0)
            {
                chestUnlocked = true;
            }

            if (c.TimeStamp != 0 && (c.dateTime - System.DateTime.UtcNow).TotalSeconds > 0)
            {
                chestUnlocking = true;
            }

            if(slotsEmpty && c.Name != "")
            {
                slotsEmpty = false;
            }
        }
        if (chestUnlocked || (!chestUnlocking && !slotsEmpty))
        {
            raid.notificationGO.SetActive(true);
            raid.notificationCountText.text = " !";
        }
        else
        {
            raid.notificationGO.SetActive(false);
        }
	}

	public void SetMailNotification()
	{
		//MAIL
		if (Data.instance.mail.newMailCount > 0)
		{
			mail.notificationGO.SetActive(true);
			mail.notificationCountText.text = Data.instance.mail.newMailCount.ToString();
		}
		else
		{
			mail.notificationGO.SetActive(false);
		}
	}
}
