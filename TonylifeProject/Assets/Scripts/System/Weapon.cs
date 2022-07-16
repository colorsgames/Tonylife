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

    public WeaponType WType { get { return type; } }

    public RuntimeAnimatorController Controller { get { return controller; } }
    public GameObject WeaponPrefab { get { return weaponPrefab; } }

    public bool CanAttack { get { return canAttack; } }
    public bool Attacks { get; set; }

    [Header("General Settings")]
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private RuntimeAnimatorController controller;
    [SerializeField] private WeaponType type = WeaponType.Guns;
    [SerializeField] private float delay;
    [SerializeField] private float damage;
    [SerializeField] private float force;
    [SerializeField] private Transform raycastTarget;
    [SerializeField] private float raycastLenght = 100;
    [SerializeField] private LayerMask attackMask;

    [Header("Guns Settings")]
    public int maxAmmo;
    [SerializeField] private float rechargeTime;

    private AliveCreature creature;

    private Animator animator;

    private RaycastHit2D oldHit;

    private float curretTime;

    private bool canAttack;


    private void Start()
    {
        creature = GetComponentInParent<AliveCreature>();
        animator = GetComponentInParent<Animator>();
        curretTime = delay;
    }

    private void Update()
    {
        curretTime += Time.deltaTime;
        if (curretTime > delay)
        {
            canAttack = true;
        }

    }

    private void FixedUpdate()
    {
        if (type == WeaponType.CloseWeapon)
        {
            if (Attacks)
            {
                RaycastHit2D hit = Physics2D.Raycast(raycastTarget.position, raycastTarget.up, raycastLenght, attackMask);
                if (oldHit)
                {
                    if(oldHit.collider != hit.collider)
                    {
                        oldHit = hit;
                        RaycastAttack(hit);
                    }
                }
                else
                {
                    oldHit = hit;
                    RaycastAttack(hit);
                }
            }
        }
    }

    public void StartAttack()
    {
        if (canAttack)
        {
            if (type == WeaponType.Guns)
            {
                Shot();
            }
            else
            {
                AttackAnim();
            }

            curretTime = 0;
            canAttack = false;
        }
    }

    void Shot()
    {
        if(maxAmmo > 0)
        {
            AttackAnim();
            RaycastHit2D hit = Physics2D.Raycast(raycastTarget.position, creature.MyDirection(), raycastLenght, attackMask);
            RaycastAttack(hit);
            maxAmmo--;
        }
        else
        {
            if (creature.GetComponent<Player>())
            {
                creature.Drop();
            }
        }
    }

    void RaycastAttack(RaycastHit2D hit)
    {
        if (hit)
        {
            Rigidbody2D rb = hit.collider.GetComponent<Rigidbody2D>();

            if (rb)
            {
                rb.AddForce(creature.MyDirection() * force, ForceMode2D.Impulse);
            }
            if (hit.collider.GetComponent<AliveCreature>())
            {
                hit.collider.GetComponent<AliveCreature>().MakeDamage(damage);
            }
        }
    }

    void AttackAnim()
    {
        animator.SetTrigger("Attack");
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(raycastTarget.position, raycastTarget.up * raycastLenght);
    }

#endif
}
