using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public Transform[] sockets;
    public WeaponInfo myWeapon;

    void Start()
    {
        myWeapon.weaponType = WeaponType.None;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && myWeapon.weaponType != WeaponType.None)
        {
            Fire();
        }

        if(Input.GetKeyDown(KeyCode.F) && myWeapon.weaponType != WeaponType.None)
        {
             DropMyWeapon();

        }

    }

    void Fire()
    {
        // 마우스 좌 클릭을 하면 카메라의 중앙을 기준으로 전방으로 레이캐스트를 한다.
        // 만일, 닿은 오브젝트의 태그가 "Enemy"라면...
        // 데미지 처리를 한다.

        // 그렇지 않다면, 닿은 위치에 파편 이펙트를 출력한다.

        // 총알의 갯수를 1 감소시킨다.(단, 0 이하가 되지 않도록 한다.)
    }

    void DropMyWeapon()
    {
        // 나의 무기에 있는 WeaponData 컴포넌트의 DropWeapon 함수를 실행한다.

        // 무기 상태(myWeapon 변수)를 초기화한다.

    }
}
