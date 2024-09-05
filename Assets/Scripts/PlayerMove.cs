using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMove : PlayerStateBase, IPunObservable
{
    public float trackingSpeed = 30;
    

    Transform cam;
    CharacterController cc;
    Animator myAnim;
    PhotonView pv;
    Vector3 myPos;
    Quaternion myRot;
    Vector3 myPrevPos;

    float mx = 0;
    //float h, v = 0;
    //float prevH, prevV = 0;

    void Start()
    {
        cam = Camera.main.transform;
        cc = GetComponent<CharacterController>();
        myAnim = GetComponentInChildren<Animator>();
        pv = GetComponent<PhotonView>();
        myPrevPos = transform.position;
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
            Vector3 targetPos = Vector3.Lerp(transform.position,myPos, Time.deltaTime* trackingSpeed);

            

            float dist = (targetPos - myPrevPos).magnitude;
            transform.position = dist > 0.01f ? targetPos : myPos;
            //Vector2 animPos = dist > 0.01f ? Vector2.one : Vector2.zero;

            Vector3 localDir = transform.InverseTransformDirection(targetPos - myPrevPos);

            float deltaX = localDir.x;
            float deltaZ = localDir.z;

            float newX =0;
            float newZ =0;
            if (Mathf.Abs(deltaX) > 0.01f)
            {
                newX = deltaX > 0 ? 1.0f : -1.0f;
                
            }
            if (Mathf.Abs(deltaZ) > 0.01f)
            {
                newZ = deltaZ > 0 ? 1.0f : -0.0f;
            }
            myPrevPos = transform.position;
            /*h = Mathf.Lerp(prevH, h, Time.deltaTime * animSpeed);
            v = Mathf.Lerp(prevV, v, Time.deltaTime * animSpeed);*/

            myAnim.SetFloat("Horizontal", newX);
            myAnim.SetFloat("Vertical", newZ);

            /*prevH = h;
            prevV = v;*/

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
            //stream.SendNext(new Vector2(h,v));
        }
        //�׷����ʰ�, ���� �����͸� ������ ���� �о���� ���¶��
        else if(stream.IsReading)
        {
            myPos = (Vector3)stream.ReceiveNext();
            myRot = (Quaternion)stream.ReceiveNext();
            /*Vector2 inputValue = (Vector2)stream.ReceiveNext();
            h = inputValue.x;
            v = inputValue.y;*/
        }

    }

}
