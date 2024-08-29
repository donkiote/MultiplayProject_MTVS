using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SetTrackingTransform : MonoBehaviour
{
    public UDPConnector conn;
    public PoseName targetPose;
    public PoseName hintPose;
    public Transform trackingList;

    void Start()
    {
        
    }

    void Update()
    {
        if(conn != null)
        {
            // 자식 오브젝트 중에서 각각 포즈 데이터 리스트에서 해당하는 번호의 벡터 값을 적용한다.
            //transform.GetChild(0).localPosition = conn.receivedPoseList[(int)targetPose];
            //transform.GetChild(1).localPosition = conn.receivedPoseList[(int)hintPose];

            transform.GetChild(0).position = trackingList.GetChild((int)targetPose).position;
            transform.GetChild(1).position = trackingList.GetChild((int)hintPose).position;
        }
    }
}
