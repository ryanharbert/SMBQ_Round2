using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PunManager : MonoBehaviourPunCallbacks
{
    public static PunManager instance;
    
	void Start ()
    {
        instance = this;
        DontDestroyOnLoad(transform.gameObject);
    }

    public void JoinQue(string searchDesc)
    {
        if(Que.instance != null)
        {
            Que.instance.displayObject.SetActive(true);
            Que.instance.searchDesc.text = searchDesc;
        }
        StartCoroutine("ConnectAndJoinBattle");
    }

    IEnumerator ConnectAndJoinBattle()
    {
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Connecting");
        while (!PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        Debug.Log("Connected");
        while (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterserver)
        {
            yield return null;
        }
        Debug.Log("ConnectedToMasterServer");

        while (!PhotonNetwork.InRoom)
        {
            yield return null;
        }
        Debug.Log("In Room");
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Master Client");
        }
    }
    public void LeaveRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public void LeaveQue()
    {
        StopAllCoroutines();
        PhotonNetwork.Disconnect();
        StartCoroutine("Disconnect");
    }

    IEnumerator Disconnect()
    {
        while(PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        Debug.Log("Disconnected");
        if (Que.instance != null)
        {
            Que.instance.displayObject.SetActive(false);
        }
    }
}
