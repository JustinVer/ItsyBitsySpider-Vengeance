using UnityEngine;

public class LegManager : MonoBehaviour
{
    [SerializeField] private GameObject leg;
    [SerializeField] private Transform[] leftLegPositions;
    [SerializeField] private Transform[] rightLegPositions;

    [SerializeField] private float rideheight = 1.75f;


    [SerializeField] private float stepDistance = 1;
    [SerializeField] private float stepHeight = 0.25f;
    [SerializeField] private float stepDuraion = 0.15f;

    [SerializeField] private float stepOffsetDuration = 0.05f;
    private float stepOffsetTimer = 0;
    private bool rightStep = true;

    private Leg[] rightLegs;
    private Leg[] leftLegs;

    private int rStepIdx = 0;
    private int lStepIdx = 0;

    private bool wasMoving = false;

    private bool grounded = true;
    private Vector3 linearVelocity;
    private Vector3 lastPosition;

    private void Awake()
    {

        lastPosition = transform.position;

        rightLegs = new Leg[rightLegPositions.Length];
        leftLegs = new Leg[leftLegPositions.Length];

        for (int i = 0; i < Mathf.Max(rightLegPositions.Length, leftLegPositions.Length); i++)
        {
            if (i < rightLegPositions.Length)
            {
                rightLegs[i] = Instantiate(leg, transform).GetComponent<Leg>();
                rightLegs[i].transform.localPosition = rightLegPositions[i].transform.localPosition;
                rightLegs[i].transform.localRotation = rightLegPositions[i].transform.localRotation;
                rightLegs[i].stepDistance = stepDistance;
                rightLegs[i].stepHeight = stepHeight;
                rightLegs[i].stepDuraion = stepDuraion;
            }
            if (i < leftLegPositions.Length)
            {
                leftLegs[i] = Instantiate(leg, transform).GetComponent<Leg>();
                leftLegs[i].transform.localPosition = leftLegPositions[i].transform.localPosition;
                leftLegs[i].transform.localRotation = leftLegPositions[i].transform.localRotation;
                leftLegs[i].stepDistance = stepDistance;
                leftLegs[i].stepHeight = stepHeight;
                leftLegs[i].stepDuraion = stepDuraion;
            }
        }
    }

    private void FixedUpdate()
    {

        linearVelocity = (transform.position - lastPosition) / Time.fixedDeltaTime;

        Vector3 down = GameplayManager.Instance.GetGravity(transform.position);
        RaycastHit hit;
        grounded = Physics.Raycast(transform.position, down, out hit, rideheight, GameplayManager.Instance.NotPlayerOrEnemyMask);


        foreach (Leg l in leftLegs)
        {
            l.BodyVelocity = linearVelocity;
        }
        foreach (Leg l in rightLegs)
        {
            l.BodyVelocity = linearVelocity;
        }

        Vector3 planarVel = Vector3.ProjectOnPlane(linearVelocity, down);

        bool moving = planarVel.sqrMagnitude > 0.25f;


        if (!moving && wasMoving)
        {
            foreach (Leg l in leftLegs)
            {
                l.Step();
            }
            foreach (Leg l in rightLegs)
            {
                l.Step();
            }

        }

        wasMoving = moving;

        stepOffsetTimer += Time.fixedDeltaTime;

        if (stepOffsetTimer >= stepOffsetDuration && grounded)
        {
            Vector3 footPosition;
            Vector3 targetPosition;
            if (rightStep)
            {
                footPosition = rightLegs[rStepIdx].FootPosition;
                targetPosition = rightLegs[rStepIdx].TargetPosition;

                Vector3 predictedTarget = targetPosition + linearVelocity * stepDuraion;

                if (Vector3.Distance(footPosition, predictedTarget) > stepDistance)
                {
                    rightLegs[rStepIdx].Step();
                    rStepIdx = (rStepIdx + 1) % rightLegs.Length;
                    stepOffsetTimer = 0;
                    rightStep = false;
                }




            }
            else
            {
                footPosition = leftLegs[lStepIdx].FootPosition;
                targetPosition = leftLegs[lStepIdx].TargetPosition;

                Vector3 predictedTarget = targetPosition + linearVelocity * stepDuraion;

                if (Vector3.Distance(footPosition, predictedTarget) > stepDistance)
                {
                    leftLegs[lStepIdx].Step();
                    lStepIdx = (lStepIdx + 1) % leftLegs.Length;
                    stepOffsetTimer = 0;
                    rightStep = true;

                }

            }
        }

        lastPosition = transform.position;

    }

    public void pauseLegs(bool enabled)
    {

    }
}
