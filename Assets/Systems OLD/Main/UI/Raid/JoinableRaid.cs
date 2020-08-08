using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class JoinableRaid : MonoBehaviour
{
    public Text displayName;
    public Text level;
    public Text host;
    public GameObject locked;
    public Button button;

    public void Set(RoomInfo room)
    {
        displayName.text = room.CustomProperties["type"].ToString();

        host.text = room.Name;
    }

    public void JoinRaid()
    {

    }
}
