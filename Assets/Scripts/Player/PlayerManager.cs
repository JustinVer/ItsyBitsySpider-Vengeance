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
        if (Input.GetButton("Glide"))
        {
            body.Glide = true;
        }
        else
        {
            body.Glide = false;
        }
        if (Input.GetButtonDown("Grapple"))
        {
            body.Grapple = !body.Grapple;
        }
        if (Input.GetButtonDown("Crash"))
        {
            body.Crash = !body.Crash;
        }

    }

    

    public Vector3 LinearVelocity()
    {
        return body.LinearVelocity();
    }
}
