using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class B_ChatUI : MonoBehaviour
{
	public Text roomNameLabel;
	public InputField messageInput;
	public Button sendButton;
	public RectTransform messageContent;
	public GameObject messageEntryPrefab;

	public string myNickname = "UnamedUser";

	private void Awake()
	{
		messageInput.onEndEdit.AddListener(x => SendChatMessage());
	}

	private void SendChatMessage()
	{
		string message = messageInput.text;
		if (string.IsNullOrEmpty(message)) return;
		if (message.ContainsFword())
		{
			ReceiveChatMessage("", "<color=red>비속어가 포함되어 있습니다.</color>");
		}
		else
		{
			B_ChatManager.Instance.SendChatMessage(message);
		}
	}

	public void ReceiveChatMessage(string nickname, string message)
	{
		var entry = Instantiate(messageEntryPrefab, messageContent);
		entry.transform.Find("Nickname").GetComponent<Text>().text = nickname;
		entry.transform.Find("Message").GetComponent<Text>().text = message;
	}
}
