using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Chat.Demo;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChatAuthValues = Photon.Chat.AuthenticationValues;

//포톤 챗을 사용하기 위해서 1. IChatClientListener 인터페이스 구현
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

	//2. ChatClient를 생성
	private void Start()
	{
		client = new ChatClient(this);
		//print(@"\n");
	}

	//3. Update에서 Service를 호출
	private void Update()
	{
		client.Service();
	}

	public void SetNickname(string nickname)
	{   //PhotonNetwork.NickName = nickname;
		client.AuthValues = new ChatAuthValues(nickname);

	}

	public void ConnectUsingSettings()
	{   //PhotonServerSettings를 사용하여 접속할 경우
		AppSettings appSettings = PhotonNetwork.PhotonServerSettings.AppSettings;

		ChatAppSettings chatSettings = appSettings.GetChatSettings();
		//new ChatAppSettings
		//{
		//	AppIdChat = appSettings.AppIdChat,
		//	....
		//	하나씩 넣기 귀찮으니 데모에 있는
		//	GetChatSettings 확장메서드를 이용해서
		//	초기값들을 설정해주기(초기화)
		//};

		client.ConnectUsingSettings(chatSettings);

	}


	public void ConnectUsingAppId()
	{   //기본적으로 AppId를 통해 접속할 경우
		string chatId = "781a0cd6-148b-4a7b-8c4e-28c85cd66ebd";
		client.Connect(chatId, "1.0", client.AuthValues);
	}

	//특정 채팅방(채팅 채널)에서 채팅 시작
	public void ChatStart(string roomName)
	{
		LogManager.Log(roomName);
		client.Subscribe(new string[] { roomName });

	}

	//채팅 메세지 전송
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
		chatUI.ReceiveChatMessage("", $"<color=green>{currentChannel} 채팅방에 입장하였습니다.</color>");
		//print($"채팅방 접속 : {currentChannel} <- {channels[0]}");
	}
	public void OnConnected()
	{
		joinUI.OnJoinedServer();
	}

	public void OnGetMessages(string channelName, string[] senders, object[] messages)
	{
		if (channelName != currentChannel)
		{
			print($"다른 채널의 메세지 수신함 : {channelName}");
			return;
		}

		//messages가 string이 아니라 object인 이유는
		//직렬화 역직렬화가 가능한 형태로 보내기 위함이다.
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
