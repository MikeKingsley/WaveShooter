using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Weapon Stats")]
public class WeaponStats : ScriptableObject
{
    [Header("Fire Modes")]
    public bool fireMode_single;
    public bool fireMode_semi;
    public bool fireMode_burst;
    public bool fireMode_auto;

    public float fireRate = 1;
}
