using UnityEngine;

public class SegmentRandomizer : MonoBehaviour
{
    [SerializeField] int segmentsPerLevel = 5;
    [SerializeField] int segmentsBeforeTurn;
    [SerializeField] SegmentNode[] area1;
    [SerializeField] SegmentNode[] area2;
    [SerializeField] SegmentNode[] area3;
    [SerializeField] GameObject[] segmentPool;
    [SerializeField] string debugOrder1 = "";
    [SerializeField] string debugOrder2 = "";
    [SerializeField] string debugOrder3 = "";

    private void Start()
    {
        area1 = new SegmentNode[segmentsPerLevel];
        area2 = new SegmentNode[segmentsPerLevel];
        area3 = new SegmentNode[segmentsPerLevel];

        if (debugOrder1.Length == segmentsPerLevel)
        {
            for (int i = 0; i < segmentsPerLevel; i++)
            {
                area1[i] = new SegmentNode(debugOrder1[i]);
                area2[i] = new SegmentNode(debugOrder2[i]);
                area3[i] = new SegmentNode(debugOrder3[i]);
            }
        }

        //loading logic
    }
}
