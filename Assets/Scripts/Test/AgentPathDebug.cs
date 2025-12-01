using UnityEngine;
using UnityEngine.AI;

public class AgentPathDebug : MonoBehaviour
{
    NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lastposition = transform.position;
        for (int i = 0; i < agent.path.corners.Length; i++)
        {
            Color color = (i % 2 == 0) ? Color.red : Color.blue;
            Debug.DrawLine(lastposition, agent.path.corners[i], color);
            lastposition = agent.path.corners[i];
        }
    }
}
