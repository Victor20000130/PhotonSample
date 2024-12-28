using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Chat.Demo;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChatAuthValues = Photon.Chat.AuthenticationValues;

public class B_ChatManager : MonoBehaviour, IChatClientListener
{

	public static B_ChatManager Instance { get; private set; }

	public B_JoinUI b_joinUI;
	public B_ChatUI b_chatUI;

	private ChatClient client;

	public ChatState chatState = 0;

	public string currentChannel;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		client = new ChatClient(this);
	}

	private void Update()
	{
		client.Service();
	}

	public void SetNickname(string nickname)
	{
		client.AuthValues = new ChatAuthValues(nickname);
	}

	public void ConnectUsingSettings()
	{
		AppSettings appSettings = PhotonNetwork.PhotonServerSettings.AppSettings;
		ChatAppSettings chatSettings = appSettings.GetChatSettings();

		client.ConnectUsingSettings(chatSettings);
	}

	public void ChatStart(string roomName)
	{
		LogManager.Log(roomName);
		client.Subscribe(new string[] { roomName });
	}

	internal void SendChatMessage(string message)
	{
		client.PublishMessage(currentChannel, message);
	}

	public void OnSubscribed(string[] channels, bool[] results)
	{
		currentChannel = channels[0];
		b_chatUI.gameObject.SetActive(true);
		b_chatUI.roomNameLabel.text = currentChannel;
		b_chatUI.ReceiveChatMessage("", $"<color=green>{currentChannel} 채팅방에 입장하였습니다.</color>");
		print($"채팅방 접속 : {currentChannel} <- {channels[0]}");
	}

	public void OnConnected()
	{
		b_joinUI.OnJoinedServer();
	}

	public void OnGetMessages(string channelName, string[] senders, object[] messages)
	{
		if (channelName != currentChannel)
		{
			print($"다른 채널의 메세지 수신함 : {channelName}");
			return;
		}

		for (int i = 0; i < senders.Length; i++)
		{
			b_chatUI.ReceiveChatMessage(senders[i], messages[i].ToString());
		}

	}

	public void DebugReturn(DebugLevel level, string message)
	{
	}

	public void OnChatStateChange(ChatState state)
	{
	}

	public void OnDisconnected()
	{
	}

	public void OnPrivateMessage(string sender, object message, string channelName)
	{
	}

	public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
	{
	}

	public void OnUnsubscribed(string[] channels)
	{
	}

	public void OnUserSubscribed(string channel, string user)
	{
	}

	public void OnUserUnsubscribed(string channel, string user)
	{
	}

}
