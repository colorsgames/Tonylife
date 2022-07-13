using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponType
    {
        CloseWeapon,
        Guns
    }

    public RuntimeAnimatorController Controller { get { return controller; } }
    public GameObject WeaponPrefab { get { return weaponPrefab; } }

    [Header("General Settings")]
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private RuntimeAnimatorController controller;
    [SerializeField] private WeaponType type = WeaponType.Guns;
    [SerializeField] private float delay;
    [SerializeField] private float damage;
    

    private Animator animator;

    private float curretTime;

    private bool canAttack;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        curretTime += Time.deltaTime;
        if (curretTime > delay)
        {
            canAttack = true;
        }
    }

    public void StartAttackAnimations()
    {
        if (type == WeaponType.CloseWeapon) return;
        animator.SetTrigger("StartAttack");
    }

    public void Attack()
    {
        if (canAttack)
        {
            animator.SetTrigger("Attack");

            curretTime = 0;
            canAttack = false;
        }
    }

    public void StopAttack()
    {
        animator.SetTrigger("StopAttack");
    }
}
