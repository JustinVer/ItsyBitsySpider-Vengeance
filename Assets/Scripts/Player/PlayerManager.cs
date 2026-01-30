using UnityEngine;
public class PlayerManager : MonoBehaviour
{
    private PlayerBody body;

    private void Start()
    {
        body = GetComponentInChildren<PlayerBody>();
    }
    void Update()
    {
        body.MovementDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        if (Input.GetAxis("Jump") > 0)
        {
            body.Jump();
        }
        if (Input.GetAxis("Glide") > 0)
        {
            body.Glide = true;
        }
        else
        {
            body.Glide = false;
        }

    }

    public Vector3 LinearVelocity()
    {
        return body.LinearVelocity();
    }
}
