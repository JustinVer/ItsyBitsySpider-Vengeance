using UnityEngine;

public class BodyFollowAgent : MonoBehaviour
{
    [SerializeField] private Transform followBody;
    [SerializeField] private float speed = 2f;
    [SerializeField] private Rigidbody rb;

    private void FixedUpdate()
    {
        rb.MovePosition(Vector3.MoveTowards(this.transform.position, followBody.position, speed * Time.fixedDeltaTime));
    }
}
