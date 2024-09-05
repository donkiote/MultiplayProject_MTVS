using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{

    void Start()
    {
        StartCoroutine(SpawnPlayer());

        //OnPhotonSerializView ���� ������ ���� �� �� �����ϱ� (per seconds)
        PhotonNetwork.SerializationRate = 30;
        //��κ��� ������ ���� �� �� �����ϱ�(per seconds)
        PhotonNetwork.SendRate = 30;
    }
    IEnumerator SpawnPlayer()
    {
        //�뿡 ������ �Ϸ�� ������ ��ٸ���.
        yield return new WaitUntil(() => { return PhotonNetwork.InRoom; });

        Vector2 randomPos = Random.insideUnitCircle;
        Vector3 initPosition = new Vector3(randomPos.x, 1.0f, randomPos.y);

        GameObject player = PhotonNetwork.Instantiate("Player", initPosition, Quaternion.identity);
        
        
    }

    
    void Update()
    {
        
    }
}
