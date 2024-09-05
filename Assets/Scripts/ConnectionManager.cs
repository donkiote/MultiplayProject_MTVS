using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;//
using Photon.Realtime;//
using System.Reflection;
using System;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class ConnectionManager : MonoBehaviourPunCallbacks
{
    public GameObject roomPrefab;
    public Transform scrollContent;
    public GameObject[] panelList;

    List<RoomInfo> cachedRoomList = new List<RoomInfo>();
    void Start()
    {
        Screen.SetResolution(640, 480, FullScreenMode.Windowed);
    }

    void Update()
    {
        
    }
    public void StartLogin()
    {
        //������ ���� ����
        if(LobbyUIController.lobbyUI.input_nickName.text.Length>0)
        {
            PhotonNetwork.GameVersion = "1.0.0";
            PhotonNetwork.NickName = LobbyUIController.lobbyUI.input_nickName.text;
            PhotonNetwork.AutomaticallySyncScene = true;
            //������ ������ ��û�ϱ�
            PhotonNetwork.ConnectUsingSettings();//@1
            LobbyUIController.lobbyUI.btn_login.interactable = false;
        }
        
    }
    public override void OnConnected()
    {
        base.OnConnected();

        //���� ������ ������ �Ϸ�Ǿ����� �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name+"is call");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        //���� ������ ����Ѵ�.
        Debug.LogError("Disconnected from Server - " + cause);
        LobbyUIController.lobbyUI.btn_login.interactable = true;
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        //������ ������ ������ �Ϸ�Ǿ����� �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name + "is call");

        //������ �κ�� ����.
        PhotonNetwork.JoinLobby();//@2
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        //�����κ� ������ �˷���
        print(MethodInfo.GetCurrentMethod().Name + "is call");
        LobbyUIController.lobbyUI.ShowRoomPanel();
    }
    
    public void CreateRoom()
    {
        string roomName = LobbyUIController.lobbyUI.roomSetting[0].text;
        int playerCount = Convert.ToInt32(LobbyUIController.lobbyUI.roomSetting[1].text);

        if (roomName.Length > 0 && playerCount > 1)
        {
            //���� ���� �����.
            RoomOptions roomOpt = new RoomOptions();
            roomOpt.MaxPlayers = playerCount;
            roomOpt.IsOpen = true;
            roomOpt.IsVisible = true;//�˻�����

            //Ŀ���� ������ �߰��Ѵ�.
            //Ű �� ����ϱ�
            roomOpt.CustomRoomPropertiesForLobby = new string[] { "MASTER_NAME" , "PassWord" };
            //Ű�� �´� �ؽ� ���̺� �߰�
            Hashtable roomTable = new Hashtable();
            roomTable.Add("MASTER_NAME", PhotonNetwork.NickName);
            roomTable.Add("PassWord", 1234);
            roomOpt.CustomRoomProperties = roomTable;

            PhotonNetwork.CreateRoom(roomName, roomOpt, TypedLobby.Default);//@3
        }

        
    }
    public void JoinRoom()
    {
        /*string roomName = LobbyUIController.lobbyUI.roomSetting[0].text;

        if (roomName.Length > 0)
        {
            PhotonNetwork.JoinRoom(roomName);//@4
        }*/

        //join ���� �г� Ȱ��ȭ
        ChangePanel(1, 2);
    }
    ///<summary>
    ///�г��� ������ �ϱ� ���� �Լ�
    ///</summary>
    ///<param name="offIndex">���ߵ� �г� �ε���</param>
    ///<param name="onIndex">�Ѿߵ� �г� �ε���</param>
    
    void ChangePanel(int offIndex, int onIndex)
    {
        panelList[offIndex].SetActive(false);
        panelList[onIndex].SetActive(true);

    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        //���������� �� ���� �˸�
        print(MethodInfo.GetCurrentMethod().Name + "is call");
        LobbyUIController.lobbyUI.PrintLog("�游�����!");
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        //���������� �濡 �����
        print(MethodInfo.GetCurrentMethod().Name + "is call");
        LobbyUIController.lobbyUI.PrintLog("�����强��!");

        //�濡 ������ ģ������ ��� 1���� �̵�����
        PhotonNetwork.LoadLevel(1);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        //�뿡 ������ ������ ���� ���
        Debug.LogError(message);
        LobbyUIController.lobbyUI.PrintLog("���� ����!"+message);

        
    }

    //�뿡 �ٸ� �÷��̾ ���� ������ �ݹ� �Լ�
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        string playerMsg = $"{newPlayer.NickName}���� �����ϼ̽��ϴ�.";
        LobbyUIController.lobbyUI.PrintLog(playerMsg);
    }
    //�뿡 �ٸ� �÷��̾ ���������� �ݹ� �Լ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        string playerMsg = $"{otherPlayer.NickName}���� �����ϼ̽��ϴ�.";
        LobbyUIController.lobbyUI.PrintLog(playerMsg);
    }
    //���� �κ񿡼� �뿡 ������� �˷��ִ� �ݹ� �Լ�
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        foreach (RoomInfo room in roomList)
        {
            //����, ���ŵ� �� ������ ���� ����Ʈ�� �ִٸ�..
            if (room.RemovedFromList)
            {
                //cachedRoomList���� �ش� ���� ���� �����Ѵ�.
                cachedRoomList.Remove(room);
            }
            //�׷��� �ʴٸ�..
            else
            {
                //����, �̹� cachedRoomList�� �ִ� ���̶��...
                if (cachedRoomList.Contains(room))
                {
                    //���� �� ������ �����Ѵ�.
                    cachedRoomList.Remove(room);
                }
                //�� ���� cachedRoomList�� �߰��Ѵ�.
                cachedRoomList.Add(room);
            }
        }
        //������ ��� �� ���� ����
        for (int i = 0; i < scrollContent.childCount; i++)
        {
            Destroy(scrollContent.GetChild(i).gameObject);
        }

        foreach (RoomInfo room in cachedRoomList)
        {
            //cachedRoomList�� �ִ� ��� ���� ���� ��ũ�Ѻ信 �߰��Ѵ�.
            GameObject go = Instantiate(roomPrefab, scrollContent);
            RoomPanel roomPanel = go.GetComponent<RoomPanel>();
            roomPanel.SetRoomInfo(room);
            //��ư�� �� ���� ��� �����ϱ�
            roomPanel.btn_join.onClick.AddListener(() =>
            {
                PhotonNetwork.JoinRoom(room.Name);
            });

        }
    }
}
