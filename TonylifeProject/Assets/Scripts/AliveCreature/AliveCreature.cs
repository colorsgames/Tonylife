using System.Collections;
using System.Collections.Generic;
using UnityEngine.U2D.IK;
using UnityEngine;

public abstract class AliveCreature : MonoBehaviour
{
    public Transform DropTarget { get { return dropTarget; } }
    public Weapon CurretWeapon { get { return curretWeapon; } }

    public bool Alive { get { return this.alive; } }

    [SerializeField] private float maxHealth = 100;
    [Header("Drop Item Settings")]
    [SerializeField] protected float weaklyDropItemForce = 0f;
    [SerializeField] protected Transform dropTarget;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float maxAcceleration = 30f;
    [SerializeField] private float jumpHeight = 1.5f;

    [Header("Animations")]
    [SerializeField] private Animator legAnimator;
    [SerializeField] protected Animator armAmimator;
    [SerializeField] private float changeAnimSpeed = 10;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float radiusCheckCircle = 0.35f;
    [SerializeField] private LayerMask checkMask;

    protected bool running;
    protected bool desiredJump;

    protected Inventory inventory;
    protected Weapon curretWeapon;
    protected InteractiveObject closeIntObj;

    protected Rigidbody2D rb;
    protected Rigidbody2D[] limbs;

    private Rigidbody2D connectedBody;
    private Rigidbody2D previousConnectedBody;

    private IKManager2D iKManager2D;

    private Vector2 velocity;
    private Vector2 connectionWorldPosition;
    private Vector2 connectionVelocity;

    private InteractiveObjectDetector interactiveDetector;

    private float curretSpeed;
    private float curretSize;
    private float curretHealth;

    private bool alive = true;
    private bool canAttack = true;

    protected virtual void Start()
    {
        curretHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        iKManager2D = GetComponent<IKManager2D>();
        inventory = GetComponent<Inventory>();

        interactiveDetector = GetComponentInChildren<InteractiveObjectDetector>();

        if (GetComponentInChildren<Rigidbody2D>())
        {
            limbs = GetComponentsInChildren<Rigidbody2D>();
        }

        curretSpeed = walkSpeed;
        curretSize = transform.localScale.x;

        curretWeapon = inventory.GetCurretWeapon();
    }

    protected virtual void Update()
    {
        curretSpeed = running ? runSpeed : walkSpeed;
        if (curretWeapon)
        {
            canAttack = curretWeapon.CanAttack;
        }
        else
        {
            canAttack = true;
        }
    }

    protected void Movement(float hor)
    {
        if (!alive) hor = 0;

        velocity = rb.velocity;

        if (!isGrounded())
        {
            connectionVelocity = Vector2.zero;
            previousConnectedBody = connectedBody;
            connectedBody = null;
        }

        if (connectedBody)
        {
            if (connectedBody.mass > rb.mass)
            {
                UpdateConnectionState();
            }
        }

        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        Vector2 relativeVelocity = velocity - connectionVelocity;

        float curretX = Vector3.Dot(relativeVelocity, Vector2.right);
        float newX = Mathf.MoveTowards(curretX, DesiredVelocity(hor).x, maxSpeedChange);

        velocity += Vector2.right * (newX - curretX);

        Rotate(hor);

        MoveAnimation(hor);

        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }

