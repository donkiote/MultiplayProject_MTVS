using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Net;

public class UDPConnector : MonoBehaviour
{
    public int portNumber = 5000;
    public List<Vector3> receivedPoseList = new List<Vector3>();

    Thread udpThread;
    UdpClient receivePort;

    void Start()
    {
        InitializeUDPThread();
    }

    void InitializeUDPThread()
    {
        // 백그라운드에서 새 Thread를 실행하고 싶다.
        udpThread = new Thread(new ThreadStart(ReceiveData));
        udpThread.IsBackground = true;
        udpThread.Start();
    }    

    void ReceiveData()
    {
        // 서버 오픈 및 원격 클라이언트를 설정한다.
        receivePort = new UdpClient(portNumber);
        IPEndPoint remoteClient = new IPEndPoint(IPAddress.Any, portNumber);
        try
        {
            while (true)
            {
                // 통신 결과로 바이너리 데이터를 받는다.
                byte[] bins = receivePort.Receive(ref remoteClient);
                string binaryString = Encoding.UTF8.GetString(bins);
                //print($"받은 데이터: {binaryString}");

                PoseList jsonData = JsonUtility.FromJson<PoseList>(binaryString);

                receivedPoseList.Clear();

                // 변환된 json 배열 데이터를 벡터 형태의 리스트로 저장한다.
                foreach(PoseData poseData in jsonData.landmarkList)
                {
                    Vector3 receiveVector = new Vector3(poseData.x, poseData.y, poseData.z);
                    receivedPoseList.Add(receiveVector);
                }
            }
        }
        catch (SocketException message)
        {
            // 통신 에러 코드 및 에러 내용을 출력한다.
            Debug.LogError($"Error Code {message.ErrorCode} - {message}");
        }
        finally
        {
            receivePort.Close();
        }
    }

    private void OnDisable()
    {
        // UDP 스트림을 종료한다.
        receivePort.Close();
    }
    void Update()
    {
        
    }
}


[System.Serializable]
public struct PoseData
{
    public float x;
    public float y;
    public float z;
}

[System.Serializable]
public struct PoseList
{
    public List<PoseData> landmarkList;
}