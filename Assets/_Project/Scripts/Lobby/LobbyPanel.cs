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
		//파괴후보
		List<RoomInfo> destroyCandidate;
		//현재 RoomList에는 있는데, OnRoomListUpdate의 파라미터로
		//넘어온 RoomList에는 없는 방 참여 버튼은 삭제해야함.
		destroyCandidate = currentRoomList.FindAll(x => false == roomsList.Contains(x));

		//currentRoomList에는 없는데 roomList에 있는 방참여 버튼 생성하기.
		foreach (RoomInfo roomInfo in roomsList)
		{
			if (currentRoomList.Contains(roomInfo)) continue;
			AddRoomButton(roomInfo);
		}

		//destroyCandidate 리스트에 있는 방 참여 버튼 삭제.
		foreach (Transform child in roomListRect)
		{
			if (destroyCandidate.Exists(x => x.Name == child.name))
			{
				Destroy(child.gameObject);
			}
			currentRoomList = roomsList;
		}
	}

	//방 데이터가 있으면, 방 참여 버튼을 생성할 메서드.
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