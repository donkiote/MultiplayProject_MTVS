using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerUI : MonoBehaviour
{
    [SerializeField]TMP_Text nickName;
    [SerializeField] Slider hpBar;
    [SerializeField] Image fillImage;

   public void SetNickName()
    {
        hpBar.value = 1.0f;
    }
    private void Update()
    {
        //항상 메인 카메라에 보이도록 처리 한다.
        transform.forward = Camera.main.transform.forward;
    }

    //닉네임 값과 컬러를 지정하는 함수
    public void SetNickName(string name, Color hpColor)
    {
        nickName.text = name;
        nickName.color = hpColor;
    }
    //슬라이더의 컬러 지정하는 함수
    public void SetHpColor(Color hpColor)
    {
        fillImage.color = hpColor;
    }
    public void SetHPValue(float curHp, float maxHp)
    {
        hpBar.value = curHp / maxHp;
    }
}
