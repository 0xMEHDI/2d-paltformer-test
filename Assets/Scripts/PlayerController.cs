using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerController : PhysicsHandler
{
    // Movement
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float sprintSpeed = 5f;
    float currentSpeed;

    [SerializeField] float jump = 5f;
    [SerializeField] int jumpLimit = 2;
    int jumpCount;

    Animator animator;
    Vector2 move;

    bool walking;
    bool sprinting;

    // Combat
    [SerializeField] float attackCooldown = 1f;
    float timeTillAttack;

    bool attacking;

    protected override void OnEnable()
    {
        base.OnEnable();

        animator = GetComponentInChildren<Animator>();
    }

    protected override void ComputeVelocity()
    {
        Move();
        Rotate();

        Attack();

        Animate();
    }

    void Move()
    {
        move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");

        Jump();
        SetSpeed();

        targetVelocity = move * currentSpeed;
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
            if (grounded || jumpCount < jumpLimit)
            {
                velocity.y = jump;
                jumpCount++;
            }

            else if (Input.GetButtonUp("Jump"))
                if (velocity.y > Mathf.Epsilon)
                    velocity.y *= 0.5f;

        if (velocity.y == 0)
            jumpCount = 0;
    }

    void SetSpeed()
    {
        if (grounded)
        {
            if (Input.GetKeyDown(KeyCode.CapsLock))
                walking = !walking;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                sprinting = true;
                walking = false;
            }

            else
                sprinting = false;

        }

        if (walking && !sprinting)
            currentSpeed = walkSpeed;

        else if (sprinting)
            currentSpeed = sprintSpeed;

        else
            currentSpeed = runSpeed;
    }

    void Rotate()
    {
        if (move.x > Mathf.Epsilon)
            transform.localScale = new Vector2((Mathf.Abs(transform.localScale.x)), transform.localScale.y);

        else if (move.x < -Mathf.Epsilon)
            transform.localScale = new Vector2(-(Mathf.Abs(transform.localScale.x)), transform.localScale.y);
    }

    //TODO Remove hardcoded keys
    void Attack()
    {
        if (timeTillAttack >= attackCooldown)
        {
            if (Input.GetMouseButtonDown(0))
                StartCoroutine(AttackCoroutine());
                
            StopCoroutine(AttackCoroutine());
        }

        timeTillAttack += Time.deltaTime;
    }

    IEnumerator AttackCoroutine()
    {
        //TODO Allow mid air attacks
        if (grounded)
        {
            attacking = true;

            yield return new WaitForSeconds(0.5f);

            timeTillAttack = 0;

            attacking = false;
        }
    }

    void Animate()
    {
        animator.SetFloat("x", Mathf.Abs(targetVelocity.x));
        animator.SetFloat("y", velocity.y);

        animator.SetInteger("jumpCount", jumpCount);

        animator.SetBool("grounded", grounded);
        animator.SetBool("walking", walking);
        animator.SetBool("sprinting", sprinting);

        animator.SetBool("attacking", attacking);
    }
}
