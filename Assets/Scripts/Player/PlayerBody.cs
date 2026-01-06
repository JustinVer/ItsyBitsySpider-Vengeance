using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    private Rigidbody rb;

    private Vector3 movementDir = Vector3.zero;
    private Vector3 actualVel = Vector3.zero;


    public Vector3 MovementDir
    {
        get { return movementDir; }
        set { movementDir = value; }
    }


    [SerializeField] private float maxSpeed = 1f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float maxAcceleration = 10f;
    [SerializeField] private float rideHight = 1;
    [SerializeField] private float maxRayDist = 1;
    [SerializeField] private float springForce = 10;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        movePlayer();
    }

    private void movePlayer()
    {
        Vector3 gravity = GameplayManager.Instance.GetGravity(gameObject);

        Vector3 targetVel = movementDir * maxSpeed;
        actualVel = Vector3.MoveTowards(actualVel, targetVel, acceleration * Time.fixedDeltaTime);
        Vector3 neededForce = ((rb.mass * targetVel) - (rb.mass * rb.linearVelocity)) / Time.fixedDeltaTime;
        neededForce = Vector3.ClampMagnitude(neededForce, maxAcceleration);
        rb.AddForce(neededForce);
        rb.linearVelocity = actualVel;
        rb.AddForce(gravity, ForceMode.Acceleration);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, gravity, out hit, maxRayDist))
        {
            // Debug.Log(hit.distance);
            Vector3 offset = gravity * (hit.distance - rideHight) * springForce;

            Debug.Log(offset);
            if (Vector3.Dot(offset, gravity) < 0)
            {
                rb.AddForce(offset);
            }
        }
    }
}
