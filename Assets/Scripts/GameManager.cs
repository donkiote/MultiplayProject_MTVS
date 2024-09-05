using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{

    void Start()
    {
        StartCoroutine(SpawnPlayer());

        //OnPhotonSerializView 에서 데이터 전송 빈도 수 설정하기 (per seconds)
        PhotonNetwork.SerializationRate = 30;
        //대부분의 데이터 전송 빈도 수 설정하기(per seconds)
        PhotonNetwork.SendRate = 30;
    }
    IEnumerator SpawnPlayer()
    {
        //룸에 입장이 완료될 때까지 기다린다.
        yield return new WaitUntil(() => { return PhotonNetwork.InRoom; });

        Vector2 randomPos = Random.insideUnitCircle;
        Vector3 initPosition = new Vector3(randomPos.x, 1.0f, randomPos.y);

        GameObject player = PhotonNetwork.Instantiate("Player", initPosition, Quaternion.identity);
        
        
    }

    
    void Update()
    {
        
    }
}
