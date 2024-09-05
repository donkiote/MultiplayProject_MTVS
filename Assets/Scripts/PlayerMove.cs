using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerStateBase, IPunObservable
{
    public float trackingSpeed = 3;

    Transform cam;
    CharacterController cc;
    Animator myAnim;
    PhotonView pv;
    Vector3 myPos;
    Quaternion myRot;

    float mx = 0;

    void Start()
    {
        cam = Camera.main.transform;
        cc = GetComponent<CharacterController>();
        myAnim = GetComponentInChildren<Animator>();
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        Move();
        Rotate();
    }

    void Move()
    {
        //����,���� �������� ���� ĳ���Ͷ��..
        if (pv.IsMine)
        {
            // ���� ī�޶� �ٶ󺸴� �������� �̵��� �ϰ� �ʹ�.
            // �̵��� ���� ����� W,A,S,D Ű�� �̿��Ѵ�.
            // ĳ���� ��Ʈ�ѷ� Ŭ������ Move �Լ��� �̿��Ѵ�.
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 dir = new Vector3(h, 0, v);
            dir.Normalize();
            dir = transform.TransformDirection(dir);
            cc.Move(dir * moveSpeed * Time.deltaTime);

            if (myAnim != null)
            {
                myAnim.SetFloat("Horizontal", h);
                myAnim.SetFloat("Vertical", v);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position,myPos, Time.deltaTime* trackingSpeed);

        }

    }


    void Rotate()
    {
        if (pv.IsMine)
        {
            // ������� ���콺 �¿� �巡�� �Է��� �޴´�.
            mx += Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime;

            // �Է¹��� ���⿡ ���� �÷��̾ �¿�� ȸ���Ѵ�.
            transform.eulerAngles = new Vector3(0, mx, 0);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, myRot, Time.deltaTime* trackingSpeed);
        }
        

    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //����, �����͸� ������ ����(PhotonView.IsMine =true)�ϴ� ���¶��..
        if (stream.IsWriting)
        {
            //iterable �����͸� ������.
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        //�׷����ʰ�, ���� �����͸� ������ ���� �о���� ���¶��
        else if(stream.IsWriting)
        {
            myPos = (Vector3)stream.ReceiveNext();
            myRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
