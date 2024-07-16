using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevelPhysics;
using UnityEngine.Rendering;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : CharacterController
{
    [SerializeField] float punchForce = 5.0f;
    [SerializeField] float elasticConstant = 0.5f;
    [SerializeField] float offset = 1.0f;
    [SerializeField] PlayerInteract playerInteract;

    public List<BotController> botStack = new();
    private int stackLimit = 1;

    protected override void Start()
    {
        base.Start();
        playerInteract.GetComponent<Collider>().enabled = true;
    }

    public void FixedUpdate()
    {
        Move(walkDirection);
        SimulateStackPhysics();
    }

    private void Punch()
    {
        //animation
        animator.Play("Punching", 1);
        animator.SetLayerWeight(1, 1);
        
        //ragdoll
        Debug.Log("Punching");
        foreach (BotController bot in playerInteract.botInRange)
        {
            if (bot.Alive)
            {
                bot.TakePunch();
                bot.Spine.GetComponent<Rigidbody>().AddForce(transform.forward * punchForce, ForceMode.Impulse);
                playerInteract.botInRange.Remove(bot);
                return;
            }
        }
    }

    private void ThrowBotFromStack()
    {
        animator.Play("Throw", 0);

        if (botStack.Count > 0)
        {
            BotController bot = botStack.Last();
            bot.Spine.isKinematic = false;
            botStack.Remove(bot);
            bot.Spine.AddForce(transform.forward * punchForce, ForceMode.Impulse);
        }
    }

    public void StackBot()
    {
        BotController bot = FindClosestBot(2.0f);

        if (bot != null && !botStack.Contains(bot) && !bot.Alive)
        {
            if (botStack.Count >= stackLimit)
            {
                return;
            }

            Debug.Log("Stacking");
            Bounds playerBounds = col.bounds;
            bot.transform.position = transform.position + playerBounds.size.y * Vector3.up;
            bot.Spine.isKinematic = true;

            botStack.Add(bot);
        }
        
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

    private BotController FindClosestBot(float maxDist)
    {
        BotController[] allBots = FindObjectsByType<BotController>(FindObjectsSortMode.None);
        BotController closestBot = null;
        float closestDistance = Mathf.Infinity;

        foreach (BotController bot in allBots)
        {
            if (botStack.Contains(bot))
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, bot.Spine.transform.position);
            if (distance < closestDistance && distance < maxDist)
            {
                closestDistance = distance;
                closestBot = bot;
            }
        }

        return closestBot;
    }

    #region Upgrades

    public void UpdatePunchForce(float punchForce)
    {
        this.punchForce *= punchForce;
    }

    public void UpdateStackLimit(int stackLimit)
    {
        this.stackLimit += stackLimit;
    }

    public void UpdateSpeed(float speed)
    {
        this.WalkSpeed *= speed;
    }

    public void ChangePlayerColor()
    { 
        Material playerMaterial = GetComponentInChildren<SkinnedMeshRenderer>().material;
        playerMaterial.mainTextureScale = new Vector2(Random.Range(0.1f, 2.0f), Random.Range(0.1f, 2.0f));
    }

    #endregion Upgrades

    #region input

    public void OnThrow(CallbackContext value)
    {
        if (value.phase == InputActionPhase.Started)
        {
            ThrowBotFromStack();
            animator.Play("Throw");
            return;
        }
    }

    public void OnInteract(CallbackContext value)
    {
        if (value.phase == InputActionPhase.Started)
        {
            SetState(CharacterState.Interact);
            animator.Play("Picking Up");
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
