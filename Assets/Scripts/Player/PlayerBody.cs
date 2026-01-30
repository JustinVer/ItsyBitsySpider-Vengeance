using UnityEngine;

public class PlayerBody : MonoBehaviour, IDamageable
{
    private Rigidbody rb;

    private Vector3 movementDir = Vector3.zero;
    private Vector3 gravity = Vector3.zero;

    private bool doJump = false;

    private bool doRideForce = false;

    private float currentMaxSpeed;

    private float currentJumpDelay = 0;
    private float jumpDelay = 1;

    private const float ROTATION_THRESHOLD = 0.1f;
    public Vector3 MovementDir
    {
        get { return movementDir; }
        set { movementDir = value; }
    }

    private bool glide = false;
    public bool Glide
    {
        get { return glide; }
        set { glide = value; }
    }

    private float currentHP;
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 5f;
    [SerializeField] private float maxSpeed = 10f;

    [SerializeField] private float jumpForce = 100f;

    [SerializeField] private float velCap = 100f;

    [SerializeField] private float gravityMod = 1f;

    [SerializeField] private float rideheight = 1f;
    [SerializeField] private float rideSpringStrength = 5f;
    [SerializeField] private float rideSpringDamp = 5f;
    [SerializeField] private float maxRayDist = 2f;

    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float airAngle = 120f;
    [SerializeField] private float groundAngle = 5f;

    [SerializeField] private Camera cam;

    [SerializeField, Range(0, 1)] private float glideStrength = 0.5f;

    private void Awake()
    {
        currentHP = maxHP;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        gravity = GameplayManager.Instance.GetGravity(transform.position);
        movePlayer();
        rotateBody();

        Vector3 targetGrapplePoint = Vector3.zero;
        float maxDepth = 100;


        foreach (GameObject go in GrapplePoint.VisiblePoints)
        {
            Debug.DrawLine(transform.position, go.transform.position, Color.red);
            if (targetGrapplePoint == Vector3.zero) targetGrapplePoint = go.transform.position;

            float currentDist = Vector3.Distance(targetGrapplePoint, Vector3.Project(targetGrapplePoint, cam.transform.position + cam.transform.forward));
            float nextDist = Vector3.Distance(go.transform.position, Vector3.Project(targetGrapplePoint, cam.transform.position + cam.transform.forward));
            if (nextDist < currentDist) targetGrapplePoint = go.transform.position;
        }
        Debug.DrawRay(cam.transform.position, cam.transform.position + cam.transform.forward, Color.blue);
        Debug.DrawLine(transform.position, targetGrapplePoint, Color.green);
    }

    public bool IsGrounded()
    {
        //dimensions of the sphere
        float sphereRadius = 0.5f;
        float castDist = rideheight * 1.1f; //adds a 10% margin

        RaycastHit rayHit;

        return Physics.SphereCast(transform.position, sphereRadius, gravity, out rayHit, castDist);

    }

    private bool canJump()
    {
        return IsGrounded() && currentJumpDelay < 0;
    }

    public void Jump()
    {
        //Debug.Log(IsGrounded());
        if (canJump())
        {
            doJump = true;
        }
    }

    private void movePlayer()
    {
        Vector3 camRight = Vector3.Cross(cam.transform.forward, -gravity);
        Vector3 trueForward = Vector3.Cross(-gravity, camRight);

        movementDir = Quaternion.LookRotation(trueForward, -gravity) * movementDir;


        if (movementDir != Vector3.zero)
        {
            //would new speed go above max speed
            Vector3 targetVel = rb.linearVelocity + (MovementDir * acceleration) / rb.mass * Time.fixedDeltaTime;

            if (targetVel.magnitude <= currentMaxSpeed)
            {
                rb.AddForce(MovementDir * acceleration, ForceMode.Force);

                //turning assist
                if (IsGrounded())
                {
                    Vector3 turnForce = Vector3.zero;
                    if (Vector3.Dot(movementDir, rb.linearVelocity) < -0.9)
                    {
                        turnForce = -rb.linearVelocity * deceleration;
                    }
                    else
                    {
                        Vector3 forwardVel = Vector3.Project(rb.linearVelocity, movementDir);
                        turnForce = -(rb.linearVelocity - forwardVel) * deceleration;
                    }
                    rb.AddForce(turnForce, ForceMode.Force);
                }

            }
            //else if (rb.linvel < maxSpeed) calc what force would get it to max and apply that 
        }
        else
        {
            //decel if on ground
            if (IsGrounded())
            {
                rb.AddForce(-rb.linearVelocity * deceleration, ForceMode.Force);
            }
        }

        RaycastHit rayHit;
        if (Physics.Raycast(transform.position, gravity, out rayHit, maxRayDist))
        {

            float x = rayHit.distance - rideheight;

            float springForce = (x * rideSpringStrength);

            Vector3 springVector = gravity.normalized * springForce;
            if (x < 0 && currentJumpDelay < 0)
            {
                doRideForce = true;
            }

            if (doRideForce)
            {
                rb.AddForce(springVector, ForceMode.Acceleration);
            }

        }

        //gravity
        rb.AddForce(gravity * gravityMod, ForceMode.Acceleration);

        currentJumpDelay -= Time.fixedDeltaTime;
        if (doJump)
        {
            rb.AddForce(-gravity.normalized * jumpForce, ForceMode.Impulse);
            doJump = false;
            doRideForce = false;
            currentJumpDelay = jumpDelay;
        }

        //glide
        if (glide)
        {
            rb.AddForce(-gravity * glideStrength, ForceMode.Force);
        }

        //max velocity
        if (rb.linearVelocity.magnitude > velCap)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * velCap;
        }
        currentMaxSpeed = maxSpeed;
    }
    //TODO fix this
    private void rotateBody()
    {


        Vector3 moveDir = (rb.linearVelocity.magnitude > ROTATION_THRESHOLD) ? rb.linearVelocity : transform.forward;

        Vector3 up = -gravity.normalized;
        Vector3 forward = Vector3.ProjectOnPlane(moveDir, up).normalized;
        Vector3 left = Vector3.Cross(up, forward);

        float pitchAngle = Vector3.SignedAngle(forward, moveDir, left);
        pitchAngle = IsGrounded() ? Mathf.Clamp(pitchAngle, -(groundAngle / 2), groundAngle / 2) : Mathf.Clamp(pitchAngle, -(airAngle / 2), airAngle / 2);

        Quaternion baseRot = Quaternion.LookRotation(forward, up);
        Quaternion pitch = Quaternion.Euler(pitchAngle, 0, 0);

        Quaternion targetRot = baseRot * pitch;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }

    public void ApplyForce(Vector3 force, ForceMode forceMode)
    {
        rb.AddForce(force, forceMode);
    }

    public void Slow(float slowPercent)
    {
        currentMaxSpeed = maxSpeed * slowPercent;
    }

    public float getHP()
    {
        return currentHP;
    }

    public void modifyHP(float hpChange)
    {
        currentHP = Mathf.Clamp(currentHP + hpChange, 0f, maxHP);
    }

    public void setHP(float hp)
    {
        currentHP = Mathf.Clamp(hp, 0f, maxHP);
    }

    public float getMaxHP()
    {
        return maxHP;
    }

    public Vector3 LinearVelocity()
    {
        return rb.linearVelocity;
    }
}
