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
    //ĳ���Ͱ� �Ĵٺ� ��
    private Transform pointer;
    //����ü�� ������ ��
    private Transform shotPoint;

    private int shotCount = 0;
    private float hp = 100;

    public float moveSpeed = 10;
    public float shotPower = 5;

    //ü�� ǥ�� text
    public Text hpText;
    //�߻� Ƚ�� ǥ�� text
    public Text shotText;

    public Bomb bombPrefab;

    private Player player;
    private bool IsMine => player == PhotonNetwork.LocalPlayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        pointer = transform.Find("PlayerPointer");
        shotPoint = transform.Find("ShotPoint");
        tag = photonView.IsMine ? "Player" : "Enemy";

    }

    private void Start()
    {
        if (false == PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("LoadedPlayer"))
        {
            PhotonNetwork.LocalPlayer.CustomProperties.Add("LoadedPlayer", 0f);
        }
        Time.timeScale = 0f;
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.LocalPlayer.GetPlayerNumber() + 1)
        {
            Hashtable customProps = PhotonNetwork.LocalPlayer.CustomProperties;
            customProps["LoadedPlayer"] = 1f;
            PhotonNetwork.LocalPlayer.SetCustomProperties(customProps);
        }
    }

    private void Update()
    {
        hpText.text = hp.ToString();
        shotText.text = shotCount.ToString();
        if (false == photonView.IsMine) return;
        Move();
        if (Input.GetButtonDown("Fire1"))
        {   //�� ȣ���� ���ÿ����� �Ǵϱ� �̷����ϸ� �ȵ�.
            //Fire();

            //RPC�� ���ؼ� Fire�޼��带 ȣ���ϰ� RPC�� �ڽ� ���� ��� ���ֿ��� �����Ű���� ��.
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
    }

    private void Heal(float amount)
    {
        hp += amount;
        if (hp < 100) hp = 100;
    }

    //Fire�� ���ؼ� �����ϴ� bomb��ü�� "���巹Ŀ��"(�����׹� �˰���)�� ���ؼ�
    //�� Ŭ���̾�Ʈ���� ���� �����ϰ�, Fire�Լ��� ȣ�� �޴� ������ �¶������� ȣ�����.(Remote Procedure Call)
    [PunRPC]
    private void Fire(Vector3 shotPoint, Vector3 shotDir, PhotonMessageInfo info)
    {
        //���⿡ �̰� ���� �����׹� ������.
        //if (false == photonView.IsMine) return;

        print($"Fire Procedure called by {info.Sender.NickName}");
        print($"My local time : {PhotonNetwork.Time}");
        print($"Server time when procedure called : {info.SentServerTime}");

        //"��������" : (�����׹��� ����) RPC�� ���� ������ �������� ȣ��� �ð����� �׻� �ʱ� ������,
        //�ش� �����ð���ŭ ��ġ, �Ǵ� ���귮�� �������־�� �ִ��� ���ݿ����� �÷��̰� ����ȭ�� �� ����.

        //�����ؾ� �� �����ð�
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

        Bomb bomb = Instantiate(bombPrefab, shotPoint, Quaternion.identity);
        bomb.rb.AddForce(shotDir * shotPower, ForceMode.Impulse);
        bomb.owner = photonView.Owner;

        //�������� ����.
        bomb.rb.position += bomb.rb.velocity * lag;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {   //Stream�� ���� �ְ�޴� �����ʹ� Server���� �޴� �ð� �������� Queue���·� ����
        //������ ��ü�� ť
        if (stream.IsWriting)
        {   //�� �����͸� Server�� ����
            stream.SendNext(hp);
            stream.SendNext(shotCount);
        }
        else
        {
            hp = (float)stream.ReceiveNext();
            shotCount = (int)stream.ReceiveNext();
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("LoadedPlayer"))
        {
            OnLoadedPlayerChange(targetPlayer, changedProps);
        }
    }

    private void OnLoadedPlayerChange(Player targetPlayer, Hashtable changedProps)
    {
        if (player == targetPlayer)
        {
            Time.timeScale = (float)changedProps["LoadedPlayer"];
        }

    }
}
