using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractionInterface
{
    /// <summary>
    /// ������ ��� ���� ����
    /// </summary>
    /// <param name="dmg">���� ���ݷ�</param>
    /// <param name="viewID">�������� photon view Id</param>
    void TakeDamage(float dmg, int viewID);
}
