using UnityEngine;

public class BodyFollowAgent : MonoBehaviour
{
    [SerializeField] private ManualFolloeNavMeshPath followBody;
    [SerializeField] private float speed = 2f;
    [SerializeField] private Rigidbody rb;
    private bool jumping = false;
    private bool waitingForJump = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float timeToTarget;
    private float startJumpTime = 0f;

    private void FixedUpdate()
    {
        if (waitingForJump)
        {
            if (Vector3.Distance(this.transform.position, startPosition) < 1f)
            {
                Debug.Log("start jump");
                waitingForJump = false;
                jumping = true;
                rb.MovePosition(startPosition);
                rb.linearVelocity = CalculateVelocityForTime(startPosition, endPosition, 5f, 0.1f, GameplayManager.Instance.GetGravity(this.transform.position));
                startJumpTime = Time.time;
            }
            else
            {
                Debug.Log("waiting for jump");
                rb.MovePosition(Vector3.MoveTowards(this.transform.position, startPosition, speed * Time.fixedDeltaTime));
            }

        }
        else if (jumping)
        {
            if (Vector3.Distance(this.transform.position, endPosition) < 1f || timeToTarget + 0.5f <= Time.time - startJumpTime)
            {
                Debug.Log("end jump");
                jumping = false;
                rb.linearVelocity = Vector3.zero;
                rb.MovePosition(endPosition);
                followBody.endJump();
            }
            else
            {
                rb.AddForce(GameplayManager.Instance.GetGravity(this.transform.position), ForceMode.Acceleration);
            }
        }
        else
        {
            rb.MovePosition(Vector3.MoveTowards(this.transform.position, followBody.gameObject.transform.position, speed * Time.fixedDeltaTime));
        }
    }

    public void Jump(Vector3 startPosition, Vector3 endPosition)
    {
        Debug.Log("Jump: " + startPosition + " " + endPosition);
        this.startPosition = startPosition;
        this.endPosition = endPosition - (GameplayManager.Instance.GetGravity(this.transform.position) * 0.08f);
        waitingForJump = true;
    }

    public Vector3 CalculateVelocityForTime(Vector3 startPoint, Vector3 targetPoint, float timeMultiplier, float timeBase, Vector3 gravity)
    {
        Vector3 distance = targetPoint - startPoint;
        Vector3 distanceXZ = new Vector3(distance.x, 0, distance.z);

        timeToTarget = (distanceXZ.magnitude / timeMultiplier) + timeBase;
        float vxz = distanceXZ.magnitude / timeToTarget;
        float vy = (distance.y + 0.5f * Mathf.Abs(gravity.y) * timeToTarget * timeToTarget) / timeToTarget;

        Vector3 result = distanceXZ.normalized * vxz;
        result.y = vy;

        return result;
    }
}
