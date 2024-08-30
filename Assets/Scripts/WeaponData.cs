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
        // �浹�� ����� Layer�� Player���...
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerFire pf = other.transform.GetComponent<PlayerFire>();
            // �÷��̾��� ������ ���� ��ġ�� ���� ������Ų��.
            transform.parent =pf.sockets[(int)weaponInfo.weaponType].transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.GetComponent<BoxCollider>().enabled = false;
            pf.myWeapon = weaponInfo;
            pf.GetWeapon();
            // - �ڽ��� ������ �ڽ� ������Ʈ�� ����ϰ�
            // - ���� �������� (0, 0, 0)���� �����.
            // - �ڽ��� �ڽ� ������Ʈ�� ��Ȱ��ȭ�Ѵ�.
            // - ���� ������ �÷��̾�� �����Ѵ�.
        }


    }

    public void DropWeapon(WeaponInfo currentInfo)
    {
        // �÷��̾��� ���Ͽ��� ���� �����.
        // - �θ�� ��ϵ� ������Ʈ�� ���� ������ ó���Ѵ�.
        // - �ڽ��� �ڽ� ������Ʈ�� Ȱ��ȭ�Ѵ�.
        // - �÷��̾��� ���� ������ �޾ƿ´�.
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
