using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

public class ChatWindow : MonoBehaviourPunCallbacks
{

	public bool worldChat = true;
	public InputField inputField;
	public Button button;
	public Text chatText;

	public ScrollRect scrollRect;
	public RectTransform contentRect;
	public RectTransform viewRect;

    public bool inputSelected = false;

    public GameObject newMessagesObject;

    public List<JoinableRaid> raids;

    float yScrollDown = 0f;
    float getRaidTimer = 0f;

	private void Awake()
	{
		if(worldChat)
		{
			chatText.text = ChatManager.instance.GetChatString("World");
		}
		else
		{
			chatText.text = ChatManager.instance.GetChatString(Data.instance.guild.name);
		}

		button.onClick.AddListener(SendMessage);
		inputField.onValueChanged.AddListener(ChatManager.instance.MessageChanged);

        PhotonNetwork.GetCustomRoomList(TypedLobby.Default, "");
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && ChatManager.instance.message != "")
        {
            SendMessage();
        }

        getRaidTimer += Time.deltaTime;
        if (getRaidTimer > 2)
        {
            getRaidTimer = 0f;
            PhotonNetwork.GetCustomRoomList(TypedLobby.Default, "");
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for(int i = 0; i < raids.Count; i++)
        {
            if(i < roomList.Count)
            {
                raids[i].gameObject.SetActive(true);
                raids[i].Set(roomList[i]);
            }
            else
            {
                raids[i].gameObject.SetActive(false);
            }
        }
    }

	public void SendMessage()
	{
		ChatManager.instance.SendMessage(worldChat);
		ClearInputField();
	}

    private void OnEnable()
    {
        newMessagesObject.SetActive(false);
		if (worldChat)
		{
			ChatManager.instance.worldNotification.pageOpen = true;
		}
		else
		{
			ChatManager.instance.guildNotification.pageOpen = true;
		}
	}

    private void OnDisable()
    {
		if (worldChat)
		{
			ChatManager.instance.worldNotification.count = 0;
			ChatManager.instance.worldNotification.pageOpen = false;
		}
		else
		{
			ChatManager.instance.guildNotification.count = 0;
			ChatManager.instance.guildNotification.pageOpen = false;
		}
	}

    public void ClearInputField()
	{
		inputField.text = "";
	}

	public void ScrollChat()
	{
		float newY = Mathf.Max((contentRect.rect.height - viewRect.rect.height), 0f);

		if(Mathf.Abs(yScrollDown - contentRect.anchoredPosition.y) < 1f)
		{
			scrollRect.verticalNormalizedPosition = 0f;
		}

		yScrollDown = newY;
	}
}
