using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinUI : MonoBehaviour
{
	public InputField nicknameInput;
	public InputField roomnameInput;

	public Button nicknameChangeButton;
	public Button connectButton;
	public Button joinRoomButton;

	public Text logText;


	private void Awake()
	{
		nicknameInput.onValueChanged.AddListener(NicknameInputEdit);
		nicknameChangeButton.onClick.AddListener(NicknameChangeButtonClick);
		connectButton.onClick.AddListener(ConnectButtonClick);
		joinRoomButton.onClick.AddListener(JoinRoomButtonClick);

	}

	//닉네임 입력란에 입력이 될 때마다 문자열 검증
	private void NicknameInputEdit(string input)
	{
		nicknameInput.SetTextWithoutNotify(input.ToValidString());
		logText.text = "";
	}

	//유효한 닉네임인지 검증 할거야
	private void NicknameChangeButtonClick()
	{
		string nickname = nicknameInput.text;
		if (nickname.NicknameValidate())
		{
			logText.text = "사용이 가능한 닉네임입니다.";
			ChatManager.Instance.SetNickname(nickname);
		}
		else
		{
			logText.text = "닉네임 규칙에 위반되었습니다.";
		}
	}

	private void ConnectButtonClick()
	{
		ChatManager.Instance.ConnectUsingSettings();
		connectButton.interactable = false;
	}

	private void JoinRoomButtonClick()
	{
		ChatManager.Instance.ChatStart(roomnameInput.text);
		roomnameInput.interactable = false;
		joinRoomButton.interactable = false;
	}

	public void OnJoinedServer()
	{
		connectButton.GetComponentInChildren<Text>().text = "채팅 서버 접속됨";
	}
}