        rb.velocity = velocity;
    }

    protected virtual void Dead()
    {
        legAnimator.enabled = false;
        armAmimator.enabled = false;

        iKManager2D.weight = 0;

        foreach (Rigidbody2D item in limbs)
        {
            if (item.isKinematic)
            {
                item.isKinematic = false;
                item.velocity = velocity;
            }
        }

        if (curretWeapon)
        {
            Drop();
        }

        alive = false;
    }

    protected InteractiveObject GetCloseInterective()
    {
        if (interactiveDetector.InteractiveObjects.Count > 0)
        {
            Vector3[] interactObjPos = new Vector3[interactiveDetector.InteractiveObjects.Count];
            List<float> distance = new List<float>();
            for (int i = 0; i < interactiveDetector.InteractiveObjects.Count; i++)
            {
                interactObjPos[i] = interactiveDetector.InteractiveObjects[i].transform.position;
                Vector3 offset = interactObjPos[i] - transform.position;
                distance.Add(offset.magnitude);
            }

            float minDist = Mathf.Min(distance.ToArray());
            int id = distance.IndexOf(minDist);

            return interactiveDetector.InteractiveObjects[id];
        }
        else return null;
    }

    protected bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, radiusCheckCircle, checkMask);
    }

    public void Rotate(float rot)
    {
        if (rot > 0)
        {
            transform.localScale = new Vector3(curretSize, curretSize, curretSize);
        }
        else if (rot < 0)
        {
            transform.localScale = new Vector3(-curretSize, curretSize, curretSize);
        }
    }

    public void MakeDamage(float damage)
    {
        curretHealth -= damage;
        if (curretHealth <= 0)
        {
            if (alive)
            {
                Dead();
            }
        }
    }

    public Vector2 MyDirection()
    {
        if (transform.localScale.x > 0)
        {
            return Vector2.right;
        }
        else if (transform.localScale.x < 0)
        {
            return Vector2.left;
        }
        else
        {
            return Vector2.zero;
        }
    }

    public void Use()
    {
        if (!closeIntObj) return;
        if (!canAttack) return;
        closeIntObj.Use();
        if (closeIntObj.GetComponent<Item>())
        {
            if (inventory.Busy)
            {
                Drop();
            }
            inventory.TakeItem(closeIntObj.GetComponent<Item>());
            curretWeapon = inventory.GetCurretWeapon();
        }
    }

    public void QuitItem()
    {
        if (!canAttack) return;
        if (inventory.Busy)
        {
            armAmimator.SetTrigger("Drop");
            curretWeapon = null;
        }
    }

    public void Drop()
    {
        inventory.DropItem(dropTarget.position, MyDirection(), dropTarget.rotation, weaklyDropItemForce, 0);
        curretWeapon = null;
    }

    private Vector2 DesiredVelocity(float hor)
    {
        return transform.right * curretSpeed * hor;
    }

    private void UpdateConnectionState()
    {
        Vector2 connectionMovement = connectedBody.position - connectionWorldPosition;
        connectionVelocity = connectionMovement / Time.deltaTime;

        connectionWorldPosition = connectedBody.position;
    }

    private void Jump()
    {
        if (isGrounded())
        {
            float jumpSpeed = Mathf.Sqrt(-2 * Physics.gravity.y * jumpHeight);
            velocity.y += jumpSpeed;
        }
    }

    private void MoveAnimation(float hor)
    {
        if (legAnimator && armAmimator)
        {
            float curretBlendAnim = legAnimator.GetFloat("Blend");
            float maxChangeSpeed = changeAnimSpeed * Time.deltaTime;

            if (hor != 0 && isGrounded())
            {
                float blend = running ? Mathf.Lerp(curretBlendAnim, 1f, maxChangeSpeed) : Mathf.Lerp(curretBlendAnim, 0.5f, maxChangeSpeed);
                legAnimator.SetFloat("Blend", blend);
                armAmimator.SetFloat("Blend", blend);

            }
            else if (!isGrounded())
            {
                float blend = Mathf.Lerp(curretBlendAnim, 1.5f, maxChangeSpeed);
                legAnimator.SetFloat("Blend", blend);
                armAmimator.SetFloat("Blend", blend);

            }
            else
            {
                if (curretBlendAnim >= 0.01f)
                {
                    float blend = Mathf.Lerp(curretBlendAnim, 0f, maxChangeSpeed);
                    legAnimator.SetFloat("Blend", blend);
                    armAmimator.SetFloat("Blend", blend);

                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isGrounded())
        {
            if (collision.rigidbody)
            {
                connectedBody = collision.rigidbody;
            }
        }
    }
#if UNITY_EDITOR

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, radiusCheckCircle);
    }

#endif
}
