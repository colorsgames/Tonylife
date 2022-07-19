using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPC : AliveCreature
{
    [Header("NPC Settings")]
    [SerializeField] private float stopRadius;
    [SerializeField] private float closeAttackStopRadius;
    [SerializeField] private float distantAttackStopRadius;

    [Header("Obstacle Detection")]
    [SerializeField] Transform obstaclesDetectionPoint;
    [SerializeField] private float obstaclesRayDistance;
    [SerializeField] private float jumpDelay;
    [SerializeField] private LayerMask obstacles;

    [Header("Straying Settings")]
    [SerializeField] private float strayingRandomRadius;
    [SerializeField] private float changeRandomPosTime;

    [Header("Attack Settings")]
    [SerializeField] private float closeAttackDist;
    [SerializeField] private float distantAttackDist;
    [SerializeField] private LayerMask attackMask;

    private Transform target;

    private AliveCreatureDetector aliveCreatureDetector;

    private Vector2 startStrayingPos;
    private Vector2 randomTarget;

    private float curretJumpDelayTime;
    private float curretChangePosTime;
    private float curretStopRadius;

    private bool came;
    private bool aggressive;

    protected override void Start()
    {
        curretJumpDelayTime = jumpDelay;
        startStrayingPos = transform.position;
        aliveCreatureDetector = GetComponentInChildren<AliveCreatureDetector>();

        aliveCreatureDetector.discovered += ACDiscovered;

        base.Start();
    }

    protected override void Update()
    {
        if (!Alive) return;

        if (!target)
        {
            Straying();
        }
        else
        {
            if (aggressive)
            {
                if (curretWeapon)
                {
                    if (curretWeapon.WType == Weapon.WeaponType.CloseWeapon)
                    {
                        curretStopRadius = closeAttackStopRadius;
                    }
                    else
                    {
                        curretStopRadius = distantAttackStopRadius;
                    }
                }
            }
            else
            {
                curretStopRadius = stopRadius;
            }
            GoTo(target.position, curretStopRadius);
        }

        Attack();
        TakeItem();

        base.Update();
    }

    protected virtual void ACDiscovered(AliveCreature creature) { }

    protected void SetAggressive(bool aggressiveValue, Transform target)
    {
        aggressive = aggressiveValue;
        this.target = target;
    }

    void Attack()
    {
        if (!aggressive) return;
        if (curretWeapon)
        {
            if(curretWeapon.WType == Weapon.WeaponType.CloseWeapon)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, MyDirection(), closeAttackDist, attackMask);
                if (hit)
                {
                    curretWeapon.StartAttack();
                }
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, MyDirection(), distantAttackDist, attackMask);
                if (hit)
                {
                    curretWeapon.StartAttack();
                }

                if(curretWeapon.GetAmmo() <= 0)
                {
                    if(!curretWeapon.GetRecharge())
                        curretWeapon.StartRecharge();
                }
            }
        }
    }

    void TakeItem()
    {
        if (curretWeapon) return;
        closeIntObj = GetCloseInterective();
        if (closeIntObj)
        {
            target = closeIntObj.transform;
            running = true;
            if (came)
            {
                running = false;
                Use();
            }
        }
    }

    void Straying()
    {
        if (came)
        {
            curretChangePosTime += Time.deltaTime;
            if (curretChangePosTime > changeRandomPosTime)
            {
                float radius = Random.Range(-strayingRandomRadius, strayingRandomRadius);
                randomTarget = new Vector2(startStrayingPos.x + radius, 0);
                curretChangePosTime = 0;
            }
        }
        GoTo(randomTarget, stopRadius);
    }

    void GoTo(Vector3 target, float stopRadius)
    {
        if (!Alive) return;

        came = false;

        Vector3 offset = target - transform.position;

        if (offset.x < -0.1) { Rotate(-1f); }
        else if (offset.x > 0.1) { Rotate(1f); }

        if (offset.x < -stopRadius) { Movement(-1f); }
        else if (offset.x > stopRadius) { Movement(1f); }
        else
        {
            Movement(0f);
            came = true;
            return;
        }

        ObstacleAvoidance();
    }

    void ObstacleAvoidance()
    {
        curretJumpDelayTime += Time.deltaTime;
        if (curretJumpDelayTime > jumpDelay)
        {
            RaycastHit2D hit = Physics2D.Raycast(obstaclesDetectionPoint.position, MyDirection(), obstaclesRayDistance, obstacles);
            if (hit)
            {
                if (hit.transform.localScale.magnitude < 2.4f)
                {
                    desiredJump = true;
                    curretJumpDelayTime = 0f;
                }
                else
                {
                    came = true;
                }
            }
        }
    }

#if UNITY_EDITOR

    protected override void OnDrawGizmos()
    {
        Gizmos.DrawRay(obstaclesDetectionPoint.position, MyDirection() * obstaclesRayDistance);

        base.OnDrawGizmos();
    }

#endif
}
