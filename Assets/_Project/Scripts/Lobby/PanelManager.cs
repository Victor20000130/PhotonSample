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
	{   //MonoBehaviourPunCallbacks �� ��ӹ������� ������ �ݺ����� ����� �� �ִ�.
		//�θ��� �޼��带 ��ӹ޾ƾ߸�, ������ ����� �����ϴ�.
		//MonoBehaviourPunCallbacks�� ����� Ŭ������ OnEnable��
		//������ �� �� �� �θ��� OnEnable�� ȣ���ؾ� �Ѵ�.
		base.OnEnable();


		//print("����");
	}

	public override void OnConnected()
	{   //���� ������ ������� �� ȣ��
		PanelOpen("Menu");
	}

	public override void OnDisconnected(DisconnectCause cause)
	{   //������ �������� �� ȣ��
		LogManager.Log($"�α׾ƿ� �� : {cause}");
		PanelOpen("Login");
	}

	public override void OnCreatedRoom()
	{   //���� �����Ͽ��� �� ȣ��
		PanelOpen("Room");
	}

	public override void OnJoinedRoom()
	{   //�濡 ������ �� ȣ��
		PanelOpen("Room");
		Hashtable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
		if (roomCustomProperties.ContainsKey("Difficulty"))
		{
			room.OnDifficultyChange((Difficulty)roomCustomProperties["Difficulty"]);
		}
	}

	public override void OnLeftRoom()
	{   //���� ���� �� ȣ��
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
	{   //�κ� ���ƿ� �� ȣ��
		PanelOpen("Lobby");
	}

	public override void OnLeftLobby()
	{   //�κ� ���� �� ȣ��
		PanelOpen("Menu");
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{   //�� ����Ʈ�� ������Ʈ�� �� ȣ��
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




