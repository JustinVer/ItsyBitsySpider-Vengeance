using UnityEngine;

public class BodyFollowAgent : MonoBehaviour
{
    [SerializeField] private AgentLinkMover followBody;
    [SerializeField] private float speed = 2f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float distanceToAgentBeforeJump = 0.3f;
    private bool jumping = false;
    private bool waitingForJump = false;

    private void FixedUpdate()
    {

        rb.MovePosition(Vector3.MoveTowards(this.transform.position, followBody.transform.position, speed * Time.fixedDeltaTime));
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
        else
        {
            Rotate(this.transform.position - followBody.transform.position);
        }
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

    private void Rotate(Vector3 direction)
    {
        this.transform.rotation = Quaternion.LookRotation(direction, GameplayManager.Instance.GetGravity(this.transform.position) * -1);
    }
}
