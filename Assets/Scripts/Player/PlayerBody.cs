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

    private float minGrappleDist = 0.5f;

    private const float ROTATION_THRESHOLD = 0.2f;

    [SerializeField] private ParticleSystem damageParticle;
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
    private bool grapple = false;
    public bool Grapple
    {
        get { return grapple; }
        set { grapple = value; }
    }

    private Vector3 targetGrapplePoint = Vector3.zero;

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

    [SerializeField, Range(0, 1)] private float glideStrength = 0.1f;
    [SerializeField] private float grappleStrength = 10f;
    [SerializeField] private float crashSpeed = 10f;
    [SerializeField] private float crashDuration = 2;
    private float currentCrashDuration = 2;

    private bool crash = false;
    public bool Crash
    {
        get { return crash; }
        set { crash = value; }
    }

    private void Awake()
    {
        currentHP = maxHP;
        if (damageParticle)
        {
            damageParticle.transform.parent = null;
            damageParticle.gameObject.SetActive(true);
        }
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

        targetGrapplePoint = getTargetGrapplePoint();

        if (grapple && Vector3.Distance(transform.position, targetGrapplePoint) < minGrappleDist)
        {
            grapple = false;

        }
        if (crash)
        {
            currentCrashDuration += Time.deltaTime;
            Debug.Log("CRASHING");
        }
        else
        {
            currentCrashDuration = 0;
        }
        if (currentCrashDuration > crashDuration)
        {
            crash = false;
        }
    }

    private Vector3 getTargetGrapplePoint()
    {
        if (grapple) return this.targetGrapplePoint;

        Vector3 targetGrapplePoint = Vector3.zero;
        float maxDepth = 100;


        foreach (GameObject go in GrapplePoint.VisiblePoints)
        {
            Debug.DrawLine(transform.position, go.transform.position, Color.red);
            if (targetGrapplePoint == Vector3.zero) targetGrapplePoint = go.transform.position;

            Vector3 camToNextPoint = go.transform.position - cam.transform.position;
            Vector3 camToCurrentPoint = targetGrapplePoint - cam.transform.position;

            //if (Vector3.Distance(cam.transform.position, go.transform.position) < Vector3.Distance(cam.transform.position, transform.position)) continue; //this will be a check to see if the grapple point is behind the player;

            float currentDist = Vector3.ProjectOnPlane(camToCurrentPoint, cam.transform.forward).magnitude;
            float nextDist = Vector3.ProjectOnPlane(camToNextPoint, cam.transform.forward).magnitude;
            if (nextDist < currentDist) targetGrapplePoint = go.transform.position;

        }

        Debug.DrawLine(transform.position, targetGrapplePoint, Color.green);

        return targetGrapplePoint;
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
                rb.AddForce(-rb.linearVelocity.normalized * deceleration, ForceMode.Force);
            }
        }

        if (IsGrounded() && rb.linearVelocity.magnitude >= currentMaxSpeed)
        {
            rb.AddForce(-rb.linearVelocity.normalized * deceleration, ForceMode.Force);
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

        if (rb.linearVelocity.magnitude < 0.1) rb.linearVelocity = Vector3.zero;

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
        if (glide && Vector3.Dot(rb.linearVelocity, gravity) >= 0) //check if moving down
        {
            rb.AddForce(-gravity * glideStrength, ForceMode.Force);
        }

        //grapple
        if (grapple && targetGrapplePoint != Vector3.zero)
        {
            float currentSpeed = rb.linearVelocity.magnitude;
            float dot = Vector3.Dot(rb.linearVelocity, targetGrapplePoint - transform.position);

            rb.linearVelocity = Vector3.zero;

            Vector3 grappleForce = (targetGrapplePoint - transform.position).normalized * ((dot > 0.5) ? Mathf.Max(currentSpeed, grappleStrength) : grappleStrength);
            rb.AddForce(grappleForce, ForceMode.Acceleration);
        }

        //crash
        if (crash)
        {
            rb.linearVelocity = Vector3.zero;
            Vector3 crashForce = Vector3.Cross(-gravity, -cam.transform.right).normalized * crashSpeed;
            rb.AddForce(crashForce, ForceMode.Acceleration);
        }

        //max velocity
        if (rb.linearVelocity.magnitude > velCap)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * velCap;
        }
        currentMaxSpeed = maxSpeed;


    }

    private void rotateBody()
    {
        if (grapple)
        {
            transform.rotation = Quaternion.LookRotation(targetGrapplePoint - transform.position, -gravity);
        }
        if (crash)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(-gravity, -cam.transform.right), -gravity);
        }

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

    public void modifyHP(int hpChange)
    {
        currentHP = Mathf.Clamp(currentHP + hpChange, 0f, maxHP);
    }

    public void setHP(int hp)
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

    private void OnCollisionEnter(Collision collision)
    {
        if (crash)
        {
            crash = false;
            BreakableWall wall = collision.gameObject.GetComponent<BreakableWall>();
            if (wall != null)
            {
                wall.Break();

            }
        }
    }

    public void hitEffect(Vector3 position, Vector3 forwardDirection)
    {
        damageParticle.transform.position = position;
        damageParticle.transform.forward = forwardDirection;
        damageParticle.Play();
    }
}
