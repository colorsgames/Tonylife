using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AliveCreature : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    public bool alive = true;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float maxAcceleration;
    [SerializeField] private float jumpHeight;

    [Header("Animations")]
    [SerializeField] private Animator legAnimator;
    [SerializeField] private Animator armAnimator;
    [SerializeField] private float changeAnimSpeed;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float radiusCheckCircle;
    [SerializeField] private LayerMask checkMask;

    protected bool running;
    protected bool desiredJump;

    protected Rigidbody2D rb;
    protected Rigidbody2D[] limbs;

    private Rigidbody2D connectedBody;
    private Rigidbody2D previousConnectedBody;

    private Vector2 velocity;
    private Vector2 connectionWorldPosition;
    private Vector2 connectionVelocity;

    private float curretSpeed;
    private float curretSize;
    private float curretHealth;

    protected virtual void Start()
    {
        curretHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
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
            alive = false;
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

    protected virtual void Dead()
    {
        legAnimator.enabled = false;
        armAnimator.enabled = false;

        foreach (Rigidbody2D item in limbs)
        {
            if (item.isKinematic)
            {
                item.isKinematic = false;
            }
        }
    }

    protected bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, radiusCheckCircle, checkMask);
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
        if (legAnimator != null && armAnimator != null)
        {
            float curretBlendAnim = legAnimator.GetFloat("Blend");
            float maxChangeSpeed = changeAnimSpeed * Time.deltaTime;

            if (hor != 0 && isGrounded())
            {
                legAnimator.SetFloat("Blend", running ? Mathf.Lerp(curretBlendAnim, 1f, maxChangeSpeed) : Mathf.Lerp(curretBlendAnim, 0.5f, maxChangeSpeed));
                armAnimator.SetFloat("Blend", running ? Mathf.Lerp(curretBlendAnim, 1f, maxChangeSpeed) : Mathf.Lerp(curretBlendAnim, 0.5f, maxChangeSpeed));
            }
            else if (!isGrounded())
            {
                legAnimator.SetFloat("Blend", Mathf.Lerp(curretBlendAnim, 1.5f, maxChangeSpeed));
                armAnimator.SetFloat("Blend", Mathf.Lerp(curretBlendAnim, 1.5f, maxChangeSpeed));
            }
            else
            {
                if (curretBlendAnim >= 0.01f)
                {
                    legAnimator.SetFloat("Blend", Mathf.Lerp(curretBlendAnim, 0f, maxChangeSpeed));
                    armAnimator.SetFloat("Blend", Mathf.Lerp(curretBlendAnim, 0f, maxChangeSpeed));
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, radiusCheckCircle);
    }
}
