using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using Random = UnityEngine.Random;

public class MenuPanel : MonoBehaviour
{
	//상수
	const int MAX_PLAYER = 8;

	public Text playerName;

	public InputField nicknameInput;
	public Button nicknameUpdateButton;

	public Text logText;

	[Header("Main Menu")]
	#region Main Menu
	public RectTransform mainMenuPanel;
	public Button createRoomButton;
	public Button findRoomButton;
	public Button randomRoomButton;
	public Button logoutButton;
	#endregion

	[Space(20)]
	[Header("Create Room Menu")]
	#region Create Room Menu
	public RectTransform createRoomMenuPanel;
	public InputField roomNameInput;
	public InputField playerNumInput;
	public Button createButton;
	public Button cancelButton;
	#endregion

	private void Awake()
	{
		createRoomButton.onClick.AddListener(CreateRoomButtonClick);
		findRoomButton.onClick.AddListener(FindRoomButtonClick);
		randomRoomButton.onClick.AddListener(RandomRoomButtonClick);
		logoutButton.onClick.AddListener(LogoutButtonClick);
		createButton.onClick.AddListener(CreateButtonClick);
		cancelButton.onClick.AddListener(CancelButtonClick);
		nicknameUpdateButton.onClick.AddListener(NicknameUpdateButtonClick);
		nicknameInput.onValueChanged.AddListener(NickNameInputEdit);
	}

	private void OnEnable()
	{
		playerName.text = $"안녕하세요, {PhotonNetwork.LocalPlayer.NickName}님";
		CancelButtonClick();
	}

	private void CreateRoomButtonClick()
	{
		mainMenuPanel.gameObject.SetActive(false);
		createRoomMenuPanel.gameObject.SetActive(true);
	}

	private void FindRoomButtonClick()
	{
		PhotonNetwork.JoinLobby();
	}

	private void RandomRoomButtonClick()
	{
		RoomOptions roomOptions = new RoomOptions
		{
			MaxPlayers = MAX_PLAYER
		};

		PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: roomOptions);

	}

	private void LogoutButtonClick()
	{   //연결 해제
		PhotonNetwork.Disconnect();
	}

	private void CreateButtonClick()
	{
		string roomName = roomNameInput.text;

		//int.TryParse를 쓰면 유효성 검사 가능
		if (false == int.TryParse(playerNumInput.text, out int maxPlayer))
		{
			maxPlayer = MAX_PLAYER;
		}

		if (string.IsNullOrEmpty(roomName))
		{
			roomName = $"Room{Random.Range(0, 1000)}";
		}

		if (maxPlayer <= 0)
		{
			maxPlayer = MAX_PLAYER;
		}

		RoomOptions option = new RoomOptions
		{
			MaxPlayers = MAX_PLAYER
		};

		//최대 플레이어가 8인 방 생성
		PhotonNetwork.CreateRoom(roomName, option);

	}

	private void CancelButtonClick()
	{
		mainMenuPanel.gameObject.SetActive(true);
		createRoomMenuPanel.gameObject.SetActive(false);
	}

	public void NicknameUpdateButtonClick()
	{
		string nickname = nicknameInput.text;
		if (nickname.NicknameValidate())
		{
			logText.text = "사용이 가능한 닉네임입니다.";
			nicknameInput.text = "";
			B_ChatManager.Instance.SetNickname(nickname);
			createRoomButton.gameObject.SetActive(true);
			findRoomButton.gameObject.SetActive(true);
			randomRoomButton.gameObject.SetActive(true);
			logoutButton.gameObject.SetActive(true);
			PhotonNetwork.NickName = nickname;
			playerName.text = $"안녕하세요, {PhotonNetwork.NickName}님";
		}
		else
		{
			logText.text = "닉네임 규칙에 위반되었습니다.";
		}

	}

	private void NickNameInputEdit(string input)
	{
		nicknameInput.SetTextWithoutNotify(input.ToValidString());
		logText.text = "";
	}

}

