using Unity.VisualScripting;
using UnityEngine;

public class SegmentRandomizer : MonoBehaviour
{
    [SerializeField] int segmentsBeforeTurn = 3;
    [SerializeField] Quaternion cornerAngle = Quaternion.Euler(0, 90, 0);
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
        int sectionsToTurn = 0;
        for (int i = 0; i < debugOrder1.Length; i++)
        {
            area1[i] = new SegmentNode(segmentPool[debugOrder1[i]]);
        }

        area1[0].loadSection(Vector3.zero, Quaternion.identity);
        Debug.Log("loading start");
        SegmentNode cornerSegment = new SegmentNode(corner);
        for(int i = 1; i < debugOrder1.Length; i++)
        {
            if (sectionsToTurn >= segmentsBeforeTurn)
            {
                cornerSegment.loadSection(area1[i - 1].getEnd(), area1[i - 1].getRotation());
                Debug.Log("loading corner");
                area1[i].loadSection(cornerSegment.getEnd(), Quaternion.Inverse(cornerSegment.getRotation() * cornerAngle));
                Debug.Log("loading section after corner");
                sectionsToTurn = 1;
            }
            else
            {
                area1[i].loadSection(area1[i - 1].getEnd(), area1[i - 1].getRotation());
                Debug.Log("loading straight");
                sectionsToTurn++;
            }
        }
    }
}
