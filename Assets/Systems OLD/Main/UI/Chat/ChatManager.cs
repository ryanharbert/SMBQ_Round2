using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Chat;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, IChatClientListener
{
	public static ChatManager instance;

	public struct ChatNotification
	{
		public int count;
		public bool pageOpen;
	}

	public ChatClient chatClient;

	public ChatWindow worldChat;
	public ChatWindow guildChat;

	public ChatNotification worldNotification;
	public ChatNotification guildNotification;

	public string message = "";

	public string guildChatRoom = "";
	string worldChatRoom = "World";
	public bool guildSubscribed = false;

    bool connected;

	private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;

		worldNotification.pageOpen = false;
		worldNotification.count = 0;

		guildNotification.pageOpen = false;
		guildNotification.count = 0;
	}

	public IEnumerator Connect()
    {
        connected = false;

        chatClient = new ChatClient(this, ExitGames.Client.Photon.ConnectionProtocol.WebSocketSecure);
		chatClient.Connect(Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new AuthenticationValues(Data.instance.user.displayName));

        while(connected)
        {
            yield return null;
        }
	}

	/// <summary>To avoid that the Editor becomes unresponsive, disconnect all Photon connections in OnApplicationQuit.</summary>
	public void OnApplicationQuit()
	{
		if (this.chatClient != null)
		{
			this.chatClient.Disconnect();
		}
	}

	public void Update()
	{
		if (connected && chatClient != null)
		{
			chatClient.Service(); // make sure to call this regularly! it limits effort internally, so calling often is ok!
		}

	}

	public void MessageChanged(string message)
	{
		this.message = message;
	}

	public void SendMessage(bool worldChat)
	{
		if(message == "")
		{
			return;
		}

		if(worldChat)
		{
			chatClient.PublishMessage(worldChatRoom, message);
		}
		else
		{
			message = DateTime.UtcNow.Month + "/" + DateTime.UtcNow.Day + " " + DateTime.UtcNow.Hour + ":" + DateTime.UtcNow.Minute + " " + message;
            chatClient.PublishMessage(guildChatRoom, message);
		}
	}

	public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
	{
		if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
		{
			UnityEngine.Debug.LogError(message);
		}
		else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
		{
			UnityEngine.Debug.LogWarning(message);
		}
		else
		{
			UnityEngine.Debug.Log(message);
		}
	}

	public void OnConnected()
	{
		Debug.Log("Connected to Chat!");
		chatClient.Subscribe(new string[] { worldChatRoom });
		if(guildChatRoom != "")
		{
			chatClient.Subscribe(new string[] { guildChatRoom }, 20);
		}

        connected = true;
	}

	public void OnDisconnected()
	{

	}

	public void OnChatStateChange(ChatState state)
	{

	}

	public void OnSubscribed(string[] channels, bool[] results)
	{
		for(int i = 0; i < channels.Length; i++)
		{
			if(channels[i] == guildChatRoom)
			{
				guildSubscribed = true;
			}
		}
	}

	public void OnUnsubscribed(string[] channels)
	{
		for (int i = 0; i < channels.Length; i++)
		{
			if (channels[i] == guildChatRoom)
			{
				guildSubscribed = false;
			}
		}
	}

	public void OnGetMessages(string channelName, string[] senders, object[] messages)
	{
        if(channelName == "World")
        {
			if(worldChat != null)
			{
				worldChat.chatText.text = GetChatString(channelName);
				worldChat.ScrollChat();
				if (worldNotification.pageOpen == false)
				{
					worldNotification.count++;
					if (NavBar.instance != null)
					{
						NavBarToggle t = NavBar.instance.chat;

						t.notificationCountText.text = worldNotification.count.ToString();

						if (t.notificationGO.activeSelf == false)
						{
							t.notificationGO.SetActive(true);
						}
					}
				}
			}
		}
		else
		{
			if(guildChat != null)
			{
				guildChat.chatText.text = GetChatString(channelName);
				guildChat.ScrollChat();
				if (guildNotification.pageOpen == false)
				{
					guildNotification.count++;
					if (NavBar.instance != null)
					{
						NavBarToggle t = NavBar.instance.guild;

						t.notificationCountText.text = guildNotification.count.ToString();

						if (t.notificationGO.activeSelf == false)
						{
							t.notificationGO.SetActive(true);
						}
					}
				}
			}

		}
	}

	public void OnPrivateMessage(string sender, object message, string channelName)
	{

	}

	public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
	{

	}

	public void OnUserSubscribed(string channel, string user)
	{
		throw new NotImplementedException();
	}

	public void OnUserUnsubscribed(string channel, string user)
	{
		throw new NotImplementedException();
	}

	public string GetChatString(string channelName)
    {
        return chatClient.PublicChannels[channelName].ToStringMessages();
    }
}