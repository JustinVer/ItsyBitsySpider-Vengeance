using Unity.Mathematics;
using UnityEngine;

public class LegManager : MonoBehaviour
{
    [SerializeField] private GameObject leg;
    [SerializeField] private Transform[] rightLegPositions;
    [SerializeField] private Transform[] leftLegPositions;

    [SerializeField] private float stepDistance = 0.2f;

    private Leg[] rightLegs;
    private Leg[] leftLegs;

   

    private void Awake()
    {
        rightLegs = new Leg[rightLegPositions.Length];
        leftLegs = new Leg[leftLegPositions.Length];

        for (int i = 0; i < Mathf.Max(rightLegPositions.Length, leftLegPositions.Length); i++)
        {
            if (i < rightLegPositions.Length)
            {
                rightLegs[i] = Instantiate(leg, transform).GetComponent<Leg>();
                rightLegs[i].transform.localPosition = rightLegPositions[i].transform.localPosition;
                rightLegs[i].transform.localRotation = rightLegPositions[i].transform.localRotation;
            }
            if (i < leftLegPositions.Length)
            {
                leftLegs[i] = Instantiate(leg, transform).GetComponent<Leg>();
                leftLegs[i].transform.localPosition = leftLegPositions[i].transform.localPosition;
                leftLegs[i].transform.localRotation = leftLegPositions[i].transform.localRotation;
            }
        }
    }

    private void Update()
    {
       
    }
}
