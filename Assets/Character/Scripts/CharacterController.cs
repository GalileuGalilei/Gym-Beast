using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;

public partial class CharacterController : MonoBehaviour
{
    protected Vector2 walkDirection;
    protected Animator animator;
    protected Rigidbody rb;
    protected CharacterState state;
    [SerializeField] protected float walkSpeed = 5f;
    [SerializeField] protected float rotationSpeed = 5f;

    protected void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        EnableRagdoll(false);
    }

    protected void EnableRagdoll(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = state;
        }

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = !state;
        }
    }

    protected void Move(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            return;
        }

        walkDirection = direction;
        
        float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);
        rb.rotation = Quaternion.Euler(0, angle, 0);
        rb.position += transform.forward * walkSpeed * Time.deltaTime;
    }

    protected void SetState(CharacterState state)
    {
        animator.SetInteger("State", (int)state);
        this.state = state;

        switch (state)
        {
            case CharacterState.Attack:
                animator.SetLayerWeight(1, 1);
                break;  
            default:
                animator.SetLayerWeight(1, 0);
                break;
        }
    }
}
