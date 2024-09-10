using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerFire : MonoBehaviourPun
{
    public Transform[] sockets;
    public WeaponInfo myWeapon;
    public Animator anim;

    PlayerUI playerUI;

    void Start()
    {
        myWeapon.weaponType = WeaponType.None;
        //UI갱신
        UIManager.main_ui.SetWeaponInfo(myWeapon);

        playerUI = GetComponentInChildren<PlayerUI>();
        //생성한 플레이어의 닉네임과 컬러를 입력한다.(나: 녹색, 상대방:적색)
        Color nameColor = photonView.IsMine ? new Color(0, 1, 0) : new Color(1, 0, 0);
        playerUI.SetNickName(photonView.Owner.NickName, nameColor);

        //생성한 플레이어의 체력 바의 색상을 입력한다..(나: 녹색, 상대방:적색)
        Color healthColor = photonView.IsMine ? new Color(0, 1, 0) : new Color(1, 0.2f, 0);
        playerUI.SetHpColor(healthColor);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && myWeapon.weaponType != WeaponType.None)
        {
            Fire();
        }

        if(photonView.IsMine&&Input.GetKeyDown(KeyCode.F) && myWeapon.weaponType != WeaponType.None)
        {
             RPC_DropWeapon();
        }

    }

    void Fire()
    {
        // 마우스 좌 클릭을 하면 카메라의 중앙을 기준으로 전방으로 레이캐스트를 한다.
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;

        bool result = Physics.Raycast(ray, out hitInfo, myWeapon.range, ~(1 << 6));
        if (result)
        {
            // 만일, 닿은 오브젝트의 태그가 "Enemy"라면...
            if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // 데미지 처리를 한다.
                hitInfo.transform.GetComponent<IInteractionInterface>().RPC_TakeDamage
                    (myWeapon.attakPower, photonView.ViewID);
            }
            // 그렇지 않다면, 닿은 위치에 파편 이펙트를 출력한다.
            else
            {
                
            }


        }
        // 총알의 갯수를 1 감소시킨다.(단, 0 이하가 되지 않도록 한다.)
        myWeapon.ammo = Mathf.Max(--myWeapon.ammo, 0);
        UIManager.main_ui.SetWeaponInfo(myWeapon);


    }
    void RPC_DropWeapon()
    {
        //나의 무기에 있는 WeaponData 컴포넌트의 DropWeapon 함수를 실행한다.
        WeaponData data = sockets[(int)myWeapon.weaponType].GetChild(0).GetComponent<WeaponData>();
        if(data != null)
        {
            data.DropWeapon(myWeapon);
        }
        photonView.RPC("DRropWeapon", RpcTarget.All);
    }
    [PunRPC]
    void DropMyWeapon()
    {
       // 무기 상태(myWeapon 변수)를 초기화한다.
       myWeapon = new WeaponInfo();
       UIManager.main_ui.SetWeaponInfo(myWeapon);
       
       anim.SetBool("GetPistol", false);
       anim.SetBool("GetRifle", false);

    }
    public void RPC_GetWeapon(int ammo, float atkPower, float range, int weaponType)
    {
        photonView.RPC("GetWeapon", RpcTarget.All, ammo, atkPower, range, weaponType);
    }

    [PunRPC]
    public void GetWeapon(int ammo, float atkPower, float range, int weaponType)
    {
        // 플레이어의 지정된 소켓 위치에 총을 부착시킨다.
        // - 자신을 소켓의 자식 오브젝트로 등록하고
        // - 로컬 포지션을 (0, 0, 0)으로 맞춘다.
        // - 자신의 박스 컴포넌트를 비활성화한다.
        // - 무기 정보를 플레이어에게 전달한다.

        myWeapon.SetInformation(ammo,atkPower,range,(WeaponType)weaponType);

        UIManager.main_ui.SetWeaponInfo(myWeapon);
        if (myWeapon.weaponType == WeaponType.PistolType)
        {
            anim.SetBool("GetPistol", true);
            anim.SetBool("GetRifle", false);
        }
        else if(myWeapon.weaponType == WeaponType.RifleType)
        {
            //순서 중요
            anim.SetBool("GetRifle", true);
            anim.SetBool("GetPistol", false);
        }
        
    }
}
