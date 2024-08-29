using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerStateBase
{
    Transform cam;
    CharacterController cc;
    Animator myAnim;

    float mx = 0;

    void Start()
    {
        cam = Camera.main.transform;
        cc = GetComponent<CharacterController>();
        myAnim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Move();
        Rotate();
    }

    void Move()
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

        if(myAnim != null)
        {
            myAnim.SetFloat("Horizontal", h);
            myAnim.SetFloat("Vertical", v);
        }
    }

    void Rotate()
    {
        // 사용자의 마우스 좌우 드래그 입력을 받는다.
        mx += Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime;

        // 입력받은 방향에 따라 플레이어를 좌우로 회전한다.
        transform.eulerAngles = new Vector3(0, mx, 0);

    }
}
