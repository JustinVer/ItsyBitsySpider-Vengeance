using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float followSpeed = 1f;
    [SerializeField] private float sensitivity = 1f;

    private Vector2 mouseInput = Vector2.zero;
    private Vector2 lastMouse = Vector2.zero;


    private Camera cam;

    private Vector3 targetPosition = Vector3.zero;

    private void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        mouseInput = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }

    private void FixedUpdate()
    {
        Vector3 gravity = GameplayManager.Instance.GetGravity(target.transform.position);




        Quaternion armRotation = Quaternion.LookRotation(cam.transform.forward, -gravity);
        Quaternion camRotation = Quaternion.LookRotation(cam.transform.forward, -gravity);
        //camRotation *= Quaternion.AngleAxis((mouseInput - lastMouse).x * sensitivity, -gravity);
        targetPosition = target.transform.position + (armRotation * offset);

        cam.transform.rotation = camRotation;

        float camDist = Vector3.Distance(transform.position, targetPosition);

        float camMove = followSpeed * camDist * Time.fixedDeltaTime;

        Vector3 moveStep = (targetPosition - transform.position) * Mathf.Min(camMove, camDist);

        transform.position += moveStep;
        lastMouse = mouseInput;
    }

}
