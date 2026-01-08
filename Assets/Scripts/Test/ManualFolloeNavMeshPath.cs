using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ManualFolloeNavMeshPath : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform target;
    public Transform cylinder;
    public float speed = 3.5f;
    [SerializeField] private BodyFollowAgent body;
    [SerializeField] private float maxDistanceFromFollow = 0.6f;
    private float currentMaxDistance;

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
        if (agent.pathPending || ((agent.remainingDistance <= agent.stoppingDistance) && agent.isOnOffMeshLink) || Mathf.Abs(Vector3.Distance(this.transform.position, body.gameObject.transform.position)) > maxDistanceFromFollow)
        {
            return; // Wait for the path to be calculated or stop if the destination is reached
        }

        Vector3 targetPosition = agent.nextPosition;
        if (agent.isOnOffMeshLink)
        {
            if (agent.currentOffMeshLinkData.owner.GameObject().tag == "JumpLink")
            {
                body.Jump(agent.currentOffMeshLinkData.startPos, agent.currentOffMeshLinkData.endPos);
                targetPosition = agent.path.corners[0];
                transform.position = targetPosition;
                agent.nextPosition = transform.position;
                agent.CompleteOffMeshLink();
                transform.position = targetPosition;
                agent.nextPosition = transform.position;
            }
            else
            {
                targetPosition = agent.path.corners[0] + (Vector3.up * agent.baseOffset);
                if (Vector3.Distance(transform.position, agent.path.corners[0] + (Vector3.up * agent.baseOffset)) <= (agent.stoppingDistance + 0.001))
                {
                    agent.CompleteOffMeshLink();
                }
            }
        }

        // Calculate the next position along the path
        Vector3 nextPosition = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Update the agent's position manually
        transform.position = nextPosition;

        // Synchronize the agent's position with the transform
        agent.nextPosition = transform.position;


        rotateAroundCylinder();

    }

    private void rotateAroundCylinder()
    {
        Vector3 direction = transform.position - cylinder.position;
        direction.z = 0;  // Flatten to XZ plane

        // Calculate angle
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        // Apply rotation (object faces outward from cylinder)
        transform.rotation = Quaternion.Euler(0, 0, angle * -1);
    }

    public void endJump()
    {

    }
}
