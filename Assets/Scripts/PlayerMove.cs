using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerMove : PlayerStateBase, IPunObservable, IInteractionInterface
{
    public float trackingSpeed = 30;
    public PlayerUI healthUI;
    public Vector3 shakePower;

    Transform cam;
    CharacterController cc;
    Animator myAnim;
    PhotonView pv;
    Vector3 myPos;
    Quaternion myRot;
    Vector3 myPrevPos;
    bool isShaking = false;

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

        //���� ü���� �ʱ�ȭ�Ѵ�.
        currentHealth = maxHealth;

        //���̾ �����Ѵ�.
        gameObject.layer = pv.IsMine ? LayerMask.NameToLayer("Player") : LayerMask.NameToLayer("Enemy");

        playerState = PlayerState.RUN;

    }

    void Update()
    {
        if (playerState == PlayerState.RUN)
        {
            Move();
            Rotate();
        }
        #region ������
        //if(Input.GetKeyDown(KeyCode.I))
        //{
        //    StartCoroutine(ShakeCamera(5, 20, 0.3f));
        //}
        #endregion

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

    public void RPC_TakeDamage(float dmg, int viewID)
    {
        pv.RPC("TakeDamage", RpcTarget.All, dmg, viewID);
    }

    [PunRPC]
    public void TakeDamage(float dmg, int viewID)
    {
        if (playerState != PlayerState.RUN)
        {
            return;
        }

        currentHealth = Mathf.Max(currentHealth - dmg, 0);
        healthUI.SetHPValue(currentHealth, maxHealth);

        if (currentHealth > 0)
        {
            // �ǰ� ȿ�� ó��
            // ī�޶� ���� ȿ���� �ش�.
            if (!isShaking && pv.IsMine)
            {
                StartCoroutine(ShakeCamera(5, 20, 0.3f));
            }

        }
        else
        {
            // ���� ó��
            DieProcess();
        }
    }
    IEnumerator ShakeCamera(float amplitude, float frequency, float duration)
    {
        isShaking = true;
        CameraFollow camFollow = Camera.main.transform.GetComponent<CameraFollow>();
        if (camFollow != null)
        {
            camFollow.isShaking = true;
        }

        // duration��ŭ Perlin Noise�� ���� �����ͼ� �� ����ŭ x��� y���� ȸ����Ų��.
        float currentTime = 0;
        float delayTime = 1.0f / frequency;
        Quaternion originRot = Camera.main.transform.localRotation;

        while (currentTime < duration)
        {
            float range1 = Mathf.PerlinNoise1D(currentTime) - 0.5f;
            float range2 = Mathf.PerlinNoise1D(duration - currentTime) - 0.5f;
            float xRot = range1 * shakePower.x * amplitude;
            float yRot = range2 * shakePower.y * amplitude;

            Camera.main.transform.Rotate(xRot, yRot, 0);

            yield return new WaitForSeconds(delayTime);
            currentTime += delayTime;
        }

        Camera.main.transform.localRotation = originRot;
        isShaking = false;
        if (camFollow != null)
        {
            camFollow.isShaking = false;
        }
    }

    void DieProcess()
    {
        if (pv.IsMine)
        {
            // ȭ���� ������� ó���Ѵ�.
            Volume currentVolume = FindObjectOfType<Volume>();
            ColorAdjustments postColor;
            currentVolume.profile.TryGet<ColorAdjustments>(out postColor);
            postColor.saturation.value = -10000;
        }

        // ���� �ִϸ��̼��� �����Ѵ�.
        myAnim.SetTrigger("Die");
        // capsule Collider�� ��Ȱ��ȭ�Ѵ�.
        GetComponent<CapsuleCollider>().enabled = false;
        // �������� ���� ���·� ��ȯ�Ѵ�.
        playerState = PlayerState.DIE;
        // �ִϸ��̼��� ������ �÷��̾ �����Ѵ�.
    }
}
