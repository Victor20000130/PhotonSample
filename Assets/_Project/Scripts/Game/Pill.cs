using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRan = UnityEngine.Random;

public class Pill : MonoBehaviourPun
{
	public Renderer render;

	//���� ����
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
	{   //PhotonNetwork.Instantiate ȣ�� �� �Բ� ���� Data �Ķ����
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