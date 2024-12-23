using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PhotonTest : MonoBehaviour
{
	public ClientState state = 0;

	//private void Start()
	//{   //유저 이름 세팅
	//	PhotonNetwork.NickName = $"Test Player {Random.Range(100, 1000)}";

	//	//포톤 서버 접속(PhotonServerSettings 파일의 설정 사용)
	//	PhotonNetwork.ConnectUsingSettings();
	//}



	private void Update()
	{
		if (PhotonNetwork.NetworkClientState != state)
		{
			state = PhotonNetwork.NetworkClientState;
			LogManager.Log($"state changed : {state}");
		}
	}
}
