using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float followSpeed = 1f;

    private Camera cam;

    private Vector3 targetPosition = Vector3.zero;

    private void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        targetPosition = target.transform.position + offset;

        float camDist = Vector3.Distance(transform.position, targetPosition);

        float camMove = followSpeed * camDist * Time.deltaTime;

        Vector3 moveStep = (targetPosition - transform.position) * Mathf.Min(camMove, camDist);

        transform.position += moveStep;

    }

}
