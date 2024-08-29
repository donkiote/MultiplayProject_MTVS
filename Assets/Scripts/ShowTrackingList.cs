using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTrackingList : MonoBehaviour
{
    public UDPConnector conn;


    void Start()
    {
        
    }

    void Update()
    {
        // 만일, 트래킹된 데이터가 있다면...
        if(conn.receivedPoseList.Count > 0)
        {
            // 트래킹된 벡터 값을 모든 자식 오브젝트의 로컬 위치 값으로 전달한다.
            for(int i = 0; i < conn.receivedPoseList.Count; i++)
            {
                transform.GetChild(i).localPosition = conn.receivedPoseList[i];
            }
        }
    }
}
