using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevelPhysics;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : CharacterController
{
    [SerializeField] float punchDistance = 10.0f;
    [SerializeField] float punchForce = 5.0f;
    [SerializeField] float elasticConstant = 0.5f;
    [SerializeField] float offset = 1.0f;
    [SerializeField] PlayerInteract playerInteract;

    public List<BotController> botStack = new();
    private List<Vector3> botStackIntertia = new();

    public void FixedUpdate()
    {
        Move(walkDirection);
        SimulateStackPhysics();
    }

    public void Update()
    {
        //SimulateStackPhysics();
    }

    private void Punch()
    {
        RaycastHit hit;
        animator.Play("Punching", 1);
        animator.SetLayerWeight(1, 1);
        

        Debug.Log("Punching");
        BotController bot = playerInteract.botInRange.FirstOrDefault();
        bot.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * punchForce, ForceMode.Impulse);
        bot.TakePunch();

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
                    bot.GetComponent<Rigidbody>().isKinematic = true;
                    bot.Spine.isKinematic = true;
                    bot.transform.position = transform.position + Vector3.up * offset * botStack.Count;
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
        Bounds playerBounds = col.bounds;
        Vector3 previusTarget = transform.position + playerBounds.size.y * Vector3.up;
        foreach (BotController bot in botStack)
        {
            Vector3 target = previusTarget + Vector3.up * offset;
            bot.Spine.position = new Vector3(bot.Spine.transform.position.x, target.y, bot.Spine.transform.position.z);
            bot.Spine.transform.forward = Vector3.down;

            // Apply elastic force in xz plane
            Vector3 force = elasticConstant * (target - bot.Spine.transform.position);
            force.y = 0;
            bot.Spine.position += force * elasticConstant;
            previusTarget = bot.Spine.position;
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
