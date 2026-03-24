using UnityEngine;

public class Leg : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private GameObject hint;
    [SerializeField] private GameObject foot;
    [SerializeField] private GameObject raycastObject;

    [SerializeField] private float legLength;

    [HideInInspector] public float stepDistance;
    [HideInInspector] public float stepHeight;
    [HideInInspector] public float stepDuraion;
    [HideInInspector] public int stepGroup = -1;

    [SerializeField] private AudioClip step;
    [SerializeField, Range(0, 1)] private float stepVolume = 0.25f;

    public bool stepping = false;
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

        Vector3 raycastPoint = raycastObject.transform.position;

        Vector3 down = GameplayManager.Instance.GetGravity(raycastPoint).normalized;

        Debug.DrawRay(raycastPoint, down * legLength, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(raycastPoint, down, out hit, legLength))
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
            targetPosition = raycastPoint + down * legLength;
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
                AudioManager.Instance.PlaySound(step, stepVolume, stepEnd);
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


        Vector3 footPos = foot.transform.position;

        float rootHeight = Vector3.Dot(root.transform.position, down);
        float footHeight = Vector3.Dot(footPos, down);

        Vector3 hintPos = footPos + down * (rootHeight - footHeight);


        hint.transform.position = hintPos;//


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
