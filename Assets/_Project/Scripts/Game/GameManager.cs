using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using Photon.Realtime;
using UniRan = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class GameManager : MonoBehaviourPunCallbacks
{

    public static GameManager Instance { get; private set; }

    public Transform playerPositions;

    public static bool isGameReady;

    public bool readyToBattle;

    private void Awake()
    {
        Instance = this;
    }

    //Photon에서 컨트롤 동기화 하는 방법

    //1. 프리팹에 PhotonView 컴포넌트를 붙이고,
    //PhotonNetwork.Instantiate를 통해
    //원격 클라이언트에게도 동기화된 오브젝트를 생성하도록 함.

    //2. PhotonView가 Observing할 수 있도록 View 컴포넌트를 부착.

    //3. 내 View가 부착되지 않은 오브젝트는 내가 제어하지 않도록 예외처리를 반드시 할 것.

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => isGameReady);
        //바로 실행하면 동기화 안될 때가 있어서 1초 대기
        yield return new WaitForSeconds(1f);
        //GetPlayerNumber 확장함수 :
        //포톤 네트워크에 연결된 다른 플레이어들 사이에서 동기화된 플레이어 번호.
        //Actor Number와 다름.(Scene마다 선착순으로 0~플레이어 수만큼 부여됨.)
        //GetPlayerNumber 확장함수가 동작하기 위해서는 씬에
        //PlayerNumbering 컴포넌트가 필요하다.
        int playerNum = PhotonNetwork.LocalPlayer.GetPlayerNumber();
        Vector3 playerPos = playerPositions.GetChild(playerNum).position;
        GameObject playerObj = PhotonNetwork.Instantiate("Player", playerPos, Quaternion.identity);
        playerObj.name = $"Player {playerNum}";

        Hashtable customProp = PhotonNetwork.LocalPlayer.CustomProperties;

        //PhotonNetwork.LocalPlayer.SetCustomProperties(customProp);

        //LogManager.Log(customProp["CharacterSelect"]);
        //이건 쓰레기 같이 찾아오는 방식임
        //다만 귀찮기 때문에 그냥 함.
        //문제점 : playerObj하위에 옵젝 많아지면 부하 심해짐.
        //장점 : 쓰레기 같지면 편하죠?
        //해결책 : 배열을 쓰던 뭘 쓰던 customProp의 값하고 매칭시키면 됨.
        switch (customProp["CharacterSelect"])
        {
            case 0:
                playerObj.transform.Find("Renderer").Find("Eyes").Find("Cube").gameObject.SetActive(true);
                break;
            case 1:
                playerObj.transform.Find("Renderer").Find("Eyes").Find("TwoEyes").gameObject.SetActive(true);
                break;
            case 2:
                playerObj.transform.Find("Renderer").Find("Eyes").Find("Lense").gameObject.SetActive(true);
                break;
        }

        //이 밑에서는 내가 MasterClient가 아니면 동작하지 않음
        if (false == PhotonNetwork.IsMasterClient)
        {
            yield break;
        }

        //Master Client만 5초마다 .Pill을 PhotonNetwork를 통해 Instantiate.
        while (true)
        {   //PhotonNetwork.Instantiate를 통해 생성할 경우,
            //position과 rotation이 반드시 필요.
            Vector3 spawnPos = UniRan.insideUnitSphere * 15;
            spawnPos.y = 0;
            Quaternion spawnRot = Quaternion.Euler(0, UniRan.Range(0, 180f), 0);

            //각 Pill마다 Random color(Color)와 Random healAmount(float)를 주입하고 싶으면?

            Vector3 color = new Vector3(UniRan.value, UniRan.value, UniRan.value);
            float healAmount = UniRan.Range(10f, 30f);

            PhotonNetwork.Instantiate("Pill", spawnPos, spawnRot,
                data: new object[] { color, healAmount });

            yield return new WaitForSeconds(5f);
        }




        //이렇게하면 각 클라이언트에 플레이어가 생성안됨.
        //로컬에서만 보임.(멀티 유저가 안보임)
        //GameObject playerPrefab = Resources.Load<GameObject>("Player");
        //Instantiate(playerPrefab).name = PhotonNetwork.NickName;

        //Vector3 spawnPos = playerPositions
        //	.GetChild(UniRan.Range(0, playerPositions.childCount)).position;
        ////이렇게 생성해야지 각 클라이언트 로컬에도 멀티 유닛이 생성이 된다.
        //PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity).name = PhotonNetwork.NickName;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {

    }
}
