using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="WeaponData", fileName ="New WeaponData", order = 53)]
public class WeaponData : ScriptableObject
{
    [Header("General Settings")]
    public GameObject weaponPrefab;
    public GameObject bloodParticlePrefab;
    public GameObject shotParticlePrefab;
    public GameObject shotTrail;
    public RuntimeAnimatorController controller;
    public float delay;
    public float force;
    public float raycastLenght = 100;

    [Header("Guns Settings")]
    public int maxAmmo;
    public float rechargeTime;
}
