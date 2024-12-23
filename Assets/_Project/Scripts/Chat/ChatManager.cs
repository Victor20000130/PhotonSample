using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Chat.Demo;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChatAuthValues = Photon.Chat.AuthenticationValues;

//���� ê�� ����ϱ� ���ؼ� 1. IChatClientListener �������̽� ����
public class ChatManager : MonoBehaviour, IChatClientListener
{
	public static ChatManager Instance { get; private set; }

	public JoinUI joinUI;
	public ChatUI chatUI;

	private ChatClient client;

	public ChatState chatState = 0;

	public string currentChannel;

	private void Awake()
	{
		Instance = this;
	}

	//2. ChatClient�� ����
	private void Start()
	{
		client = new ChatClient(this);
		//print(@"\n");
	}

	//3. Update���� Service�� ȣ��
	private void Update()
	{
		client.Service();
	}

	public void SetNickname(string nickname)
	{   //PhotonNetwork.NickName = nickname;
		client.AuthValues = new ChatAuthValues(nickname);

	}

	public void ConnectUsingSettings()
	{   //PhotonServerSettings�� ����Ͽ� ������ ���
		AppSettings appSettings = PhotonNetwork.PhotonServerSettings.AppSettings;

		ChatAppSettings chatSettings = appSettings.GetChatSettings();
		//new ChatAppSettings
		//{
		//	AppIdChat = appSettings.AppIdChat,
		//	....
		//	�ϳ��� �ֱ� �������� ���� �ִ�
		//	GetChatSettings Ȯ��޼��带 �̿��ؼ�
		//	�ʱⰪ���� �������ֱ�(�ʱ�ȭ)
		//};

		client.ConnectUsingSettings(chatSettings);

	}


	public void ConnectUsingAppId()
	{   //�⺻������ AppId�� ���� ������ ���
		string chatId = "781a0cd6-148b-4a7b-8c4e-28c85cd66ebd";
		client.Connect(chatId, "1.0", client.AuthValues);
	}

	//Ư�� ä�ù�(ä�� ä��)���� ä�� ����
	public void ChatStart(string roomName)
	{
		LogManager.Log(roomName);
		client.Subscribe(new string[] { roomName });

	}

	//ä�� �޼��� ����
	public void SendChatMessage(string message)
	{
		client.PublishMessage(currentChannel, message);
	}

	public void OnChatStateChange(ChatState state)
	{
		if (this.chatState != state)
		{
			print($"Chat state Changed : {this.chatState} -> {state}");
			this.chatState = state;
		}
	}

	public void OnSubscribed(string[] channels, bool[] results)
	{
		currentChannel = channels[0];
		joinUI.gameObject.SetActive(false);
		chatUI.gameObject.SetActive(true);
		chatUI.roomNameLabel.text = currentChannel;
		chatUI.ReceiveChatMessage("", $"<color=green>{currentChannel} ä�ù濡 �����Ͽ����ϴ�.</color>");
		//print($"ä�ù� ���� : {currentChannel} <- {channels[0]}");
	}
	public void OnConnected()
	{
		joinUI.OnJoinedServer();
	}

	public void OnGetMessages(string channelName, string[] senders, object[] messages)
	{
		if (channelName != currentChannel)
		{
			print($"�ٸ� ä���� �޼��� ������ : {channelName}");
			return;
		}

		//messages�� string�� �ƴ϶� object�� ������
		//����ȭ ������ȭ�� ������ ���·� ������ �����̴�.
		for (int i = 0; i < senders.Length; i++)
		{
			chatUI.ReceiveChatMessage(senders[i], messages[i].ToString());
		}
	}

	public void ChatEnd(string roomName)
	{
		client.Unsubscribe(new string[] { roomName });
	}


	public void DebugReturn(DebugLevel level, string message)
	{
	}

	public void OnDisconnected()
	{

	}



	public void OnPrivateMessage(string sender, object message, string channelName)
	{
	}


	public void OnUnsubscribed(string[] channels)
	{
	}

	public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
	{
	}

	public void OnUserSubscribed(string channel, string user)
	{
	}

	public void OnUserUnsubscribed(string channel, string user)
	{
	}
}
