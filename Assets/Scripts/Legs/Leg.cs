using UnityEngine;

public class Leg : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Transform knee;
    [SerializeField] private GameObject foot;

    [SerializeField] private float legLength;
    [HideInInspector] public float stepDistance;
    [HideInInspector] public float stepHeight;
    [HideInInspector] public float stepDuraion;

    private bool stepping = false;
    private float stepTimer = 0f;

    private Vector3 stepStart = Vector3.zero;
    private Vector3 stepEnd = Vector3.zero;

    private bool footPlanted = true;
    private Vector3 plantPosition = Vector3.zero;

    private Vector3 footPosition = Vector3.zero;
    public Vector3 FootPosition
    {
        get { return footPosition; }
        set { footPosition = value; }
    }

    private Vector3 targetPosition = Vector3.zero;
    public Vector3 TargetPosition
    {
        get { return targetPosition; }
        set { targetPosition = value; }
    }

    private Vector3 bodyVelocity = Vector3.zero;
    public Vector3 BodyVelocity
    {
        get { return bodyVelocity; }
        set { bodyVelocity = value; }
    }

    private Transform animFoot;
    public Transform AnimFoot
    {
        set { animFoot = value; }
    }

    private LegManager legManager;

    private void Start()
    {
        legManager = GetComponentInParent<LegManager>();
    }


    private void FixedUpdate()
    {
        if (legManager.Animating)
        {
            if (foot && animFoot)
            {
                foot.transform.position = animFoot.position;
            }
            return;
        }

        Vector3 down = GameplayManager.Instance.GetGravity(knee.position).normalized;

        Debug.DrawRay(knee.position, down * legLength, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(knee.position, down, out hit, legLength))
        {
            targetPosition = hit.point;

            if (!footPlanted && !stepping)
            {
                plantPosition = hit.point;
                footPlanted = true;
            }
        }
        else
        {
            targetPosition = knee.position + down * legLength;
            footPlanted = false;
        }


        if (stepping)
        {
            Vector3 predictedTarget = targetPosition + BodyVelocity * stepDuraion;
            stepEnd = Vector3.Lerp(targetPosition, predictedTarget, 0.5f);

            stepTimer += Time.fixedDeltaTime;
            float t = stepTimer / stepDuraion;

            t = Mathf.Clamp01(t);

            Vector3 pos = Vector3.Lerp(stepStart, stepEnd, t);

            float height = Mathf.Sin(t * Mathf.PI) * stepHeight;
            pos += -down * height;

            foot.transform.position = pos;

            if (t >= 1)
            {
                stepping = false;
                footPlanted = true;
                plantPosition = stepEnd;
            }


        }
        else if (footPlanted)
        {
            foot.transform.position = plantPosition;
        }
        else
        {
            foot.transform.position = targetPosition;
        }





        footPosition = foot.transform.position;
    }



    public void Step()
    {


        if (stepping) return;

        stepping = true;
        footPlanted = false;

        stepTimer = 0f;
        stepStart = foot.transform.position;
        stepEnd = targetPosition;
    }
}
