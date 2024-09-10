using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractionInterface
{
    /// <summary>
    /// 데미지 기능 구현 강제
    /// </summary>
    /// <param name="dmg">실제 공격력</param>
    /// <param name="viewID">공격자의 photon view Id</param>
    void TakeDamage(float dmg, int viewID);
}
