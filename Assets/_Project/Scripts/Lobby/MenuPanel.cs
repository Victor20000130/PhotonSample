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
	//���
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
		playerName.text = $"�ȳ��ϼ���, {PhotonNetwork.LocalPlayer.NickName}��";
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
	{   //���� ����
		PhotonNetwork.Disconnect();
	}

	private void CreateButtonClick()
	{
		string roomName = roomNameInput.text;

		//int.TryParse�� ���� ��ȿ�� �˻� ����
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

		//�ִ� �÷��̾ 8�� �� ����
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
			logText.text = "����� ������ �г����Դϴ�.";
			nicknameInput.text = "";
			B_ChatManager.Instance.SetNickname(nickname);
			createRoomButton.gameObject.SetActive(true);
			findRoomButton.gameObject.SetActive(true);
			randomRoomButton.gameObject.SetActive(true);
			logoutButton.gameObject.SetActive(true);
			PhotonNetwork.NickName = nickname;
			playerName.text = $"�ȳ��ϼ���, {PhotonNetwork.NickName}��";
		}
		else
		{
			logText.text = "�г��� ��Ģ�� ���ݵǾ����ϴ�.";
		}



	}

	private void NickNameInputEdit(string input)
	{
		nicknameInput.SetTextWithoutNotify(input.ToValidString());
		logText.text = "";
	}

}

