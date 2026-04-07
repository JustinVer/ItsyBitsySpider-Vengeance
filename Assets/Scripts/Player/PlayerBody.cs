using System;
using UnityEngine;

public enum PlayerState
{
    IDLE = 0,
    MOVING = 1,
    JUMPING = 2,
    GLIDE = 3,
    GRAPPLE = 4,
    DASH = 5
}


public class PlayerBody : MonoBehaviour, IDamageable
{
    private Rigidbody rb;

    private Vector3 movementDir = Vector3.zero;
    private Vector3 gravity = Vector3.zero;
    private Vector3 up = Vector3.zero;

    private bool doJump = false;

    private bool doRideForce = false;

    private float currentMaxSpeed;

    private float currentJumpDelay = 0;
    private float jumpDelay = 1;

    private float minGrappleDist = 0.5f;

    private const float ROTATION_THRESHOLD = 0.5f;

    [SerializeField] private ParticleSystem damageParticle;
    [SerializeField] Animator animator;
    [SerializeField] LegManager legManager;
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

    [HideInInspector] public Vector3 TargetGrapplePoint = Vector3.zero;
    private Vector3 modGrapplePoint = Vector3.zero;
    public bool ValidGrapplePoint = false;

    private float currentHP;
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 10f;
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

    [SerializeField] private float maxWebs = 10;
    [SerializeField] private float startWebs = 5;

    private float currentWebs;
    public float CurrentWebs
    {
        get { return currentWebs; }
        set
        {
            if (value < maxWebs)
            {
                currentWebs = value;
            }
            else
            {
                currentWebs = maxWebs;
            }
        }
    }

    [SerializeField, Range(0, 1)] private float glideStrength = 0.1f;
    [SerializeField] private float grappleStrength = 10f;
    [SerializeField] private float maxGrappleDist = 50f;
    [SerializeField] private float crashSpeed = 10f;
    [SerializeField] private float crashDuration = 2;

    [SerializeField] private float knockBackHeight = 10f;
    [SerializeField] private float knockBackStrength = 50f;
    private float currentCrashDuration = 2;

    [SerializeField] private AudioClip damage;
    [SerializeField, Range(0, 1)] private float damageVolume = 0.5f;
    [SerializeField] private AudioClip jump;
    [SerializeField, Range(0, 1)] private float jumpVolume = 0.5f;

    private float timeNoWebs = 0;
    [SerializeField] private float maxTimeNoWebs = 15f;

    public Transform startParent;

    [SerializeField] private SphereCollider playerCollider;
    [SerializeField] private float colliderEndRadius;
    private float colliderStartRadius = 0.2f;

    [SerializeField] private GameObject webGrappleObject;
    [SerializeField] private Transform grappleShotPoint;
    [SerializeField] private Vector3 webGrappleOffset;


    private bool crash = false;
    public bool Crash
    {
        get { return crash; }
        set { crash = value; }
    }

    private void Awake()
    {
        currentHP = maxHP;
        currentWebs = maxWebs;
        if (damageParticle)
        {
            damageParticle.transform.parent = null;
            damageParticle.gameObject.SetActive(true);
        }
        startParent = this.transform.parent;
    }

    void Start()
    {
        colliderStartRadius = playerCollider.radius;
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        GameplayManager.Instance.resetEvent += ResetPlayer;
    }

    private void OnDisable()
    {
        if (GameplayManager.Instance != null)
        {
            GameplayManager.Instance.resetEvent -= ResetPlayer;
        }
    }

