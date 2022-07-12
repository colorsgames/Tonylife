using System.Collections;
using System.Collections.Generic;
using UnityEngine.U2D.IK;
using UnityEngine;

public abstract class AliveCreature : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [Header("Inventory Settings")]
    [SerializeField] protected float stronglyDropItemForce = 20f;
    [SerializeField] protected float weaklyDropItemForce = 0f;
    [SerializeField] protected float rotationForce = 5f;
    [SerializeField] protected Transform dropTarget;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float maxAcceleration;
    [SerializeField] private float jumpHeight;

    [Header("Animations")]
    [SerializeField] private Animator legAnimator;
    [SerializeField] private Animator leftArmAnimator;
    [SerializeField] private Animator rightArmAnimator;
    [SerializeField] private float changeAnimSpeed;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float radiusCheckCircle;
    [SerializeField] private LayerMask checkMask;

    public bool Alive { get { return this.alive; } }

    protected bool running;
    protected bool desiredJump;

    protected Inventory inventory;
    protected Weapon curretWeapon;

    protected Rigidbody2D rb;
    protected Rigidbody2D[] limbs;

    private Rigidbody2D connectedBody;
    private Rigidbody2D previousConnectedBody;

    private IKManager2D iKManager2D;

    private Vector2 velocity;
    private Vector2 connectionWorldPosition;
    private Vector2 connectionVelocity;

    private List<InteractiveObject> interactiveObjects = new List<InteractiveObject>();

    private float curretSpeed;
    private float curretSize;
    private float curretHealth;

    private bool alive = true;

    protected virtual void Start()
    {
        curretHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        iKManager2D = GetComponent<IKManager2D>();
        inventory = GetComponent<Inventory>();

        if (GetComponentInChildren<Rigidbody2D>())
        {
            limbs = GetComponentsInChildren<Rigidbody2D>();
        }

        curretSpeed = walkSpeed;
        curretSize = transform.localScale.x;
    }

    protected virtual void Update()
    {
        curretSpeed = running ? runSpeed : walkSpeed;

    }

    protected void Movement(float hor)
    {
        if (!alive)
        {
            hor = 0;
        }
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
        leftArmAnimator.enabled = false;
        rightArmAnimator.enabled = false;

        iKManager2D.weight = 0;

        foreach (Rigidbody2D item in limbs)
        {
            if (item.isKinematic)
            {
                item.isKinematic = false;
                item.velocity = velocity;
            }
        }

        alive = false;
    }

    protected InteractiveObject GetCloseInterective()
    {
        if (interactiveObjects.Count > 0)
        {
            Vector3[] interactObjPos = new Vector3[interactiveObjects.Count];
            List<float> distance = new List<float>();
            for (int i = 0; i < interactiveObjects.Count; i++)
            {
                interactObjPos[i] = interactiveObjects[i].transform.position;
                Vector3 offset = interactObjPos[i] - transform.position;
                distance.Add(offset.magnitude);
            }

            float minDist = Mathf.Min(distance.ToArray());
            int id = distance.IndexOf(minDist);

            return interactiveObjects[id];
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
        if (legAnimator && leftArmAnimator && rightArmAnimator)
        {
            float curretBlendAnim = legAnimator.GetFloat("Blend");
            float maxChangeSpeed = changeAnimSpeed * Time.deltaTime;

            if (hor != 0 && isGrounded())
            {
                float blend = running ? Mathf.Lerp(curretBlendAnim, 1f, maxChangeSpeed) : Mathf.Lerp(curretBlendAnim, 0.5f, maxChangeSpeed);
                legAnimator.SetFloat("Blend", blend);
                leftArmAnimator.SetFloat("Blend", blend);
                rightArmAnimator.SetFloat("Blend", blend);
            }
            else if (!isGrounded())
            {
                float blend = Mathf.Lerp(curretBlendAnim, 1.5f, maxChangeSpeed);
                legAnimator.SetFloat("Blend", blend);
                leftArmAnimator.SetFloat("Blend", blend);
                rightArmAnimator.SetFloat("Blend", blend);
            }
            else
            {
                if (curretBlendAnim >= 0.01f)
                {
                    float blend = Mathf.Lerp(curretBlendAnim, 0f, maxChangeSpeed);
                    legAnimator.SetFloat("Blend", blend);
                    leftArmAnimator.SetFloat("Blend", blend);
                    rightArmAnimator.SetFloat("Blend", blend);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<InteractiveObject>())
        {
            interactiveObjects.Add(collision.gameObject.GetComponent<InteractiveObject>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<InteractiveObject>())
        {
            interactiveObjects.Remove(collision.gameObject.GetComponent<InteractiveObject>());
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

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, radiusCheckCircle);
    }
}
