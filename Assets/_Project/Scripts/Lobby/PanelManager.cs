using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PanelManager : MonoBehaviourPunCallbacks
{
	public static PanelManager Instance;

	public LoginPanel login;
	public MenuPanel menu;
	public LobbyPanel lobby;
	public RoomPanel room;


	Dictionary<string, GameObject> panelDic;

	private void Awake()
	{
		Instance = this;
		panelDic = new Dictionary<string, GameObject>()
		{
			{ "Login", login.gameObject},
			{ "Menu", menu.gameObject},
			{ "Lobby", lobby.gameObject},
			{ "Room", room.gameObject }
		};

		PanelOpen("Login");
	}

	public void PanelOpen(string panelName)
	{
		foreach (var row in panelDic)
		{
			row.Value.SetActive(row.Key == panelName);
		}
	}

	public override void OnEnable()
	{   //MonoBehaviourPunCallbacks 을 상속받음으로 포톤의 콜벡들을 사용할 수 있다.
		//부모의 메서드를 상속받아야만, 서버와 통신이 가능하다.
		//MonoBehaviourPunCallbacks를 상속한 클래스는 OnEnable을
		//재정의 할 때 꼭 부모의 OnEnable을 호출해야 한다.
		base.OnEnable();


		//print("하이");
	}

	public override void OnConnected()
	{   //포톤 서버에 연결됐을 때 호출
		PanelOpen("Menu");
	}

	public override void OnDisconnected(DisconnectCause cause)
	{   //연결이 해제됐을 때 호출
		LogManager.Log($"로그아웃 됨 : {cause}");
		PanelOpen("Login");
	}

	public override void OnCreatedRoom()
	{   //방을 생성하였을 때 호출
		PanelOpen("Room");
	}

	public override void OnJoinedRoom()
	{   //방에 참여할 때 호출
		PanelOpen("Room");
		Hashtable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
		if (roomCustomProperties.ContainsKey("Difficulty"))
		{
			room.OnDifficultyChange((Difficulty)roomCustomProperties["Difficulty"]);
		}
	}

	public override void OnLeftRoom()
	{   //방을 떠날 때 호출
		PanelOpen("Menu");
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		room.JoinPlayer(newPlayer);
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		room.LeavePlayer(otherPlayer);
	}

	public override void OnJoinedLobby()
	{   //로비에 돌아올 때 호출
		PanelOpen("Lobby");
	}

	public override void OnLeftLobby()
	{   //로비를 떠날 때 호출
		PanelOpen("Menu");
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{   //룸 리스트가 업데이트될 때 호출
		lobby.UpdateRoomList(roomList);
	}

	public override void OnRoomPropertiesUpdate(Hashtable properties)
	{
		if (properties.ContainsKey("Difficulty"))
		{
			room.OnDifficultyChange((Difficulty)properties["Difficulty"]);
		}
	}

	public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		if (changedProps.ContainsKey("CharacterSelect"))
		{
			room.OnCharacterSelectChange(targetPlayer, changedProps);
		}
		if (changedProps.ContainsKey("Ready"))
		{
			room.OnReadySelectChange(targetPlayer, changedProps);
		}
	}

}




