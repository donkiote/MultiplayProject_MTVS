using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UIManager : MonoBehaviour
{
    public static UIManager main_ui;
    public TMP_Text[] weaponInfo;

    private void Awake()
    {
        if (main_ui == null)
        {
            main_ui = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        
    }

   
    void Update()
    {
        
    }
    public void SetWeaponInfo(WeaponInfo info)
    {
        weaponInfo[0].text = $"Ammo: <color=#dceb15>{info.ammo}</color>";
        weaponInfo[1].text = $"Damage:<color=#ff0000>{info.attakPower}</color>";
        weaponInfo[2].text = $"Weapon Type:<i>{info.weaponType}</i>";
    }
}