    private void FixedUpdate()
    {
        //Debug.Log("Current Webs: " + currentWebs);

       

        gravity = GameplayManager.Instance.GetGravity(transform.position);
        up = -gravity.normalized;

        TargetGrapplePoint = getTargetGrapplePoint();
        if (TargetGrapplePoint != Vector3.zero)
        {
            ValidGrapplePoint = true;
            Debug.DrawLine(transform.position, modGrapplePoint, Color.magenta);
        }
        else
        {
            ValidGrapplePoint = false;
        }
        if (TargetGrapplePoint == Vector3.zero)
        {
            grapple = false;
        }
        if (grapple)
        {
            playerCollider.radius = colliderEndRadius;

            //Update web object location, rotation, and scale
            webGrappleObject.SetActive(true);
            Vector3 offsetForward = grappleShotPoint.forward * webGrappleOffset.z;
            Vector3 offsetUp = grappleShotPoint.up * webGrappleOffset.y;
            Vector3 offsetRight = grappleShotPoint.right * webGrappleOffset.x;

            Vector3 offset = offsetForward + offsetUp + offsetRight;

            webGrappleObject.transform.SetPositionAndRotation((TargetGrapplePoint + grappleShotPoint.position + offset) / 2.0f, Quaternion.LookRotation(grappleShotPoint.position + offset - TargetGrapplePoint));
            webGrappleObject.transform.localScale = new Vector3(webGrappleObject.transform.localScale.x, webGrappleObject.transform.localScale.y, Mathf.Abs(Vector3.Distance(TargetGrapplePoint, grappleShotPoint.position + offset) / 2.0f));

            updateGrappleDirection();
        }
        else
        {
            webGrappleObject.SetActive(false);
            playerCollider.radius = colliderStartRadius;
        }
        movePlayer();
        rotateBody();



        if (grapple && (Vector3.Distance(transform.position, TargetGrapplePoint) < minGrappleDist))
        {
            webGrappleObject.SetActive(false);
            grapple = false;
        }
        if (crash)
        {
            currentCrashDuration += Time.deltaTime;
        }
        else
        {
            currentCrashDuration = 0;
        }
        if (currentCrashDuration > crashDuration)
        {
            crash = false;
        }
        if (currentHP <= 0) onDeath();
        animator.SetInteger("State", (int)evaluateState());

        if (currentWebs < 1)
        {
            timeNoWebs += Time.fixedDeltaTime;
            if (timeNoWebs > maxTimeNoWebs)
            {
                currentWebs++;
                timeNoWebs = 0;
            }
        }
    }

    private PlayerState evaluateState()
    {
        PlayerState state;

        //evaluate booleans
        Vector3 planarVel = Vector3.ProjectOnPlane(rb.linearVelocity, gravity);
        bool moving = planarVel.sqrMagnitude > 0.25;
        state = PlayerState.IDLE;
        legManager.Animating = false;

        if (moving)
        {
            state = PlayerState.MOVING;
        }

        if (!IsGrounded())
        {
            state = PlayerState.JUMPING;
            if (glide)
            {
                state = PlayerState.GLIDE;
                legManager.Animating = true;
            }
        }

        if (crash)
        {
            state = PlayerState.DASH;
            legManager.Animating = true;
        }

        if (grapple)
        {
            state = PlayerState.GRAPPLE;
            legManager.Animating = true;
        }

        //animator.SetBool("Grapple", grapple);
        //animator.SetBool("Glide", glide);
        //animator.SetBool("Dash", crash);

        return state;
    }

    private Vector3 getTargetGrapplePoint()
    {
        if (grapple) return this.TargetGrapplePoint;

        Vector3 targetGrapplePoint = Vector3.zero;

        if (GrapplePoint.VisiblePoints == null)
        {
            return Vector3.zero;
        }

        foreach (GameObject go in GrapplePoint.VisiblePoints)
        {
            if (go == null) continue;

            if (Vector3.Dot(go.transform.position - transform.position, cam.transform.forward) < 0) continue;
            if (Vector3.Distance(transform.position, go.transform.position) > maxGrappleDist)
            {
                Debug.DrawLine(transform.position, go.transform.position, Color.blue);
                continue;
            }
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

    private void updateGrappleDirection()
    {
        modGrapplePoint = TargetGrapplePoint;
        return;

        float moveAmount = 1f;
        float dist = Vector3.Distance(transform.position, modGrapplePoint);

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 1, modGrapplePoint - transform.position, out hit, dist))
        {
            Vector3 stepDir = Vector3.Project(transform.position - modGrapplePoint, up).normalized;
            modGrapplePoint += stepDir * moveAmount;
        }
        else
        {
            modGrapplePoint = TargetGrapplePoint;
        }
    }

    public bool IsGrounded()
    {
        //dimensions of the sphere
        float sphereRadius = 0.5f;
        float castDist = rideheight * 1.1f; //adds a 10% margin

        RaycastHit rayHit;

        return Physics.SphereCast(transform.position, sphereRadius, gravity, out rayHit, castDist, GameplayManager.Instance.NotPlayerOrEnemyMask);

    }

    private bool canJump()
    {
        return IsGrounded() && currentJumpDelay < 0;
    }

