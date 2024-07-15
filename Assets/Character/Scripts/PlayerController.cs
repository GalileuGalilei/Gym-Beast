using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : CharacterController
{
    [SerializeField] float punchDistance = 10.0f;
    [SerializeField] float punchForce = 5.0f;
    [SerializeField] float elasticConstant = 0.5f;
    const float offset = 4.0f;

    public List<BotController> botStack = new();
    private List<Vector3> botStackIntertia = new();

    private void FixedUpdate()
    {
        Move(walkDirection);
        SimulateStackPhysics();
    }

    private void Punch()
    {
        RaycastHit hit;
        animator.Play("Punching", 1);
        animator.SetLayerWeight(1, 1);
        if (Physics.Raycast(transform.position, transform.forward, out hit, punchDistance))
        {
            if (hit.collider.CompareTag("Bot"))
            {
                Debug.Log("Punching");
                hit.collider.GetComponent<BotController>().TakePunch();
                hit.collider.GetComponent<Rigidbody>().AddForce(transform.forward * punchForce, ForceMode.Impulse);
            }
        }   
    }

    private void StackBot()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2.0f);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Bot"))
            {   
                Transform parent = GetLastParent(col.transform);
                BotController bot = parent.GetComponent<BotController>();
                if (!botStack.Contains(bot))
                {
                    Debug.Log("Stacking");
                    botStack.Add(bot);
                    botStackIntertia.Append(bot.GetComponent<Rigidbody>().velocity);
                    botStackIntertia.Add(bot.GetComponent<Rigidbody>().velocity);
                    bot.transform.position = transform.position + Vector3.up * offset;
                }
            }
        }
    }

    private Transform GetLastParent(Transform transform)
    {
        if (transform.parent == null)
        {
            return transform;
        }

        return GetLastParent(transform.parent);
    }

    private void SimulateStackPhysics()
    {
        foreach (BotController bot in botStack)
        {
            Vector3 target = transform.position + Vector3.up * offset;
            Vector3 dir = target - bot.transform.position;
            Vector3 force = elasticConstant * dir;
            bot.Spine.transform.position += force * Time.deltaTime;



        }
    }

    #region input
    public void OnInteract(CallbackContext value)
    {
        if (value.phase == InputActionPhase.Started)
        {
            SetState(CharacterState.Interact);
            StackBot();
            return;
        }
    }

    public void OnAttack(CallbackContext value)
    {
        if (value.phase == InputActionPhase.Started)
        {
            SetState(CharacterState.Attack);
            Punch();
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
