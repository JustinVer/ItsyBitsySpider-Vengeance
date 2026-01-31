using Unity.VisualScripting;
using UnityEngine;

public class SegmentRandomizer : MonoBehaviour
{
    [SerializeField] int segmentsBeforeTurn;
    [SerializeField] SegmentNode[] area1;
    [SerializeField] SegmentNode[] area2;
    [SerializeField] SegmentNode[] area3;
    [SerializeField] GameObject[] segmentPool; //0 is reserved for the start area
    [SerializeField] GameObject corner;
    [SerializeField] int[] debugOrder1;
    [SerializeField] int[] debugOrder2;
    [SerializeField] int[] debugOrder3;

    private int lastLoaded = 0;

    private void Start()
    {
        area1 = new SegmentNode[debugOrder1.Length];
        area2 = new SegmentNode[debugOrder2.Length];
        area3 = new SegmentNode[debugOrder3.Length];

        //loading logic
        for (int i = 0; i < debugOrder1.Length; i++)
        {
            Debug.Log(i);
            area1[i] = new SegmentNode(segmentPool[debugOrder1[i]]);
        }

        area1[0].loadSection(Vector3.zero, Quaternion.identity);
        for(int i = 1; i < debugOrder1.Length; i++)
        {
            area1[i].loadSection(area1[i - 1].getEnd(), area1[i - 1].getRotation());
        }
    }
}
