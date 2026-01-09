using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    private Rigidbody rb;

    private Vector3 movementDir = Vector3.zero;

    private bool doJump = false;

    private bool doRideForce = false;

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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        movePlayer();
    }

    public bool IsGrounded()
    {
        //dimensions of the sphere
        float sphereRadius = 0.5f;
        float castDist = rideheight * 1.1f; //adds a 10% margin

        RaycastHit rayHit;

        return Physics.SphereCast(transform.position, sphereRadius, GameplayManager.Instance.GetGravity(this.transform.position), out rayHit, castDist);

    }

    private bool canJump()
    {
        return IsGrounded() && jumpDelay < 0;
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

        Vector3 gravity = GameplayManager.Instance.GetGravity(this.transform.position);

        if (movementDir != Vector3.zero)
        {
            //would new speed go above max speed

            Vector3 targetVel = rb.linearVelocity + (MovementDir * acceleration) / rb.mass * Time.fixedDeltaTime;


            if (targetVel.magnitude <= maxSpeed)
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
            if (x < 0 && jumpDelay < 0)
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

        jumpDelay -= Time.fixedDeltaTime;
        if (doJump)
        {
            rb.AddForce(-gravity.normalized * jumpForce, ForceMode.Impulse);
            doJump = false;
            doRideForce = false;
            jumpDelay = 1;
        }


        //max velocity
        if (rb.linearVelocity.magnitude > velCap)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * velCap;
        }

    }


}
