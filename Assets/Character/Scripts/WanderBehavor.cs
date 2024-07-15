using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderBehavor : IBotBehavior
{
    public BotController Controller { get; set; }
    private NavMeshAgent agent;
    private float restTime;

    public WanderBehavor(BotController botController, NavMeshAgent agent, float restTime)
    {
        Controller = botController;
        this.agent = agent;
        this.restTime = restTime;
    }

    public void Start()
    {
        agent.speed = Controller.WalkSpeed;
        agent.angularSpeed = Controller.RotationSpeed;
        agent.isStopped = false;
    }

    public void Update()
    {
        if (agent.remainingDistance < 0.5f)
        {
            Controller.StartCoroutine(Rest());
            Vector3 newPos = RandomNavSphere(Controller.transform.position, 15f, -1);
            agent.SetDestination(newPos);
        }
    }

    public void Exit()
    {
        agent.isStopped = true;
    }

    private IEnumerator Rest()
    {
        agent.isStopped = true;
        Controller.SetState(CharacterState.Idle);
        yield return new WaitForSeconds(restTime);
        Controller.SetState(CharacterState.Walk);
        agent.isStopped = false;
    }

    private Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        while (true)
        {
            Vector3 randomDirection = Random.insideUnitSphere * distance;
            randomDirection += origin;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, distance, layermask);

            if (IsDestinationReachable(hit.position))
            {
                return hit.position;
            }
        }
    }

    private bool IsDestinationReachable(Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(destination, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }

}
