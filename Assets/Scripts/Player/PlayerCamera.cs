using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private bool cameraFollow;

    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float followSpeed = 1f;
    [SerializeField] private float sensitivity = 30f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80;

    private const float CAMERA_BUFFER = 0.3f;

    private Camera cam;

    private Vector3 targetPosition = Vector3.zero;

    private float yaw = 0;
    private float pitch = 0;


    private void Start()
    {
        GameplayManager.Instance.resetEvent += resetCamera;
    }

    private void Update()
    {
        yaw += Mouse.current.delta.ReadValue().x * sensitivity;

        pitch -= Mouse.current.delta.ReadValue().y * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private void FixedUpdate()
    {




        Vector3 up = -GameplayManager.Instance.GetGravity(target.transform.position);
        Vector3 foreward = GameplayManager.Instance.GetForward(target.transform.position);
        Quaternion baseRot = Quaternion.LookRotation(foreward, up);

        Quaternion yawRot = Quaternion.AngleAxis(yaw, Vector3.up);
        Quaternion pitchRot = Quaternion.AngleAxis(pitch, Vector3.right);

        Quaternion camRotation = baseRot * yawRot * pitchRot;

        targetPosition = target.transform.position + (camRotation * offset);
        Vector3 updatedOffset = camRotation * offset;
        RaycastHit hit;
        if (Physics.Raycast(target.transform.position, updatedOffset, out hit, updatedOffset.magnitude, GameplayManager.Instance.NotPlayerOrEnemyMask))
        {
            if (hit.distance < updatedOffset.magnitude)
            {
                targetPosition = target.transform.position + (updatedOffset.normalized * (hit.distance - CAMERA_BUFFER));
            }
        }

        float camDist = Vector3.Distance(transform.position, targetPosition);
        float camMove = followSpeed * camDist * Time.fixedDeltaTime;
        Vector3 moveStep = (targetPosition - transform.position) * Mathf.Min(camMove, camDist);

        transform.position = (cameraFollow) ? transform.position + moveStep : targetPosition;
        if (!cam)
        {
            cam = GetComponentInChildren<Camera>();
        }
        else
        {
            cam.transform.rotation = camRotation;
        }

    }


    public void resetCamera()
    {
        yaw = 0;
        pitch = 0;
        targetPosition = Vector3.zero;
    }

}
