using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : MonoBehaviour
{
    public WeaponInfo weaponInfo;
    Coroutine weaponCoroutine;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 대상의 Layer가 Player라면...
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerFire pf = other.transform.GetComponent<PlayerFire>();
            // 플레이어의 지정된 소켓 위치에 총을 부착시킨다.
            transform.parent =pf.sockets[(int)weaponInfo.weaponType].transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.GetComponent<BoxCollider>().enabled = false;
            pf.myWeapon = weaponInfo;
            pf.GetWeapon();
            // - 자신을 소켓의 자식 오브젝트로 등록하고
            // - 로컬 포지션을 (0, 0, 0)으로 맞춘다.
            // - 자신의 박스 컴포넌트를 비활성화한다.
            // - 무기 정보를 플레이어에게 전달한다.
        }


    }

    public void DropWeapon(WeaponInfo currentInfo)
    {
        // 플레이어의 소켓에서 총을 떼어낸다.
        // - 부모로 등록된 오브젝트를 없는 것으로 처리한다.
        // - 자신의 박스 컴포넌트를 활성화한다.
        // - 플레이어의 무기 정보를 받아온다.
        transform.parent = null;
        if (weaponCoroutine == null)
        {
            weaponCoroutine = StartCoroutine(TriggerOn(3));
        }
        
        
        transform.eulerAngles = Vector3.zero;
        weaponInfo = currentInfo;
    }
    IEnumerator TriggerOn(float time)
    {
        yield return new WaitForSeconds(time);
        transform.GetComponent<BoxCollider>().enabled = true;
        weaponCoroutine = null;
    }
}
