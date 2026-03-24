using UnityEngine;
public class PlayerManager : MonoBehaviour
{
    private PlayerBody body;
    public bool InputEnabled = true;

    private bool canGrapple = true;
    private bool canDash = false;

    [SerializeField] public Camera Playercam;

    [SerializeField] private AudioClip dash;
    [SerializeField, Range(0, 1)] private float dashVolume = 0.5f;
    [SerializeField] private AudioClip grapple;
    [SerializeField, Range(0, 1)] private float grappleVolume = 0.5f;
    [SerializeField] private AudioClip glide;
    [SerializeField, Range(0, 1)] private float glideVolume = 0.5f;

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
            if (!body.Glide)
            {
                AudioManager.Instance.PlaySound(glide, glideVolume, body.transform.position);
            }
            body.Glide = true;
            body.CurrentWebs -= Time.deltaTime;
        }
        else
        {
            body.Glide = false;
        }
        if (GameplayManager.Instance.Grapple && canGrapple && body.CurrentWebs >= 1)
        {
            if (!body.Grapple)
            {
                AudioManager.Instance.PlaySound(grapple, grappleVolume, body.transform.position);
            }
            body.Grapple = !body.Grapple;
            if (body.ValidGrapplePoint && !body.Grapple)
            {
                body.CurrentWebs -= 1;
            }
            canGrapple = false;
        }
        if (GameplayManager.Instance.Dash && canDash && body.CurrentWebs >= 1)
        {
            if (!body.Crash)
            {
                AudioManager.Instance.PlaySound(dash, dashVolume, body.transform.position);
            }

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
