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
    private float speed;
    public NavMeshAgent agent { private set; get; }
    [SerializeField] private BodyFollowAgent body;
    [SerializeField] private float maxDistanceMultiplier;
    [SerializeField] private float maxDistance;
    private bool inLink = false;
    private bool waitingForFollowBody = false;
    private Vector3 lastPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        speed = agent.speed;
        agent.autoTraverseOffMeshLink = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.updatePosition = true;

        //StartCoroutine(setStartAgent());
    }

    private IEnumerator setStartAgent()
    {
        yield return new WaitForSeconds(3f);
        yield return null;
        NavMeshHit hit;
        NavMesh.SamplePosition(body.transform.position - GameplayManager.Instance.GetGravity(body.transform.position).normalized, out hit, 3f, NavMesh.AllAreas);

        if (hit.position != null)
        {
            agent.transform.position = hit.position;
        }
        else
        {
            agent.transform.position = Vector3.zero;
        }
        agent.transform.position = Vector3.zero;
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
        agent.CompleteOffMeshLink();
        yield return null;
        body.EndJump();
        inLink = false;
    }

    IEnumerator Parabola(NavMeshAgent agent, float height, float duration)
    {
        inLink = true;
        waitingForFollowBody = true;
        body.StartJump();
        yield return new WaitUntil(() => !waitingForFollowBody);
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
        yield return null;
        body.EndJump();
        this.transform.position = agent.transform.position;
        inLink = false;
    }

    IEnumerator Curve(NavMeshAgent agent, float duration, AnimationCurve curve)
    {
        inLink = true;
        waitingForFollowBody = true;
        body.StartJump();
        yield return new WaitUntil(() => !waitingForFollowBody);
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
        yield return null;
        body.EndJump();
        this.transform.position = agent.transform.position;
        inLink = false;
    }

    private void rotateAroundCylinder()
    {
        if (agent.velocity != Vector3.zero)
        {
            this.transform.rotation = Quaternion.LookRotation(agent.velocity, GameplayManager.Instance.GetGravity(this.transform.position) * -1);
        }
    }

    public void StartJump()
    {
        waitingForFollowBody = false;
    }

    public void SetDestination(Vector3 destination)
    {
        agent.destination = destination;
    }

    public void SetPosition(Vector3 position)
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(position, out hit, 10f, NavMesh.AllAreas);
        if (hit.position != null)
        {
            agent.transform.position = hit.position;
        }
    }
}