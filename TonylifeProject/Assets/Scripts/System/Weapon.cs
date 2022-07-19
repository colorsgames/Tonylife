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

    private int curretAmmo;

    private GameObject shotPart;

    private AliveCreature creature;

    private Animator animator;

    private RaycastHit2D oldHit;

    private float curretAttackTime;
    private float curretRechargeTime;

    private bool canAttack;
    private bool recharge;


    private void Start()
    {
        creature = GetComponentInParent<AliveCreature>();
        animator = GetComponentInParent<Animator>();
        curretAttackTime = data.delay;
        curretAmmo = data.maxAmmo;

        if(WType == WeaponType.Guns)
        {
            shotPart = Instantiate(data.shotParticlePrefab, raycastTarget.position, Quaternion.identity);
            shotPart.transform.parent = transform;
            shotPart.transform.localRotation = Quaternion.Euler(0, 90, 0);
            shotPart.transform.localScale = Vector3.one;
            shotPart.SetActive(false);
        }
    }

    private void Update()
    {
        curretAttackTime += Time.deltaTime;
        if (curretAttackTime > data.delay)
        {
            canAttack = true;
        }
        Recharge();
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

            curretAttackTime = 0;
            canAttack = false;
        }
    }

    public void StartRecharge()
    {
        recharge = true;
        animator.SetTrigger("Recharge");
    }

    public int GetAmmo() { return curretAmmo; }
    public bool GetRecharge() { return recharge; }
    public void SetAmmo(int value) { curretAmmo = value; }

    void Shot()
    {
        if(curretAmmo > 0)
        {
            AttackAnim();

            shotPart.SetActive(true);

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

    void Recharge()
    {
        if (recharge)
        {
            curretRechargeTime += Time.deltaTime;
            if (curretRechargeTime > data.rechargeTime)
            {
                curretAmmo = data.maxAmmo;
                curretRechargeTime = 0;
                recharge = false;
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
            if (hit.collider.GetComponent<Limbs>())
            {
                hit.collider.GetComponent<Limbs>().MakeDamage(damage);
                Instantiate(data.bloodParticlePrefab, hit.point, Quaternion.Euler(0,0, -90 * creature.MyDirection().x));
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
