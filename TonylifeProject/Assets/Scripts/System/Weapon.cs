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

    public RuntimeAnimatorController Controller { get { return data.controller; } }
    public GameObject WeaponPrefab { get { return data.weaponPrefab; } }

    public bool CanAttack { get { return canAttack; } }
    public bool Attacks { get; set; }

    [SerializeField] private WeaponData data;
    [SerializeField] private WeaponType type = WeaponType.Guns;
    [SerializeField] private Transform raycastTarget;
    [SerializeField] private float damage;
    [SerializeField] private LayerMask attackMask;

    [HideInInspector] public int curretAmmo;

    private AliveCreature creature;

    private Animator animator;

    private RaycastHit2D oldHit;

    private float curretTime;

    private bool canAttack;


    private void Start()
    {
        creature = GetComponentInParent<AliveCreature>();
        animator = GetComponentInParent<Animator>();
        curretTime = data.delay;
        curretAmmo = data.maxAmmo;
    }

    private void Update()
    {
        curretTime += Time.deltaTime;
        if (curretTime > data.delay)
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
                RaycastHit2D hit = Physics2D.Raycast(raycastTarget.position, raycastTarget.up, data.raycastLenght, attackMask);
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
        if(curretAmmo > 0)
        {
            AttackAnim();

            RaycastHit2D hit = Physics2D.Raycast(raycastTarget.position, creature.MyDirection(), data.raycastLenght, attackMask);
            RaycastAttack(hit);
            CreateShotTrail(hit);

            curretAmmo--;
        }
        else
        {
            if (creature.GetComponent<Player>())
            {
                creature.Drop();
            }
        }
    }

    void CreateShotTrail(RaycastHit2D hit)
    {
        ShotTrail shotTrail = Instantiate(data.shotTrail, raycastTarget.position, Quaternion.identity).GetComponent<ShotTrail>();
        if (hit)
            shotTrail.SetTarget(hit.point);
        else
            shotTrail.SetTarget(creature.MyDirection() * 100);
    }

    void RaycastAttack(RaycastHit2D hit)
    {
        if (hit)
        {
            Rigidbody2D rb = hit.collider.GetComponent<Rigidbody2D>();

            if (rb)
            {
                rb.AddForce(creature.MyDirection() * data.force, ForceMode2D.Impulse);
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
        Gizmos.DrawRay(raycastTarget.position, raycastTarget.up * data.raycastLenght);
    }

#endif
}
