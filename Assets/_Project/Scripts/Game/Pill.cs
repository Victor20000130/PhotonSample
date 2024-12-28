using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pill : MonoBehaviourPun
{
	public Renderer render;

	//힐량 랜덤
	private float healAmount;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			other.SendMessage("Heal", healAmount);
		}
		Destroy(gameObject);
	}

	private void Awake()
	{   //PhotonNetwork.Instantiate 호출 시 함께 보낸 Data 파라미터
		object[] param = photonView.InstantiationData;
		if (param != null)
		{
			Vector3 cv = (Vector3)param[0];
			float healAmount = (float)param[1];

			render.material.color = new Color(cv.x, cv.y, cv.z);
			this.healAmount = healAmount;
		}
	}

	private void Reset()
	{
		render = GetComponentInChildren<Renderer>();
	}
}