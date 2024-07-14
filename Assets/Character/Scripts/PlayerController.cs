using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : CharacterController
{
    [SerializeField] float punchDistance = 1.0f;
    [SerializeField] float punchForce = 5.0f;

    private void FixedUpdate()
    {
        Move(walkDirection);
    }

    private void Punch()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, punchDistance))
        {
            if (hit.collider.CompareTag("Bot"))
            {
                hit.collider.GetComponent<BotController>().TakePunch();
                hit.collider.GetComponent<Rigidbody>().AddForce(transform.forward * punchForce, ForceMode.Impulse);
            }
        }   
    }

    #region input
    public void OnInteract(CallbackContext value)
    {
        if (value.phase == InputActionPhase.Started)
        {
            SetState(CharacterState.Interact);
            return;
        }
    }

    public void OnAttack(CallbackContext value)
    {
        if (value.phase == InputActionPhase.Started)
        {
            SetState(CharacterState.Attack);
            return;
        }
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

        if (walkDirection.magnitude < 0.5f)
        {
            SetState(CharacterState.Walk);
        }
        else
        {
            SetState(CharacterState.Run);
        }
    }
    #endregion input
}
