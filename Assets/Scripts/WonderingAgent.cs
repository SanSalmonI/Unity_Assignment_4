using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class WonderingAgent : MonoBehaviour
{
    [Header("AI Settings")]
    public NavMeshAgent navMeshAgent;
    public Transform chaseTarget;

    [Header("Waypoints")]
    [Tooltip("List of all waypoints the agent can visit.")]
    public List<Transform> allWaypoints = new List<Transform>();
    // Separate list to keep track of which waypoints remain unvisited
    private List<Transform> unvisitedWaypoints = new List<Transform>();

    [Header("State Objects")]
    [Tooltip("Assign a GameObject for Patrol state.")]
    public GameObject patrolObject;
    [Tooltip("Assign a GameObject for Chase state.")]
    public GameObject chaseObject;
    [Tooltip("Assign a GameObject for Investigate state.")]
    public GameObject investigateObject;

    private AgentState agentState;

    void Start()
    {
        // Initialize unvisitedWaypoints with a copy of allWaypoints
        unvisitedWaypoints.AddRange(allWaypoints);

        // Choose the first random destination if we have any waypoints
        if (unvisitedWaypoints.Count > 0)
        {
            SetRandomDestination();
        }

        agentState = AgentState.Patrol;

        // Initialize state objects visibility
        UpdateStateObjects();
    }

    void Update()
    {
        switch (agentState)
        {
            case AgentState.Patrol:
                PatrolLogic();
                break;

            case AgentState.Chase:
                ChaseLogic();
                break;

            case AgentState.Investigate:
                InvestigateLogic();
                break;
        }

        // Update the objects each frame to ensure correct state object is active
        UpdateStateObjects();
    }

    /// <summary>
    /// Picks a random waypoint from the unvisited list, sets the agent’s destination,
    /// and removes that waypoint from the unvisited list.
    /// </summary>
    private void SetRandomDestination()
    {
        int randomIndex = Random.Range(0, unvisitedWaypoints.Count);
        navMeshAgent.SetDestination(unvisitedWaypoints[randomIndex].position);
        unvisitedWaypoints.RemoveAt(randomIndex);
    }

    private void PatrolLogic()
    {
        // If the agent has reached the current waypoint or is close enough
        if (!navMeshAgent.pathPending &&
            navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            // If we’ve visited all waypoints, refill the unvisited list
            if (unvisitedWaypoints.Count == 0)
            {
                unvisitedWaypoints.AddRange(allWaypoints);
            }

            // Set another random destination
            SetRandomDestination();
        }
    }

    private void ChaseLogic()
    {
        if (chaseTarget == null) return;

        navMeshAgent.SetDestination(chaseTarget.position);

        // If the target is too far, switch back to patrol
        if (Vector3.Distance(transform.position, chaseTarget.position) > 15f)
        {
            agentState = AgentState.Patrol;
        }
    }

    private void InvestigateLogic()
    {
        // For Investigate, we can make the AI stop
        navMeshAgent.SetDestination(transform.position);
    }

    public void PlayerSpotted(Transform playerTarget)
    {
        chaseTarget = playerTarget;
        agentState = AgentState.Chase;
    }

    public void StartInvestigate()
    {
        agentState = AgentState.Investigate;
    }

    public void EndInvestigate()
    {
        if (agentState == AgentState.Investigate)
        {
            agentState = AgentState.Patrol;
        }
    }


    private void UpdateStateObjects()
    {
        if (patrolObject != null)
            patrolObject.SetActive(agentState == AgentState.Patrol);

        if (chaseObject != null)
            chaseObject.SetActive(agentState == AgentState.Chase);

        if (investigateObject != null)
            investigateObject.SetActive(agentState == AgentState.Investigate);
    }
}

public enum AgentState
{
    Patrol,
    Chase,
    Investigate
}
