using UnityEngine;
using Photon.Pun;

public class ExampleScript : MonoBehaviour
{
    void Start()
    {
        // ...existing code...
        object[] myCustomData = new object[] { "exampleString", 123, true };
        byte myGroup = 1; // 그룹 1에 객체를 생성
        PhotonNetwork.Instantiate("MyPrefab", new Vector3(0, 0, 0), Quaternion.identity, myGroup, myCustomData);
        // ...existing code...
    }
}
