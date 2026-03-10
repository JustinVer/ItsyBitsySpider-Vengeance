using UnityEngine;
public class PlayerManager : MonoBehaviour
{
    private PlayerBody body;
    public bool InputEnabled = true;

    private bool canGrapple = true;
    private bool canDash = false;

    [SerializeField] public Camera Playercam;

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

        body.MovementDir = new Vector3(GameplayManager.Instance.MoveVector.x, 0, GameplayManager.Instance.MoveVector.y).normalized;
        if (GameplayManager.Instance.Jump)
        {
            body.Jump();
        }
        if (GameplayManager.Instance.Glide && body.CurrentWebs > 0)
        {
            body.Glide = true;
            body.CurrentWebs -= Time.deltaTime;
        }
        else
        {
            body.Glide = false;
        }
        if (GameplayManager.Instance.Grapple && canGrapple && body.CurrentWebs >= 1)
        {
            body.Grapple = !body.Grapple;
            if (body.ValidGrapplePoint && !body.Grapple)
            {
                body.CurrentWebs -= 1;
            }
            canGrapple = false;
        }
        if (GameplayManager.Instance.Dash && canDash && body.CurrentWebs >= 1)
        {
            body.Crash = !body.Crash;

            body.CurrentWebs -= 1;

            canDash = false;
        }

        if (!GameplayManager.Instance.Dash)
        {
            canDash = true;
        }
        if (!GameplayManager.Instance.Grapple)
        {
            canGrapple = true;
        }

    }



    public Vector3 LinearVelocity()
    {
        return body.LinearVelocity();
    }
}
