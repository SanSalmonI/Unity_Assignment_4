using UnityEngine;
using UnityEngine.AI;

public class FollowAgent : MonoBehaviour
{
    [Header("AI Settings")]
    public NavMeshAgent navMeshAgent;
    public Transform chaseTarget;  // The player or object to follow

    void Update()
    {
        if (chaseTarget != null)
        {
            navMeshAgent.SetDestination(chaseTarget.position);
        }
    }
}
