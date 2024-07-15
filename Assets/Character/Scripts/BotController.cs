using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotController : CharacterController
{
    private NavMeshAgent agent;
    [SerializeField] float restTime = 6f;

    public IBotBehavior CurrentBehavior { get; private set; }
    public bool Alive { get; private set; } = true;
    public Rigidbody Spine;
    

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        CurrentBehavior = new WanderBehavor(this, agent, restTime);
    }

    private void Update()
    {
        CurrentBehavior?.Update();
    }

    public void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public void TakePunch()
    {
        EnableRagdoll(true);
        agent.enabled = false;
        CurrentBehavior = null;
        Alive = false;
    }
}
