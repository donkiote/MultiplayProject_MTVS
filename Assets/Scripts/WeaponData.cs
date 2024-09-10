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
        // �浹�� ����� Layer�� Player�̸鼭, �� �÷��̾��� PhotonView�� isMine���...
        PhotonView pv = other.GetComponent<PhotonView>();
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && pv != null && pv.IsMine)
        {
            PlayerFire pf = other.transform.GetComponent<PlayerFire>();
            transform.parent = pf.sockets[(int)weaponInfo.weaponType].transform;
            // �浹ü ��Ȱ��ȭ�ϱ�
            photonView.RPC("SwitchEnabledCollider", RpcTarget.All, false);

            // �ڽ��� ��Ʈ��ũ �������� �浹�� �÷��̾�� �����Ѵ�.
            photonView.TransferOwnership(pv.Owner);
            StartCoroutine(ResetPosition());

            // RPC �Լ��� ȣ���ϵ��� ��û�Ѵ�.
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
        //�������� �־ isMine�� �ɶ�����
        yield return new WaitUntil(() => { return photonView.IsMine; });

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
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
