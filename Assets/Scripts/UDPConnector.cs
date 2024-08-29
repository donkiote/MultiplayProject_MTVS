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
    //public ReceivedPoseList receivedPoseList;

    Thread udpThread;
    UdpClient receivePort;
    UdpClient sendPort;

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
                foreach (PoseData poseData in jsonData.landmarkList)
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

    void SendData(string message)
    {
        // 클라이언트로서 준비
        sendPort = new UdpClient(portNumber);

        // 데이터를 전송한다.
        byte[] binData = Encoding.UTF8.GetBytes(message);
        sendPort.Send(binData, binData.Length, "168.12.0.1", 7000);
    }

    private void OnDisable()
    {
        // UDP 스트림을 종료한다.
        receivePort.Close();
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

public enum PoseName
{
    nose,
    left_eye_inner,
    left_eye,
    left_eye_outer,
    right_eye_inner,
    right_eye,
    right_eye_outer,
    left_ear,
    right_ear,
    mouth_left,
    mouth_right,
    left_shoulder,
    right_shoulder,
    left_elbow,
    right_elbow,
    left_wrist,
    right_wrist,
    left_pinky,
    right_pinky,
    left_index,
    right_index,
    left_thumb,
    right_thumb,
    left_hip,
    right_hip,
    left_knee,
    right_knee,
    left_ankle,
    right_ankle,
    left_heel,
    right_heel,
    left_foot_index,
    right_foot_index
}

//[System.Serializable]
//public struct ReceivedPoseList
//{
//    public Vector3 nose;
//    public Vector3 left_eye_inner;
//    public Vector3 left_eye;
//    public Vector3 left_eye_outer;
//    public Vector3 right_eye_inner;
//    public Vector3 right_eye;
//    public Vector3 right_eye_outer;
//    public Vector3 left_ear;
//    public Vector3 right_ear;
//    public Vector3 mouth_left;
//    public Vector3 mouth_right;
//    public Vector3 left_shoulder;
//    public Vector3 right_shoulder;
//    public Vector3 left_elbow;
//    public Vector3 right_elbow;
//    public Vector3 left_wrist;
//    public Vector3 right_wrist;
//    public Vector3 left_pinky;
//    public Vector3 right_pinky;
//    public Vector3 left_index;
//    public Vector3 right_index;
//    public Vector3 left_thumb;
//    public Vector3 right_thumb;
//    public Vector3 left_hip;
//    public Vector3 right_hip;
//    public Vector3 left_knee;
//    public Vector3 right_knee;
//    public Vector3 left_ankle;
//    public Vector3 right_ankle;
//    public Vector3 left_heel;
//    public Vector3 right_heel;
//    public Vector3 left_foot_index;
//    public Vector3 right_foot_index;

//    public void SetVectorData(List<Vector3> list)
//    {

//    }
//}