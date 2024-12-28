using Photon.Chat;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
	private Animator anim;
	private Rigidbody rb;
	//캐릭터가 쳐다볼 곳
	private Transform pointer;
	//투사체가 생성될 곳
	private Transform shotPoint;

	private int shotCount = 0;
	private float hp = 1;

	public float moveSpeed = 10;
	public float shotPower = 5;

	//체력 표시 text
	public Text hpText;
	//발사 횟수 표시 text
	public Text shotText;

	public Bomb bombPrefab;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();

		pointer = transform.Find("PlayerPointer");
		shotPoint = transform.Find("ShotPoint");
		tag = photonView.IsMine ? "Player" : "Enemy";
	}

	private void Update()
	{
		hpText.text = hp.ToString();
		shotText.text = shotCount.ToString();
		if (false == photonView.IsMine) return;
		Move();
		if (Input.GetButtonDown("Fire1"))
		{   //이 호출은 로컬에서만 되니까 이렇게하면 안됨.
			//Fire();

			//RPC를 통해서 Fire메서드를 호출하고 RPC에 자신 포함 모든 유닛에게 실행시키도록 함.
			photonView.RPC("Fire", RpcTarget.All, shotPoint.position, shotPoint.forward);
			shotCount++;
			anim.SetTrigger("Attack");
		}
	}

	private void FixedUpdate()
	{
		if (false == photonView.IsMine) return;
		Rotate();
	}

	private void Move()
	{
		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");
		rb.velocity = new Vector3(x, 0, z) * moveSpeed;

		anim.SetBool("IsMoving", rb.velocity.magnitude > 0.01f);
	}

	private void Rotate()
	{
		Vector3 pos = rb.position;
		pos.y = 0;
		Vector3 forward = pointer.position - pos;
		rb.rotation = Quaternion.LookRotation(forward, Vector3.up);
		rb.angularVelocity = Vector3.zero;
	}

	private void Hit(float damage)
	{
		hp -= damage;
		if (hp <= 0)
		{
			Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
			customProperties.Add("Die", 0);
			PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
		}
	}

	private void Heal(float amount)
	{
		hp += amount;
		if (hp < 100) hp = 100;
	}

	//Fire를 통해서 생성하는 bomb객체는 "데드레커닝"(추측항법 알고리즘)을 통해서
	//각 클라이언트들이 직접 생성하고, Fire함수를 호출 받는 시점을 온라인으로 호출받음.(Remote Procedure Call)
	[PunRPC]
	private void Fire(Vector3 shotPoint, Vector3 shotDir, PhotonMessageInfo info)
	{
		//여기에 이거 쓰면 추측항법 못쓴다.
		//if (false == photonView.IsMine) return;

		print($"Fire Procedure called by {info.Sender.NickName}");
		print($"My local time : {PhotonNetwork.Time}");
		print($"Server time when procedure called : {info.SentServerTime}");

		//"지연보상" : (추측항법을 위해) RPC를 받은 시점은 서버에서 호출된 시간보다 항상 늦기 때문에,
		//해당 지연시간만큼 위치, 또는 연산량을 보정해주어야 최대한 원격에서의 플레이가 동기화될 수 있음.

		//보정해야 할 지연시간
		float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

		Bomb bomb = Instantiate(bombPrefab, shotPoint, Quaternion.identity);
		bomb.rb.AddForce(shotDir * shotPower, ForceMode.Impulse);
		bomb.owner = photonView.Owner;

		//지연보상 들어간다.
		bomb.rb.position += bomb.rb.velocity * lag;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{   //Stream을 통해 주고받는 데이터는 Server에서 받는 시간 기준으로 Queue형태로 전달
		//데이터 자체도 큐
		if (stream.IsWriting)
		{   //내 데이터를 Server로 보냄
			stream.SendNext(hp);
			stream.SendNext(shotCount);
		}
		else
		{
			hp = (float)stream.ReceiveNext();
			shotCount = (int)stream.ReceiveNext();
		}
	}

}
