using UnityEngine;

public class BodyFollowAgent : MonoBehaviour
{
    [SerializeField] private AgentLinkMover followBody;
    [SerializeField] private float speed = 2f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float distanceToAgentBeforeJump = 0.3f;
    [SerializeField] private float height = 0.7f;
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
        else if (Vector3.Distance(this.transform.position, followBody.transform.position) > 0.5f)
        {
            Rotate((this.transform.position - followBody.transform.position) + ((height / 2.0f) * GameplayManager.Instance.GetGravity(this.transform.position).normalized));
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
