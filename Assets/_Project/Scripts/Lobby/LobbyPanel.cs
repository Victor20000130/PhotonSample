using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
	public RectTransform roomListRect;

	public GameObject roomButtonPrefab;

	public Button cancelButton;

	private List<RoomInfo> currentRoomList = new List<RoomInfo>();

	private void Awake()
	{
		cancelButton.onClick.AddListener(CancleButtonClick);

	}

	private void OnEnable()
	{
		if (PhotonNetwork.InLobby == false) return;



		foreach (RoomInfo roomInfo in currentRoomList)
		{
			AddRoomButton(roomInfo);
		}
	}

	private void OnDisable()
	{
		foreach (Transform child in roomListRect)
		{
			Destroy(child.gameObject);
		}
	}

	public void UpdateRoomList(List<RoomInfo> roomsList)
	{
		//�ı��ĺ�
		List<RoomInfo> destroyCandidate;
		//���� RoomList���� �ִµ�, OnRoomListUpdate�� �Ķ���ͷ�
		//�Ѿ�� RoomList���� ���� �� ���� ��ư�� �����ؾ���.
		destroyCandidate = currentRoomList.FindAll(x => false == roomsList.Contains(x));


		//currentRoomList���� ���µ� roomList�� �ִ� ������ ��ư �����ϱ�.
		foreach (RoomInfo roomInfo in roomsList)
		{
			if (currentRoomList.Contains(roomInfo)) continue;
			AddRoomButton(roomInfo);
		}

		//destroyCandidate ����Ʈ�� �ִ� �� ���� ��ư ����.
		foreach (Transform child in roomListRect)
		{
			if (destroyCandidate.Exists(x => x.Name == child.name))
			{
				Destroy(child.gameObject);
			}
			currentRoomList = roomsList;
		}
	}

	//�� �����Ͱ� ������, �� ���� ��ư�� ������ �޼���.
	public void AddRoomButton(RoomInfo roomInfo)
	{
		GameObject joinButton = Instantiate(roomButtonPrefab, roomListRect, false);
		joinButton.name = roomInfo.Name;
		joinButton.GetComponent<Button>()
			.onClick.AddListener(() => PhotonNetwork.JoinRoom(roomInfo.Name));
		joinButton.GetComponentInChildren<Text>().text = roomInfo.Name;
	}

	private void CancleButtonClick()
	{
		PhotonNetwork.LeaveLobby();
	}
}