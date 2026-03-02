using UnityEngine;
public class PlayerManager : MonoBehaviour
{
    private PlayerBody body;
    public bool InputEnabled = true;

    private void Start()
    {
        body = GetComponentInChildren<PlayerBody>();
    }
    void Update()
    {
        if (!InputEnabled)
        {
            body.MovementDir = Vector3.zero;
            return;
        }

        body.MovementDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        if (Input.GetButton("Jump"))
        {
            body.Jump();
        }
        if (Input.GetButton("Glide") && body.CurrentWebs > 0)
        {
            body.Glide = true;
            body.CurrentWebs -= Time.deltaTime;
        }
        else
        {
            body.Glide = false;
        }
        if (Input.GetButtonDown("Grapple") && body.CurrentWebs >= 1)
        {
            body.Grapple = !body.Grapple;
            body.CurrentWebs -= 1;
        }
        if (Input.GetButtonDown("Crash") && body.CurrentWebs >= 1)
        {
            body.Crash = !body.Crash;
            if (body.TargetGrapplePoint != Vector3.zero)
            {
                body.CurrentWebs -= 1;
            }
        }

    }

    

    public Vector3 LinearVelocity()
    {
        return body.LinearVelocity();
    }
}
