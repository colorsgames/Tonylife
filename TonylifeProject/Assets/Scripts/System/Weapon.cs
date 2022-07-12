using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject weaponPrefab;

    public RuntimeAnimatorController Controller { get { return controller; } }

    [SerializeField] private RuntimeAnimatorController controller;

    private Animator animator;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }
}
