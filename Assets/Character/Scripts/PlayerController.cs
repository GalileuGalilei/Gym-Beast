using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : CharacterController
{
    private void Update()
    {
        Move(walkDirection);
    }

    public void OnMove(CallbackContext value)
    {
        if (value.phase == InputActionPhase.Canceled)
        {
            walkDirection = Vector2.zero;
            SetState(CharacterState.Idle);
            return;
        }

        walkDirection = value.action.ReadValue<Vector2>();
        SetState(CharacterState.Walk);
    }
}
