using UnityEngine;
using UnityEngine.AI;

public class ManualFolloeNavMeshPath : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform target;
    public float speed = 3.5f;

    void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        // Disable automatic position updates
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // Set the destination for the agent
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    void Update()
    {
        if (agent.pathPending || ((agent.remainingDistance <= agent.stoppingDistance) && agent.isOnOffMeshLink))
        {
            Debug.Log(agent.pathPending + " " + (agent.remainingDistance <= agent.stoppingDistance));
            return; // Wait for the path to be calculated or stop if the destination is reached
        }

        Vector3 targetPosition = agent.nextPosition;
        if (agent.isOnOffMeshLink)
        {
            targetPosition = agent.path.corners[0] + (Vector3.up * agent.baseOffset);
            if (Vector3.Distance(transform.position, agent.path.corners[0] + (Vector3.up * agent.baseOffset)) <= (agent.stoppingDistance + 0.001))
            {
                agent.CompleteOffMeshLink();
            }
        }

        // Calculate the next position along the path
        Vector3 nextPosition = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Update the agent's position manually
        transform.position = nextPosition;

        // Synchronize the agent's position with the transform
        agent.nextPosition = transform.position;
    }
}
