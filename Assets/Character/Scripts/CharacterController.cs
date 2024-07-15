using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;

public abstract class CharacterController : MonoBehaviour
{
    protected Vector2 walkDirection;
    protected Animator animator;
    protected Collider col;
    protected CharacterState state;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    public float WalkSpeed { get => walkSpeed; }
    public float RotationSpeed { get => rotationSpeed; }
    public Rigidbody Rb { get; private set; }

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        EnableRagdoll(false);
    }

    protected void EnableRagdoll(bool state)
    {
        animator.enabled = !state;
        return;

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            if (col == this.col)
            {
                continue;
            }

            col.enabled = state;
        }

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            if (rb == this.Rb)
            {
                continue;
            }
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
        Rb.rotation = Quaternion.Euler(0, angle, 0);
        Rb.position += transform.forward * walkSpeed * Time.deltaTime;
    }

    public void SetUpperBodyAnimationWeight(float weight)
    {
        animator.SetLayerWeight(1, weight);
    }

    public void SetState(CharacterState state)
    {
        animator.SetInteger("State", (int)state);
        this.state = state;
    }
}
