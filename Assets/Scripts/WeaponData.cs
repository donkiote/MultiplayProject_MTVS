using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class WeaponData : MonoBehaviourPun, IPunObservable
{
    public WeaponInfo weaponInfo;

    Coroutine weaponCoroutine;
    Vector3 syncPos;
    Quaternion syncRot;

    private void Update()
    {
        if (!photonView.IsMine)
        {
            transform.position = syncPos;
            transform.rotation = syncRot;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 대상의 Layer가 Player이면서, 그 플레이어의 PhotonView가 isMine라면...
        PhotonView pv = other.GetComponent<PhotonView>();
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && pv != null && pv.IsMine)
        {
            PlayerFire pf = other.transform.GetComponent<PlayerFire>();
            transform.parent = pf.sockets[(int)weaponInfo.weaponType].transform;
            // 충돌체 비활성화하기
            photonView.RPC("SwitchEnabledCollider", RpcTarget.All, false);

            // 자신의 네트워크 소유권을 충돌한 플레이어에게 이전한다.
            photonView.TransferOwnership(pv.Owner);
            StartCoroutine(ResetPosition());

            // RPC 함수를 호출하도록 요청한다.
            pf.RPC_GetWeapon(weaponInfo.ammo, weaponInfo.attakPower, weaponInfo.range, (int)weaponInfo.weaponType);

        }
    }

    [PunRPC]
    void SwitchEnabledCollider(bool onOff)
    {
        transform.GetComponent<BoxCollider>().enabled = onOff;
    }
    IEnumerator ResetPosition()
    {
        //소유권이 있어서 isMine이 될때까지
        yield return new WaitUntil(() => { return photonView.IsMine; });

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
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
        photonView.RPC("SwitchEnabledCollider", RpcTarget.All, true);
        weaponCoroutine = null;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);

        }
        else if(stream.IsReading)
        {
            syncPos = (Vector3)stream.ReceiveNext();
            syncRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
