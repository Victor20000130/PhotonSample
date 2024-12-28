using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameTest : MonoBehaviour
{
	private void Start()
	{
		LogManager.Log(PhotonNetwork.NickName);
		LogManager.Log(PhotonNetwork.IsConnected);
	}
}
