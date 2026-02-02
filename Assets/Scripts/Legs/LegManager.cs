using Unity.Mathematics;
using UnityEngine;

public class LegManager : MonoBehaviour
{
    [SerializeField] private GameObject leg;
    [SerializeField] private Transform[] rightLegPositions;
    [SerializeField] private Transform[] leftLegPositions;

    [SerializeField] private float stepDistance = 1;
    [SerializeField] private float stepHeight = 0.25f;
    [SerializeField] private float stepDuraion = 0.5f;

    [SerializeField] private float stepOffsetDuration = 0.1f;
    private float stepOffsetTimer = 0;
    private bool rightStep = true;

    private Leg[] rightLegs;
    private Leg[] leftLegs;

    private int rStepIdx = 0;
    private int lStepIdx = 0;

    private Rigidbody body;
    private PlayerBody player;
    private bool wasMoving = false;

    private void Awake()
    {
        body = GetComponentInParent<Rigidbody>();
        player = GetComponentInParent<PlayerBody>();
        if (body == null) Debug.Log("Rigidbody not found");

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

        body = GetComponentInParent<Rigidbody>();
        player = GetComponentInParent<PlayerBody>();
        if (body == null) Debug.Log("Rigidbody not found");


    }

    private void FixedUpdate()
    {
        foreach (Leg l in leftLegs)
        {
            l.BodyVelocity = body.linearVelocity;
        }
        foreach (Leg l in rightLegs)
        {
            l.BodyVelocity = body.linearVelocity;
        }

        bool moving = body.linearVelocity.magnitude > 0.1f;
        

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

        if (stepOffsetTimer >= stepOffsetDuration && player.IsGrounded())
        {
            Vector3 footPosition;
            Vector3 targetPosition;
            if (rightStep)
            {
                footPosition = rightLegs[rStepIdx].FootPosition;
                targetPosition = rightLegs[rStepIdx].TargetPosition;

                Vector3 predictedTarget =
     targetPosition + body.linearVelocity * stepDuraion;

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

                Vector3 predictedTarget =
    targetPosition + body.linearVelocity * stepDuraion;

                if (Vector3.Distance(footPosition, predictedTarget) > stepDistance)
                {
                    leftLegs[lStepIdx].Step();
                    lStepIdx = (lStepIdx + 1) % leftLegs.Length;
                    stepOffsetTimer = 0;
                    rightStep = true;

                }
                
            }
        }
       
    }
}
