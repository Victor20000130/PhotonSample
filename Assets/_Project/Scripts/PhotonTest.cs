using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PhotonTest : MonoBehaviour
{
	public ClientState state = 0;

	//private void Start()
	//{   //���� �̸� ����
	//	PhotonNetwork.NickName = $"Test Player {Random.Range(100, 1000)}";

	//	//���� ���� ����(PhotonServerSettings ������ ���� ���)
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
