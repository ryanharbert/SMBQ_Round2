using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.DataModels;

public class InGuild : MonoBehaviour
{
	public Text guildName;

	public NavBarToggle chat;
	public NavBarToggle members;
	public NavBarToggle options;

	public GuildQuestDisplay guildQuest;
	public GameObject chatObject;
	public GameObject membersListObject;
	public GameObject optionsObject;

	public Button guildMessageButton;
	public Text guildMessageButtonText;
	public Text guildMessage;
	public InputField guildMessageInput;

    private void OnEnable()
    {
        guildName.text = Data.instance.guild.name;

        chat.toggle.isOn = true;
        members.toggle.isOn = false;
        options.toggle.isOn = false;

		MessageStart();
    }

	public void Chat(bool on)
	{
		ToggleGameObject(chatObject, on, chat);
		if(on)
        {
            guildQuest.gameObject.SetActive(true);
        }
	}

	public void Members(bool on)
	{
		ToggleGameObject(membersListObject, on, members);
		if (on)
        {
            guildQuest.gameObject.SetActive(true);
        }
	}

	public void Options(bool on)
	{
		ToggleGameObject(optionsObject, on, options);
		if (on)
        {
            guildQuest.gameObject.SetActive(false);
        }
	}

	public void ToggleGameObject(GameObject go, bool active, NavBarToggle toggle)
	{

		if (active)
		{
			go.SetActive(true);
			toggle.toggleText.color = NavBar.instance.highlightColor;
			toggle.toggleText.fontSize = 70;
		}
		else
		{
			go.SetActive(false);
			toggle.toggleText.color = NavBar.instance.normalColor;
			toggle.toggleText.fontSize = 55;
		}
	}

	void MessageStart()
	{
		guildMessage.gameObject.SetActive(true);
		guildMessage.text = Data.instance.guild.guildMessage;
		guildMessageInput.gameObject.SetActive(false);
		if (Data.instance.guild.rank != "Leader")
		{
			guildMessageButton.gameObject.SetActive(false);
		}
		else
		{
			guildMessageButton.gameObject.SetActive(true);
			guildMessageButtonText.text = "Edit";
			guildMessageButton.onClick.RemoveAllListeners();
			guildMessageButton.onClick.AddListener(Edit);
		}
	}

	public void Edit()
	{
		guildMessage.gameObject.SetActive(false);
		guildMessageInput.gameObject.SetActive(true);
		guildMessageInput.text = Data.instance.guild.guildMessage;
		guildMessageButtonText.text = "Save";
		guildMessageButton.onClick.RemoveAllListeners();
		guildMessageButton.onClick.AddListener(Save);

	}

	public void Save()
	{
		Data.instance.guild.guildMessage = guildMessageInput.text;
		guildMessageButton.interactable = false;

		EntityKey e = new EntityKey() {Id = Data.instance.guild.entityKey.Id, Type = Data.instance.guild.entityKey.Type };
		PlayFabDataAPI.SetObjects(new SetObjectsRequest() { Entity = e, Objects = new List<SetObject>() { new SetObject() { DataObject = new { Message = guildMessageInput.text }, ObjectName = "Message" } } }, Saved, ServerFailure);
	}

	void Saved(SetObjectsResponse response)
	{
		guildMessageButton.interactable = true;
		MessageStart();
	}

	void ServerFailure(PlayFabError error)
	{
		Debug.Log(error.ToString());
	}

}
