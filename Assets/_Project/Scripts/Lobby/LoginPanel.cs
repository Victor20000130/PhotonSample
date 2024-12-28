using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LoginPanel : MonoBehaviour
{
    public InputField idInput;
    public InputField pwInput;
    public Button createButton;
    public Button loginButton;
    public Button chatButton;

    private void Awake()
    {
        loginButton.onClick.AddListener(OnLogInButtonClick);
        chatButton.onClick.AddListener(OnChatButtonClick);
    }

    private void OnChatButtonClick()
    {
        SceneManager.LoadScene("ChatScene");
    }

    private void OnLogInButtonClick()
    {
        string userNickname = idInput.text;
        LogManager.Log($"OnLogInButtonClick_userNickname : {userNickname}");
        PhotonNetwork.NickName = userNickname;
        LogManager.Log($"OnLogInButtonClick_phoNicName : {PhotonNetwork.NickName}");
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
}

