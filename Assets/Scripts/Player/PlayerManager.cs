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

        Debug.Log("Player movement raw direction: " + new Vector3(GameplayManager.Instance.MoveVector.x, 0, GameplayManager.Instance.MoveVector.y) + " " + new Vector3(GameplayManager.Instance.MoveVector.x, 0, GameplayManager.Instance.MoveVector.y).normalized);
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
        if (GameplayManager.Instance.Grapple && body.CurrentWebs >= 1)
        {
            body.Grapple = !body.Grapple;
            body.CurrentWebs -= 1;
        }
        if (GameplayManager.Instance.Dash && body.CurrentWebs >= 1)
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
