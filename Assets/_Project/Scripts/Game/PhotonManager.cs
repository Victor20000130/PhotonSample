using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Æ÷Åæ ÄÝº¤ ¸®½º³Ê
public class PhotonManager : MonoBehaviourPunCallbacks
{

	public bool isTestmode = false;

	private void Start()
	{
		isTestmode = PhotonNetwork.IsConnected == false;

		if (isTestmode)
		{
			PhotonNetwork.ConnectUsingSettings();
		}
		else
		{
			GameManager.isGameReady = true;
		}
	}

	public override void OnConnectedToMaster()
	{
		if (isTestmode)
		{
			RoomOptions options = new()
			{
				IsVisible = false,
				MaxPlayers = 8,
			};
			PhotonNetwork.JoinOrCreateRoom("TestRoom", options, TypedLobby.Default);
		}
	}

	public override void OnJoinedRoom()
	{
		if (isTestmode)
		{
			GameObject.Find("Canvas/DebugText").GetComponent<Text>().text =
				PhotonNetwork.CurrentRoom.Name;

			GameManager.isGameReady = true;
		}
	}
}