    public void Jump()
    {
        if (canJump())
        {
            AudioManager.Instance.PlaySound(jump, jumpVolume, transform.position);
            doJump = true;
        }
    }

    private void movePlayer()
    {
        
        

        Vector3 camForward = Vector3.ProjectOnPlane(cam.transform.forward, up).normalized;
        Vector3 camRight = Vector3.Cross(up, camForward).normalized;

        Vector3 rotatedMoveDir = camForward * movementDir.z + camRight * movementDir.x;
        Debug.DrawLine(transform.position, transform.position + rotatedMoveDir, Color.green);
        Debug.DrawLine(transform.position, transform.position + rb.linearVelocity, Color.red);

        //Debug.DrawLine(transform.position, transform.position + this.rotatedMoveDir * 2, Color.green);
        //Debug.DrawLine(transform.position, transform.position + rotatedMoveDir * 2, Color.red);

        if (rotatedMoveDir != Vector3.zero)
        {
            //would new speed go above max speed
            Vector3 targetVel = rb.linearVelocity + (rotatedMoveDir * acceleration) / rb.mass * Time.fixedDeltaTime;

            if (true)//targetVel.magnitude <= currentMaxSpeed)
            {
                rb.AddForce(rotatedMoveDir * acceleration, ForceMode.Force);

                //turning assist
                if (IsGrounded())
                {
                    Vector3 turnForce = Vector3.zero;
                    if (Vector3.Dot(rotatedMoveDir, rb.linearVelocity) < -0.9)
                    {
                        turnForce = -rb.linearVelocity * deceleration;
                    }
                    else
                    {
                        Vector3 forwardVel = Vector3.Project(rb.linearVelocity, rotatedMoveDir);
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
            rb.AddForce(-rb.linearVelocity * deceleration, ForceMode.Force);
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
            rb.AddForce(up * jumpForce, ForceMode.Impulse);
            doJump = false;
            doRideForce = false;
            currentJumpDelay = jumpDelay;
        }

        //glide
        if (glide && Vector3.Dot(rb.linearVelocity, gravity) >= 0) //check if moving down
        {
            Debug.Log("PlayerGlide");
            rb.AddForce(-gravity * gravityMod * glideStrength, ForceMode.Force);
        }

        //grapple
        if (grapple)
        {
            float currentSpeed = rb.linearVelocity.magnitude;
            float dot = Vector3.Dot(rb.linearVelocity, modGrapplePoint - transform.position);

            rb.linearVelocity = Vector3.zero;

            float grappleSpeed = (dot > 0.5) ? Mathf.Max(currentSpeed, grappleStrength) : grappleStrength;

            Vector3 grappleForce = (modGrapplePoint - transform.position).normalized * grappleSpeed;
            rb.AddForce(grappleForce, ForceMode.Acceleration);
        }

        //crash
        if (crash)
        {
            rb.linearVelocity = Vector3.zero;
            Vector3 crashForce = Vector3.Cross(up, -cam.transform.right).normalized * crashSpeed;
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
            transform.rotation = Quaternion.LookRotation(TargetGrapplePoint - transform.position, up);
        }
        if (crash)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(up, -cam.transform.right), up);
        }

        Vector3 moveDir = (rb.linearVelocity.magnitude > ROTATION_THRESHOLD) ? rb.linearVelocity : transform.forward;

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
        if (hpChange < 0f)
        {
            AudioManager.Instance.PlaySound(damage, damageVolume, transform.position);
        }
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

    private void onDeath()
    {
        if (GameplayManager.Instance.InBossStage)
        {
            GameplayManager.Instance.DeathScreen();
        }
        else
        {
            currentHP = maxHP;
            Vector3 knockBackForce = -GameplayManager.Instance.GetForward(transform.position).normalized * knockBackStrength + -gravity * knockBackHeight;
            rb.AddForce(knockBackForce, ForceMode.VelocityChange);
        }

    }

    private void OnCollisionStay(Collision collision)
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

    public void ResetPlayer()
    {
        this.transform.position = new Vector3(-3, -23, 3);
        this.transform.rotation = Quaternion.identity;
        currentWebs = startWebs;
        currentHP = maxHP;
        movementDir = Vector3.zero;
        doJump = false;
        TargetGrapplePoint = Vector3.zero;
        modGrapplePoint = Vector3.zero;
        ValidGrapplePoint = false;
        currentJumpDelay = 0;
        webGrappleObject.SetActive(false);
    }
}
