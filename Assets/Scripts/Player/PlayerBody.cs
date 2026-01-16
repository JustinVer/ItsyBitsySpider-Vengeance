using Unity.Mathematics;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    private Rigidbody rb;

    private Vector3 movementDir = Vector3.zero;
    private Vector3 gravity = Vector3.zero;

    private bool doJump = false;

    private bool doRideForce = false;

    private float currentMaxSpeed;

    private float currentJumpDelay = 0;
    private float jumpDelay = 1;
    public Vector3 MovementDir
    {
        get { return movementDir; }
        set { movementDir = value; }
    }


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

    [SerializeField] private Camera cam;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        gravity = GameplayManager.Instance.GetGravity(transform.position);
        movePlayer();
        rotateBody();
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
        movementDir = Quaternion.LookRotation(cam.transform.forward, -gravity) * movementDir;


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

        Quaternion targetRot = Quaternion.LookRotation(rb.linearVelocity, -gravity);
        Debug.DrawLine(transform.position, transform.position + transform.forward * 2);
        Vector3 sideAxis = Vector3.Cross(transform.forward, -gravity);
        Debug.DrawLine(transform.position, transform.position + sideAxis.normalized * 2);
        Vector3 forwardAxis = Vector3.Cross(-gravity, sideAxis);
        Debug.DrawLine(transform.position, transform.position + forwardAxis.normalized * 2);

        float pitch = Vector3.SignedAngle(targetRot * Vector3.forward, forwardAxis, sideAxis);

        float max = 45;
        float min = -45;
        quaternion old = targetRot;
        if (pitch > max)
        {
            targetRot *= Quaternion.AngleAxis(max - pitch, sideAxis);
        }
        else
        if (pitch < min)
        {
            targetRot *= Quaternion.AngleAxis(min - pitch, sideAxis);
        }

        {
            //Debug.Log(old + " -> " + targetRot);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

    }

    public void ApplyForce(Vector3 force, ForceMode forceMode)
    {
        rb.AddForce(force, forceMode);
    }

    public void Slow(float slowPercent)
    {
        currentMaxSpeed = maxSpeed * slowPercent;
    }

}
