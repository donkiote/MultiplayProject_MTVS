using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    None = -1,
    PistolType = 0,
    RifleType = 1
}


[System.Serializable]
public class WeaponInfo
{
    public int ammo;
    public float attakPower;
    public float range;
    public WeaponType weaponType;

    public void SetInformation(int ammo, float atkPower, float range, WeaponType type)
    {
        this.ammo = ammo;
        attakPower = atkPower;
        this.range = range;
        weaponType = type;

    }
}
