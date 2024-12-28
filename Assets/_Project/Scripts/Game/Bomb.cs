using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
	public Rigidbody rb;
	public ParticleSystem particlePrefab;
	public Player owner;

	private void Reset()
	{
		rb = GetComponent<Rigidbody>();
		particlePrefab = Resources.Load<ParticleSystem>("BombParticle");
	}

	private void OnTriggerEnter(Collider other)
	{
		ParticleSystem particle = Instantiate(particlePrefab, transform.position, particlePrefab.transform.rotation);
		particle.Play();
		Destroy(particle.gameObject, 3);

		//충돌 즉시 Renderer와 Collider 비활성화하여 실질적인 역할을 하지 않도록.
		GetComponent<Renderer>().enabled = false;
		GetComponent<Collider>().enabled = false;

		Destroy(gameObject, 0.1f);

		Collider[] contectedColliders = Physics.OverlapSphere(transform.position, 1.5f);

		foreach (Collider collider in contectedColliders)
		{
			if (collider.tag == "Player")
			{
				collider.SendMessage("Hit", 1);
				PhotonView target = collider.GetComponent<PhotonView>();
				print($"{owner.NickName}이 던진 폭탄에 {target.Owner.NickName}가 맞음");
			}
		}
	}
}