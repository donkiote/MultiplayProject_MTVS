using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;


public class RoomPanel : MonoBehaviour
{
    public TMP_Text[] roomTexts = new TMP_Text[3];
    public Button btn_join;

    public void SetRoomInfo(RoomInfo room)
    {
        roomTexts[0].text = room.Name;
        roomTexts[1].text = $"({room.PlayerCount}/{room.MaxPlayers})";
        string masterName = room.CustomProperties["MASTER_NAME"].ToString();
        roomTexts[2].text = masterName;

    }
}
