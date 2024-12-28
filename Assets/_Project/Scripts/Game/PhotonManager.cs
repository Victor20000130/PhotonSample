using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//포톤 콜벡 리스너
public class PhotonManager : MonoBehaviourPunCallbacks
{

    public bool isTestmode = false;

    private IEnumerator Start()
    {
        isTestmode = PhotonNetwork.IsConnected == false;

        if (false == PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("SceneLoaded"))
        {
            PhotonNetwork.LocalPlayer.CustomProperties.Add("SceneLoaded", true);
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);

        if (isTestmode)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            yield return new WaitUntil(AllPlayersReady);
            yield return null;
        }
        GameManager.isGameReady = true;
    }

    private bool AllPlayersReady()
    {
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (false == player.CustomProperties.ContainsKey("SceneLoaded"))
                return false;
        }
        return true;
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