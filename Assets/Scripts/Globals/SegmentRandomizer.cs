using UnityEngine;

public class SegmentRandomizer : MonoBehaviour
{
    [SerializeField] int segmentsBeforeTurn = 3;
    int segmentLoaded = 0;
    [SerializeField] Quaternion cornerAngle = Quaternion.Euler(0, 90, 0);
    [SerializeField] SegmentNode[] area1;
    [SerializeField] SegmentNode[] area2;
    [SerializeField] SegmentNode[] area3;
    [SerializeField] GameObject[] segmentPool; //0 is reserved for the start area
    [SerializeField] GameObject corner;
    SegmentNode cornerSegment1;
    SegmentNode cornerSegment2;
    int nextCorner = 1;
    [SerializeField] int[] debugOrder1;
    [SerializeField] int[] debugOrder2;
    [SerializeField] int[] debugOrder3;

    private int lastLoaded = 0;
    private int nextUnload;
    [SerializeField] private GameObject forwardTriggerPrefab;
    private LoadingTrigger forwardTrigger;
    [SerializeField] private GameObject backwardTriggerPrefab;
    private LoadingTrigger backwardTrigger;

    private void Start()
    {
        area1 = new SegmentNode[debugOrder1.Length];
        area2 = new SegmentNode[debugOrder2.Length];
        area3 = new SegmentNode[debugOrder3.Length];

        forwardTrigger = Instantiate(forwardTriggerPrefab).GetComponentInChildren<LoadingTrigger>();
        backwardTrigger = Instantiate(backwardTriggerPrefab).GetComponentInChildren<LoadingTrigger>();

        //loading logic
        for (int i = 0; i < debugOrder1.Length; i++)
        {
            area1[i] = new SegmentNode(segmentPool[debugOrder1[i]]);
        }

        area1[0].loadSection(Vector3.zero, Quaternion.identity);
        lastLoaded = 0;
        Debug.Log("loading start");
        cornerSegment1 = new SegmentNode(corner);
        cornerSegment2 = new SegmentNode(corner);

        loadForward();
    }
    public void loadForward()
    {
        int sectionsToTurn = 0;

        for (int i = lastLoaded + 1; i < debugOrder1.Length; i++)
        {
            if (sectionsToTurn >= segmentsBeforeTurn - 1)
            {
                if (nextCorner == 1)
                {
                    endLoad(cornerSegment1, i);
                }
                else if (nextCorner == 2)
                {
                    endLoad(cornerSegment2, i);
                }
                nextUnload = lastLoaded;
                lastLoaded = i;
                break;
            }
            else
            {
                area1[i].loadSection(area1[i - 1].getEnd(), area1[i - 1].getRotation());
                Debug.Log("loading straight");
                sectionsToTurn++;
            }
        }
    }
    public void loadBackward()
    {
        int sectionsToTurn = 0;

        for (int i = nextUnload - 1; i > -1; i++)
        {
            if (sectionsToTurn >= segmentsBeforeTurn - 1)
            {
                if (nextCorner == 1)
                {
                    endLoad(cornerSegment2, i);
                }
                else if (nextCorner == 2)
                {
                    endLoad(cornerSegment1, i);
                }
                lastLoaded = nextUnload;
                nextUnload = i;
                break;
            }
            else
            {
                area1[i].loadSection(area1[i - 1].getEnd(), area1[i - 1].getRotation());
                Debug.Log("loading straight");
                sectionsToTurn++;
            }
        }
    }
    private void endLoad(SegmentNode cornerToUse, int segmentIndex)
    {
        cornerToUse.loadSection(area1[segmentIndex - 1].getEnd(), area1[segmentIndex - 1].getRotation());
        area1[segmentIndex].loadSection(cornerToUse.getEnd(), cornerToUse.exitAngle());
        forwardTrigger.reposition(area1[segmentIndex - 1].getEnd(), area1[segmentIndex - 1].getRotation());
        unloadBack(nextUnload);
        nextCorner++;
        if (nextCorner > 2)
        {
            nextCorner = 1;
        }
    }
    private void unloadBack(int unloadBefore)
    {
        for (int i = unloadBefore - 1; i >= 0; i -= 1)
        {
            area1[i].unloadSection();
        }
    }
}
