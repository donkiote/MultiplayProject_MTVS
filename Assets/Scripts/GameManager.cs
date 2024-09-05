using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class GameManager : MonoBehaviourPun
{
    public TMP_Text text_playerList;

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

        Vector2 randomPos = Random.insideUnitCircle * 5.0f;
        Vector3 initPosition = new Vector3(randomPos.x, 1.0f, randomPos.y);

        GameObject player = PhotonNetwork.Instantiate("Player", initPosition, Quaternion.identity);
        
        
    }

    
    void Update()
    {
        Dictionary<int, Player> playerDict = PhotonNetwork.CurrentRoom.Players;

        List<string> playerName = new List<string>();
        string masterName = "";
        foreach (KeyValuePair<int, Player> player in playerDict) 
        {
            if (player.Value.IsMasterClient)
            {
                masterName = player.Value.NickName;
            }
            else 
            {
                playerName.Add(player.Value.NickName);
            }
            
        }
        playerName.Sort();
        text_playerList.text = "<size=60><color=#ff0000>"+masterName+"</size></color>\n";
        foreach (string name in playerName)
        {
            text_playerList.text += name + "\n";
        }
    }
}
