using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum OffMeshLinkMoveMethod
{
    NormalSpeed,
    Parabola,
    Curve,
    Teleport
}

[RequireComponent(typeof(NavMeshAgent))]
public class AgentLinkMover : MonoBehaviour
{
    public OffMeshLinkMoveMethod Jump_Method = OffMeshLinkMoveMethod.Parabola;
    public AnimationCurve Jump_Curve = new AnimationCurve();
    public OffMeshLinkMoveMethod Connection_Method = OffMeshLinkMoveMethod.NormalSpeed;
    public AnimationCurve Connection_Curve = new AnimationCurve();
    public Transform cylinder;
    private float speed;
    private NavMeshAgent agent;
    [SerializeField] private BodyFollowAgent body;
    [SerializeField] private float maxDistanceMultiplier;
    [SerializeField] private float maxDistance;
    private bool inLink = false;
    private Vector3 lastPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        speed = agent.speed;
        agent.autoTraverseOffMeshLink = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        if (DistanceStop() > maxDistance)
        {
            agent.speed = speed / (1 + (DistanceStop() * 2));
        }
        else
        {
            agent.speed = speed;
        }
        rotateAroundCylinder();
        if (agent.isOnOffMeshLink && !inLink)
        {
            if (agent.currentOffMeshLinkData.owner.GameObject().tag == "JumpLink")
            {
                if (Jump_Method == OffMeshLinkMoveMethod.NormalSpeed)
                    StartCoroutine(NormalSpeed(agent));
                else if (Jump_Method == OffMeshLinkMoveMethod.Parabola)
                    StartCoroutine(Parabola(agent, 2.0f, 0.5f));
                else if (Jump_Method == OffMeshLinkMoveMethod.Curve)
                    StartCoroutine(Curve(agent, 0.5f, Jump_Curve));

            }
            else
            {
                if (Connection_Method == OffMeshLinkMoveMethod.NormalSpeed)
                    StartCoroutine(NormalSpeed(agent));
                else if (Connection_Method == OffMeshLinkMoveMethod.Parabola)
                    StartCoroutine(Parabola(agent, 2.0f, 0.5f));
                else if (Connection_Method == OffMeshLinkMoveMethod.Curve)
                    StartCoroutine(Curve(agent, 0.5f, Connection_Curve));
                else
                    agent.CompleteOffMeshLink();
            }
        }
        lastPosition = agent.transform.position;

    }

    private float DistanceStop()
    {
        return Mathf.Abs(Vector3.Distance(this.transform.position, body.gameObject.transform.position));
    }

    private float maxDistanceSpeed()
    {
        return speed * maxDistanceMultiplier;
    }

    IEnumerator NormalSpeed(NavMeshAgent agent)
    {
        inLink = true;
        Debug.Log("Normal Movement");
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 endPos = data.endPos + (-1 * GameplayManager.Instance.GetGravity(data.endPos).normalized) * agent.baseOffset;
        bool usedLastPosition = false;
        while (agent.transform.position != endPos)
        {
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * Time.deltaTime);
            if (!usedLastPosition)
            {
                agent.transform.position = lastPosition;
                usedLastPosition = true;
            }
            yield return null;
        }
        Debug.Log("End movement speed");
        agent.CompleteOffMeshLink();
        inLink = false;
    }

    IEnumerator Parabola(NavMeshAgent agent, float height, float duration)
    {
        inLink = true;
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + (-1 * GameplayManager.Instance.GetGravity(data.endPos).normalized) * (agent.baseOffset / 2);
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * (-1 * GameplayManager.Instance.GetGravity(agent.transform.position).normalized);
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        agent.CompleteOffMeshLink();
        inLink = false;
    }

    IEnumerator Curve(NavMeshAgent agent, float duration, AnimationCurve curve)
    {
        inLink = true;
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + (-1 * GameplayManager.Instance.GetGravity(data.endPos).normalized) * agent.baseOffset;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = curve.Evaluate(normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * (-1 * GameplayManager.Instance.GetGravity(agent.transform.position).normalized);
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        agent.CompleteOffMeshLink();
        inLink = false;
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
}