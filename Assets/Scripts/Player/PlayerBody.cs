using UnityEngine;
using UnityEngine.Rendering;

public class PlayerBody : MonoBehaviour
{
    private Rigidbody rb;

    private Vector3 movementDir = Vector3.zero;

    private bool doJump = false;

    public Vector3 MovementDir
    {
        get { return movementDir; }
        set { movementDir = value; }
    }


    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 5f;
    [SerializeField] private float maxSpeed = 10f;

    [SerializeField] private float turnFactor = 0.5f;

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

        return Physics.SphereCast(transform.position, sphereRadius, GameplayManager.Instance.GetGravity(gameObject), out rayHit, castDist);

    }

    private bool canJump()
    {
        return IsGrounded();
    }

    public void Jump()
    {
        Debug.Log(IsGrounded());
        if (canJump())
        {
            doJump = true;
        }
    }

    private void movePlayer()
    {
        
        Vector3 gravity = GameplayManager.Instance.GetGravity(gameObject);

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
                    Vector3 forwardVel = Vector3.Project(rb.linearVelocity, movementDir);
                    Vector3 turnForce = -(rb.linearVelocity - forwardVel) * deceleration;
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

            rb.AddForce(springVector, ForceMode.Acceleration);
         
        }

        //gravity
        rb.AddForce(gravity * gravityMod, ForceMode.Acceleration);

        if (doJump)
        {
            rb.AddForce(-gravity.normalized * jumpForce, ForceMode.Impulse);
            doJump = false;
        }


        //max velocity
        if (rb.linearVelocity.magnitude > velCap)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * velCap;
        }

    }

    
}
