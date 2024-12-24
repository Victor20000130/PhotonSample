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

public class RoomPanel : MonoBehaviour
{
	public Text roomTitleText;

	public Difficulty roomDifficulty;

	public Dropdown difficultyDropdown;

	public Text difficultyText;

	public RectTransform playerList;
	public GameObject playerTextPrefab;

	public Button startButton;
	public Button cancelButton;

	public List<PlayerEntry> playerEntries;

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

	private void OnEnable()
	{

		if (false == PhotonNetwork.InRoom) return;

		roomTitleText.text = PhotonNetwork.CurrentRoom.Name;

		foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
		{   //�÷��̾� ���� ��ü ����
			JoinPlayer(player);
		}

		//������ ��� IsMasterClient�� True�� �´�.
		//�ش� �Һ����� ���ؼ� ���常 ���̵��� ��ŸƮ�� �� �� �ֵ��� ����
		//������ ������ ������ �� �ڵ����� �ٸ� Ŭ���̾�Ʈ�� �����̵Ǵµ�,
		//�̶� ������ �ݺ����� Ȯ���� �� �ִٰ� �Ѵ�.
		difficultyDropdown.gameObject.SetActive(PhotonNetwork.IsMasterClient);
		startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
	}
	private void OnDisable()
	{
		foreach (Transform child in playerList)
		{   //�÷��̾� ����Ʈ�� �ٸ� ��ü�� ������ �ϴ� ��� ����
			Destroy(child.gameObject);
		}
	}

	public void JoinPlayer(Player newPlayer)
	{
		PlayerEntry playerEntry = Instantiate(playerTextPrefab, playerList,
			false).GetComponent<PlayerEntry>();
		playerEntries.Add(playerEntry);
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
				playerEntries.Remove(child.GetComponent<PlayerEntry>());
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
			}
		}
	}

	private void CancleButtonClick()
	{
		PhotonNetwork.LeaveRoom();
	}

	private void StartButtonClick()
	{
		PhotonNetwork.LoadLevel("GameScene");
	}
}

