using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshFixedTraversalLink : MonoBehaviour
{
    NavMeshAgent agent;
    bool MoveAcrossNavMeshesStarted = false;
    Vector3 savedDestination;
    Vector3 savedPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isOnOffMeshLink && !MoveAcrossNavMeshesStarted)
        {
            savedDestination = agent.destination;
            savedPosition = this.transform.position + (agent.velocity * Time.deltaTime);
            agent.CompleteOffMeshLink();
            //agent.CompleteOffMeshLink();
            //agent.transform.position = savedPosition;
            //StartCoroutine(MoveAcrossNavMeshLink());
            //MoveAcrossNavMeshesStarted = true;
        }
    }


    IEnumerator MoveAcrossNavMeshLink()
    {
        /*
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        agent.updateRotation = false;

        Vector3 startPos = agent.transform.position;
        Debug.Log("data end position: " + data.endPos);
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float duration = (endPos - startPos).magnitude / agent.velocity.magnitude;
        float t = 0.0f;
        float tStep = 1.0f / duration;
        while (t < 1.0f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
            agent.destination = transform.position;
            t += tStep * Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
        yield return null;
        agent.updateRotation = true;
        agent.CompleteOffMeshLink();
        agent.destination = savedDestination;
        MoveAcrossNavMeshesStarted = false;*/
        yield return null;
        MoveAcrossNavMeshesStarted = false;
    }
}
