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

        //현재 체력을 초기화한다.
        currentHealth = maxHealth;

        //레이어를 변경한다.
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
        #region 디버깅용
        //if(Input.GetKeyDown(KeyCode.I))
        //{
        //    StartCoroutine(ShakeCamera(5, 20, 0.3f));
        //}
        #endregion

    }

    void Move()
    {
        //만일,내가 소유권을 가진 캐릭터라면..
        if (pv.IsMine)
        {
            // 현재 카메라가 바라보는 방향으로 이동을 하고 싶다.
            // 이동의 조작 방식은 W,A,S,D 키를 이용한다.
            // 캐릭터 컨트롤러 클래스의 Move 함수를 이용한다.
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
            // 사용자의 마우스 좌우 드래그 입력을 받는다.
            mx += Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime;

            // 입력받은 방향에 따라 플레이어를 좌우로 회전한다.
            transform.eulerAngles = new Vector3(0, mx, 0);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, myRot, Time.deltaTime* trackingSpeed);
            
        }
        

    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //만일, 데이터를 서버에 전송(PhotonView.IsMine =true)하는 상태라면..
        if (stream.IsWriting)
        {
            //iterable 데이터를 보낸다.
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            //stream.SendNext(new Vector2(h,v));
        }
        //그렇지않고, 만일 데이터를 서버로 부터 읽어오는 상태라면
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
            // 피격 효과 처리
            // 카메라를 흔드는 효과를 준다.
            if (!isShaking && pv.IsMine)
            {
                StartCoroutine(ShakeCamera(5, 20, 0.3f));
            }

        }
        else
        {
            // 죽음 처리
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

        // duration만큼 Perlin Noise의 값을 가져와서 그 값만큼 x축과 y축을 회전시킨다.
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
            // 화면을 흑백으로 처리한다.
            Volume currentVolume = FindObjectOfType<Volume>();
            ColorAdjustments postColor;
            currentVolume.profile.TryGet<ColorAdjustments>(out postColor);
            postColor.saturation.value = -10000;
        }

        // 죽음 애니메이션을 실행한다.
        myAnim.SetTrigger("Die");
        // capsule Collider를 비활성화한다.
        GetComponent<CapsuleCollider>().enabled = false;
        // 움직임을 죽음 상태로 전환한다.
        playerState = PlayerState.DIE;
        // 애니메이션이 끝나면 플레이어를 제거한다.
    }
}
