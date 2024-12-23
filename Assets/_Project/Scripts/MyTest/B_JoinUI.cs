using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class B_JoinUI : MonoBehaviour
{
	//�г���, ���̸�

	public string nickName;
	public string roomName;

	public Button connectButton;
	public Button joinChatButton;

	private void Awake()
	{
		nickName = PhotonNetwork.NickName;
		roomName = PhotonNetwork.CurrentRoom.Name;
		connectButton.onClick.AddListener(ConnectButtonClick);
		joinChatButton.onClick.AddListener(JoinChatButton);
		joinChatButton.interactable = false;
	}


	private void ConnectButtonClick()
	{
		B_ChatManager.Instance.ConnectUsingSettings();
		connectButton.interactable = false;
	}

	private void JoinChatButton()
	{
		B_ChatManager.Instance.ChatStart(roomName);
		joinChatButton.interactable = false;
	}

	public void OnJoinedServer()
	{
		connectButton.GetComponentInChildren<Text>().text = "ä�����ӿϷ�";
		joinChatButton.interactable = true;
	}
}
