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
        //�׻� ���� ī�޶� ���̵��� ó�� �Ѵ�.
        transform.forward = Camera.main.transform.forward;
    }

    //�г��� ���� �÷��� �����ϴ� �Լ�
    public void SetNickName(string name, Color hpColor)
    {
        nickName.text = name;
        nickName.color = hpColor;
    }
    //�����̴��� �÷� �����ϴ� �Լ�
    public void SetHpColor(Color hpColor)
    {
        fillImage.color = hpColor;
    }
    public void SetHPValue(float curHp, float maxHp)
    {
        hpBar.value = curHp / maxHp;
    }
}
