using UnityEngine;
using UnityEngine.Rendering;

public class PlayerBody : MonoBehaviour
{
    private Rigidbody rb;

    private Vector3 movementDir = Vector3.zero;


    public Vector3 MovementDir
    {
        get { return movementDir; }
        set { movementDir = value; }
    }


    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 5f;
    [SerializeField] private float maxSpeed = 10f;

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

    private void movePlayer()
    {
        
        Vector3 gravity = GameplayManager.Instance.GetGravity(gameObject);

        //WASD movement
        if (movementDir != Vector3.zero)
        {
            rb.AddForce(MovementDir * acceleration, ForceMode.Force);
        }
        else
        {
            rb.AddForce(rb.linearVelocity * -deceleration, ForceMode.Force);
        }
        //max speed
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        //gravity
        rb.AddForce(gravity * gravityMod, ForceMode.Acceleration);

        RaycastHit rayHit;
        if (Physics.Raycast(transform.position, gravity, out rayHit, maxRayDist))
        {
            
            float x = rayHit.distance - rideheight;

            float springForce = (x * rideSpringStrength);

            Vector3 springVector = gravity.normalized * springForce;

            rb.AddForce(springVector, ForceMode.Acceleration);
         
        }


    }
}
