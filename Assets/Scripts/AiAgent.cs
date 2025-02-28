using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AiAgent : MonoBehaviour
{
    [Header("AI Settings")]
    public NavMeshAgent navMeshAgent;
    public Transform chaseTarget;
    public List<Transform> waypoints;

    [Header("State Objects")]
    [Tooltip("Object shown while the AI is in Patrol state.")]
    public GameObject patrolObject;
    [Tooltip("Object shown while the AI is in Chase state.")]
    public GameObject chaseObject;
    [Tooltip("Object shown while the AI is in Investigate state.")]
    public GameObject investigateObject;

    private int currentWaypoint = 0;

    private AIState aiState;
    public AIState CurrentState
    {
        get => aiState;
        set
        {
            if (aiState != value)
            {
                aiState = value;
                UpdateStateObjects();
            }
        }
    }

    void Start()
    {
        if (waypoints.Count > 0)
        {
            navMeshAgent.SetDestination(waypoints[0].position);
        }

        // Set initial state
        CurrentState = AIState.Patrol;
    }

    void Update()
    {
        switch (CurrentState)
        {
            case AIState.Patrol:
                PatrolLogic();
                break;

            case AIState.Chase:
                ChaseLogic();
                break;

            case AIState.Investigate:
                InvestigateLogic();
                break;
        }
    }

    private void PatrolLogic()
    {
        // If agent has reached the waypoint, move to the next
        if (!navMeshAgent.pathPending &&
            navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Count;
            navMeshAgent.SetDestination(waypoints[currentWaypoint].position);
        }
    }

    private void ChaseLogic()
    {
        if (chaseTarget == null) return;

        navMeshAgent.SetDestination(chaseTarget.position);

        // If the target is too far, switch back to patrol
        if (Vector3.Distance(transform.position, chaseTarget.position) > 15f)
        {
            CurrentState = AIState.Patrol;
            navMeshAgent.SetDestination(waypoints[currentWaypoint].position);
        }
    }

    private void InvestigateLogic()
    {
        // For Investigate, we can stop the agent
        navMeshAgent.SetDestination(transform.position);
    }

    public void PlayerSpotted(Transform playerTarget)
    {
        chaseTarget = playerTarget;
        navMeshAgent.SetDestination(chaseTarget.position);
        CurrentState = AIState.Chase;
    }

    public void StartInvestigate()
    {
        CurrentState = AIState.Investigate;
    }

    public void EndInvestigate()
    {
        if (CurrentState == AIState.Investigate)
        {
            CurrentState = AIState.Patrol;
            navMeshAgent.SetDestination(waypoints[currentWaypoint].position);
        }
    }

    private void UpdateStateObjects()
    {
        if (patrolObject) patrolObject.SetActive(CurrentState == AIState.Patrol);
        if (chaseObject) chaseObject.SetActive(CurrentState == AIState.Chase);
        if (investigateObject) investigateObject.SetActive(CurrentState == AIState.Investigate);
    }
}

public enum AIState
{
    Patrol,
    Chase,
    Investigate
}
