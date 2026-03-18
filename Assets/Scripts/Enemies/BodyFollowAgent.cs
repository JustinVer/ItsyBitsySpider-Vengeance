using UnityEngine;

public class BodyFollowAgent : MonoBehaviour
{
    [SerializeField] private AgentLinkMover followBody;
    [SerializeField] private float speed = 2f;
    [SerializeField] private Rigidbody rb;
    public Rigidbody RB { get { return rb; } }
    [SerializeField] private float distanceToAgentBeforeJump = 0.3f;
    [SerializeField] private float height = 0.7f;
    [SerializeField] private float maxDegreesRotation = 1f;
    private bool jumping = false;
    private bool waitingForJump = false;
    [SerializeField] private Animator anim;
    [SerializeField] private float downMultiplier = -0.1f;
    public Animator Anim { get { return anim; } }

    [SerializeField] private bool autoRotate = true;

    private void FixedUpdate()
    {
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        rb.MovePosition(Vector3.MoveTowards(this.transform.position, followBody.transform.position, speed * Time.fixedDeltaTime));
        //rb.MovePosition(this.transform.position + (GameplayManager.Instance.GetGravity(this.transform.position) * Time.fixedDeltaTime * downMultiplier));
        if (jumping)
        {
            rb.MovePosition(followBody.transform.position);

        }
        else if (waitingForJump && Vector3.Distance(this.transform.position, followBody.transform.position) < distanceToAgentBeforeJump)
        {
            jumping = true;
            waitingForJump = false;
            followBody.StartJump();
        }
        else if (autoRotate && Vector3.Distance(this.transform.position, followBody.transform.position) > 0.5f)
        {
            Rotate((followBody.transform.position - this.transform.position) + ((height / 2.0f) * GameplayManager.Instance.GetGravity(this.transform.position).normalized));
        }
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
    }

    public void StartJump()
    {
        waitingForJump = true;
    }

    public void EndJump()
    {
        jumping = false;
        waitingForJump = false;
        rb.MovePosition(followBody.transform.position);
    }

    public void setSpeed(float newSpeed)
    {
        this.speed = newSpeed;
    }

    public float getSpeed()
    {
        return speed;
    }

    private void Rotate(Vector3 direction)
    {
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, followBody.transform.rotation, maxDegreesRotation * Time.fixedDeltaTime);
    }
}
