using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraController : MonoBehaviour
{
    public Transform[] basePositions;
    public Vector3 nearOffset;
    public Vector3 farOffset;
    public float distance = 5.0f;
    public float rotSpeed = 300;

    CameraFollow followCam;
    float scrollValue = 1;
    Vector3 minLocalPos;
    Vector3 maxLocalPos;
    float my;

    void Start()
    {
        // nearPos�� ��ġ�� �÷��̾��� �߾ӿ��� nearOffset��ŭ ���������� �����Ѵ�.
        minLocalPos = nearOffset;

        // farPos�� ��ġ�� nearPos�� ��ġ�κ��� farOffset��ŭ ���������� �����Ѵ�.
        Vector3 camDir = farOffset.normalized * distance;
        maxLocalPos = minLocalPos + camDir;

        //����, �� ĳ���Ͱ� ĳ������ �������ڶ��...
        // ���� ī�޶��� CameraFollow ������Ʈ���� follow Target�� LookAt Target�� ��� farPos�� �����Ѵ�.
        if (GetComponent<PhotonView>().IsMine)
        {
            followCam = Camera.main.transform.GetComponent<CameraFollow>();
            followCam.SetCameraFollowTarget(basePositions[1]);
            followCam.SetCameraLookAtTarget(basePositions[1]);
        }
        

        basePositions[0].localPosition = minLocalPos;
    }

    void Update()
    {
        basePositions[1].localPosition = SetCameraPosition();
        basePositions[1].localEulerAngles = SetCameraRotation();

    }

    Vector3 SetCameraPosition()
    {
        // ���콺 ���� �����̸� farPos�� ��ġ�� nearPos�� ��ġ�κ��� �÷��̾��� �� ������ �ִ� 5���ͱ����� �־����ų� ����������� �Ѵ�.
        scrollValue = Mathf.Clamp01(scrollValue - Input.GetAxis("Mouse ScrollWheel"));
        Vector3 newCampos = Vector3.Lerp(minLocalPos, maxLocalPos, scrollValue);

        // ��, nearPos�� farPos ���̿� ��ֹ��� �ִٸ� ��ֹ� �������� farPos�� ����(Player ���̾��� ��ֹ��� �ƴ�).
        Vector3 rayDirection = transform.TransformDirection(farOffset.normalized);
        Ray ray = new Ray(transform.position + minLocalPos, rayDirection);
        RaycastHit hitInfo;
        float rayDist = Mathf.Lerp(0, distance, scrollValue);
        if (Physics.Raycast(ray, out hitInfo, rayDist, ~(1 << 6)))
        {
            Vector3 calcPos = hitInfo.point + farOffset * -0.01f;
            newCampos = calcPos - transform.position;
            newCampos = Quaternion.Inverse(transform.rotation) * newCampos;
        }
        return newCampos;
    }

    Vector3 SetCameraRotation()
    {
        my -= Input.GetAxis("Mouse Y") * rotSpeed * Time.deltaTime;
        my = Mathf.Clamp(my, -30, 30);
        return new Vector3(my, 0, 0);
    }
}
