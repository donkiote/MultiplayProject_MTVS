using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyUIController : MonoBehaviour
{
    public GameObject Panel_login;
    public Button btn_login;
    public TMP_InputField input_nickName;
    public GameObject panel_joinOnCreateRoom;
    public static LobbyUIController lobbyUI;
    public TMP_InputField[] roomSetting;
    public TMP_Text text_logText;

    string log;

    private void Awake()
    {
        if (lobbyUI == null)
        {
            lobbyUI = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }
    public void ShowRoomPanel()
    {
        btn_login.interactable = true;
        Panel_login.gameObject.SetActive(false);
        panel_joinOnCreateRoom.SetActive(true);
    }

    public void PrintLog(string message)
    {
        log += message + '\n';
        text_logText.text = log;
    }
}
