using UnityEngine;
using UnityEngine.AI;

public class TestMoveNavMesh : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000))
            {
                agent.destination = hit.point;
            }
        }
    }
}
