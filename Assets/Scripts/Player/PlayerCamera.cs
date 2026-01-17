using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float followSpeed = 1f;
    [SerializeField] private float sensitivity = 30f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80;

    private Vector2 mouseInput = Vector2.zero;
    


    private Camera cam;

    private Vector3 targetPosition = Vector3.zero;

    private float yaw = 0;
    private float pitch = 0;

    private Vector2 lastMousePos = Vector2.zero;
    

    private void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (Input.mousePosition.x != lastMousePos.x)
        {
            yaw += (Input.mousePosition.x - lastMousePos.x) * sensitivity * Time.deltaTime;
        }
        if (Input.mousePosition.y != lastMousePos.y) {
            pitch -= (Input.mousePosition.y - lastMousePos.y) * sensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
        

        lastMousePos = Input.mousePosition;
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

        float camDist = Vector3.Distance(transform.position, targetPosition);
        float camMove = followSpeed * camDist * Time.fixedDeltaTime;
        Vector3 moveStep = (targetPosition - transform.position) * Mathf.Min(camMove, camDist);

        transform.position = targetPosition;
        cam.transform.rotation = camRotation;
       
    }

    

}
