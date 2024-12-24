using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UniRan = UnityEngine.Random;


public class GameManager : MonoBehaviour
{

	public static GameManager Instance { get; private set; }

	public Transform playerPositions;

	public static bool isGameReady;

	private void Awake()
	{
		Instance = this;
	}

	//Photon���� ��Ʈ�� ����ȭ �ϴ� ���

	//1. �����տ� PhotonView ������Ʈ�� ���̰�,
	//PhotonNetwork.Instantiate�� ����
	//���� Ŭ���̾�Ʈ���Ե� ����ȭ�� ������Ʈ�� �����ϵ��� ��.

	//2. PhotonView�� Observing�� �� �ֵ��� View ������Ʈ�� ����.

	//3. �� View�� �������� ���� ������Ʈ�� ���� �������� �ʵ��� ����ó���� �ݵ�� �� ��.

	private IEnumerator Start()
	{
		yield return new WaitUntil(() => isGameReady);
		//�ٷ� �����ϸ� ����ȭ �ȵ� ���� �־ 1�� ���
		yield return new WaitForSeconds(1f);

		//GetPlayerNumber Ȯ���Լ� :
		//���� ��Ʈ��ũ�� ����� �ٸ� �÷��̾�� ���̿��� ����ȭ�� �÷��̾� ��ȣ.
		//Actor Number�� �ٸ�.(Scene���� ���������� 0~�÷��̾� ����ŭ �ο���.)
		//GetPlayerNumber Ȯ���Լ��� �����ϱ� ���ؼ��� ����
		//PlayerNumbering ������Ʈ�� �ʿ��ϴ�.
		int playerNum = PhotonNetwork.LocalPlayer.GetPlayerNumber();

		Vector3 playerPos = playerPositions.GetChild(playerNum).position;
		GameObject playerObj = PhotonNetwork.Instantiate("Player", playerPos, Quaternion.identity);
		playerObj.name = $"Player {playerNum}";

		//�� �ؿ����� ���� MasterClient�� �ƴϸ� �������� ����
		if (false == PhotonNetwork.IsMasterClient)
		{
			yield break;
		}

		//Master Client�� 5�ʸ��� .Pill�� PhotonNetwork�� ���� Instantiate.
		while (true)
		{   //PhotonNetwork.Instantiate�� ���� ������ ���,
			//position�� rotation�� �ݵ�� �ʿ�.
			Vector3 spawnPos = UniRan.insideUnitSphere * 15;
			spawnPos.y = 0;
			Quaternion spawnRot = Quaternion.Euler(0, UniRan.Range(0, 180f), 0);

			//�� Pill���� Random color(Color)�� Random healAmount(float)�� �����ϰ� ������?

			Vector3 color = new Vector3(UniRan.value, UniRan.value, UniRan.value);
			float healAmount = UniRan.Range(10f, 30f);

			PhotonNetwork.Instantiate("Pill", spawnPos, spawnRot,
				data: new object[] { color, healAmount });

			yield return new WaitForSeconds(5f);
		}




		//�̷����ϸ� �� Ŭ���̾�Ʈ�� �÷��̾ �����ȵ�.
		//���ÿ����� ����.(��Ƽ ������ �Ⱥ���)
		//GameObject playerPrefab = Resources.Load<GameObject>("Player");
		//Instantiate(playerPrefab).name = PhotonNetwork.NickName;

		//Vector3 spawnPos = playerPositions
		//	.GetChild(UniRan.Range(0, playerPositions.childCount)).position;
		////�̷��� �����ؾ��� �� Ŭ���̾�Ʈ ���ÿ��� ��Ƽ ������ ������ �ȴ�.
		//PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity).name = PhotonNetwork.NickName;
	}

}