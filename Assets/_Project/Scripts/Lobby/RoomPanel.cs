using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public enum Difficulty
{
	Easy = 0,
	Normal,
	Hard,
}

public class RoomPanel : MonoBehaviourPunCallbacks, IInRoomCallbacks
{

	public Text roomTitleText;

	public Difficulty roomDifficulty;

	public Dropdown difficultyDropdown;

	public Text difficultyText;

	public RectTransform playerList;
	public GameObject playerTextPrefab;

	public Button startButton;
	public Button cancelButton;

	private int readyCount;

	private void Awake()
	{
		startButton.onClick.AddListener(StartButtonClick);
		startButton.interactable = false;

		cancelButton.onClick.AddListener(CancleButtonClick);
		difficultyDropdown.ClearOptions();
		foreach (object difficulty in Enum.GetValues(typeof(Difficulty)))
		{
			Dropdown.OptionData optionData = new Dropdown.OptionData(difficulty.ToString());
			difficultyDropdown.options.Add(optionData);
		}
		difficultyDropdown.onValueChanged.AddListener(DifficultyValueChange);
	}

	public override void OnEnable()
	{
		base.OnEnable();
		if (false == PhotonNetwork.InRoom) return;

		roomTitleText.text = PhotonNetwork.CurrentRoom.Name;

		foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
		{   //플레이어 정보 객체 생성
			JoinPlayer(player);
		}

		//방장인 경우 IsMasterClient가 True로 온다.
		//해당 불변수를 통해서 방장만 난이도와 스타트를 볼 수 있도록 설정
		//방장이 연결이 끊겼을 때 자동으로 다른 클라이언트가 방장이되는데,
		//이때 별도의 콜벡으로 확인할 수 있다고 한다.
		OnMasterClientChanged();
	}

	public override void OnDisable()
	{
		base.OnDisable();
		foreach (Transform child in playerList)
		{   //플레이어 리스트에 다른 객체가 있으면 일단 모두 삭제
			Destroy(child.gameObject);
		}
	}

	public void JoinPlayer(Player newPlayer)
	{
		PlayerEntry playerEntry = Instantiate(playerTextPrefab, playerList,
			false).GetComponent<PlayerEntry>();
		//playerEntries.Add(playerEntry);
		playerEntry.playerNameText.text = newPlayer.NickName;
		playerEntry.player = newPlayer;
		if (PhotonNetwork.LocalPlayer.ActorNumber != newPlayer.ActorNumber)
		{
			playerEntry.readyToggle.gameObject.SetActive(false);
		}
		SortPlayer();
	}

	public void LeavePlayer(Player gonePlayer)
	{
		foreach (Transform child in playerList)
		{
			Player player = child.GetComponent<PlayerEntry>().player;
			if (player.ActorNumber == gonePlayer.ActorNumber)
			{
				Destroy(child.gameObject);
			}
		}
		SortPlayer();
	}

	public void SortPlayer()
	{
		foreach (Transform player in playerList)
		{
			Player playerInfo = player.GetComponent<PlayerEntry>().player;
			player.SetSiblingIndex(playerInfo.ActorNumber);
		}
	}

	private void DifficultyValueChange(int value)
	{
		Hashtable customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
		customProperties["Difficulty"] = value;
		PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
	}

	public void OnDifficultyChange(Difficulty value)
	{
		roomDifficulty = value;
		difficultyText.text = value.ToString();
	}

	public void OnCharacterSelectChange(Player target, Hashtable changes)
	{
		foreach (Transform child in playerList)
		{
			PlayerEntry entry = child.GetComponent<PlayerEntry>();
			if (entry.player == target)
			{
				int selection = (int)changes["CharacterSelect"];
				entry.SetSelection(selection);
			}
		}
	}

	public void OnReadySelectChange(Player target, Hashtable changes)
	{

		foreach (Transform child in playerList)
		{
			PlayerEntry entry = child.GetComponent<PlayerEntry>();
			if (entry.player == target)
			{
				bool ready = (bool)changes["Ready"];
				entry.SetReady(ready);

				if (ready) readyCount++;
				else readyCount--;
			}
		}
		LogManager.Log("씨발련아 왜 그래");
	}

	private void CancleButtonClick()
	{
		PhotonNetwork.LeaveRoom();
	}

	private void StartButtonClick()
	{
		PhotonNetwork.LoadLevel("GameScene");
	}



	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	public override void OnRoomPropertiesUpdate(Hashtable properties)
	{
		if (properties.ContainsKey("Difficulty"))
		{
			OnDifficultyChange((Difficulty)properties["Difficulty"]);
		}
	}

	public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		if (changedProps.ContainsKey("Ready"))
		{
			OnReadySelectChange(targetPlayer, changedProps);
			if (playerList.childCount == readyCount)
			{
				startButton.interactable = true;
			}
			else startButton.interactable = false;
		}

		if (changedProps.ContainsKey("CharacterSelect"))
		{
			OnCharacterSelectChange(targetPlayer, changedProps);
		}
	}

	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		OnMasterClientChanged();
	}

	private void OnMasterClientChanged()
	{
		difficultyDropdown.gameObject.SetActive(PhotonNetwork.IsMasterClient);
		startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
	}
}

