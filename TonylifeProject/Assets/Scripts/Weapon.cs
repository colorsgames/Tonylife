using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject weaponPrefab;

    [SerializeField] private Animator animator;
    [SerializeField] private Animation anim;

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }
}
