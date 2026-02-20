using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SegmentRandomizer : MonoBehaviour
{
    int segmentLoaded = 0;
    [SerializeField] SegmentNode[] area;
    [SerializeField] GameObject[] segmentPool; //0 is reserved for the start area
    // stuff to know for debug orders
    // 0 is empty straight segment
    // 1 is corner segment
    // actual levels are any larger numbers
    [SerializeField] int[] debugOrder;
    SplineContainer container;
    Spline gravitySpline;

    private int lastLoaded = 0;
    private int nextUnload;
    private int pipeLength = 44;
    private int cornerLength = 28;

    [SerializeField] private GameObject forwardTriggerPrefab;
    private LoadingTrigger forwardTrigger;
    [SerializeField] private GameObject backwardTriggerPrefab;
    private LoadingTrigger backwardTrigger;

    private void Start()
    {
        area = new SegmentNode[debugOrder.Length];
        buildSpline();

        forwardTrigger = Instantiate(forwardTriggerPrefab).GetComponentInChildren<LoadingTrigger>();
        backwardTrigger = Instantiate(backwardTriggerPrefab).GetComponentInChildren<LoadingTrigger>();

        //loading logic
        for (int i = 0; i < debugOrder.Length; i++)
        {
            area[i] = new SegmentNode(segmentPool[debugOrder[i]]);
        }

        area[0].loadSection(Vector3.zero, Quaternion.identity);
        lastLoaded = 0;
        Debug.Log("loading start");
        cornerSegment1 = new SegmentNode(corner);
        cornerSegment2 = new SegmentNode(corner);

        loadForward();
    }
    public void loadForward()
    {
        int sectionsToTurn = 0;

        for (int i = lastLoaded + 1; i < debugOrder.Length; i++)
        {
            if (sectionsToTurn >= segmentsBeforeTurn - 1)
            {
                if (nextCorner == 1)
                {
                    endLoadFront(cornerSegment1, i);
                }
                else if (nextCorner == 2)
                {
                    endLoadFront(cornerSegment2, i);
                }
                nextUnload = lastLoaded;
                lastLoaded = i;
                break;
            }
            else
            {
                area[i].loadSection(area[i - 1].getEnd(), area[i - 1].getRotation());
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
                    endLoadBack(cornerSegment2, i);
                }
                else if (nextCorner == 2)
                {
                    endLoadBack(cornerSegment1, i);
                }
                lastLoaded = nextUnload;
                nextUnload = i;
                break;
            }
            else
            {
                area[i].loadSection(area[i - 1].getEnd(), area[i - 1].getRotation());
                Debug.Log("loading straight");
                sectionsToTurn++;
            }
        }
    }
    private void endLoadFront(SegmentNode cornerToUse, int segmentIndex)
    {
        cornerToUse.loadSection(area[segmentIndex - 1].getEnd(), area[segmentIndex - 1].getRotation());
        area[segmentIndex].loadSection(cornerToUse.getEnd(), cornerToUse.exitAngle());
        forwardTrigger.reposition(area[segmentIndex - 1].getEnd(), area[segmentIndex - 1].getRotation());
        unloadBack(nextUnload);
        nextCorner++;
        if (nextCorner > 2)
        {
            nextCorner = 1;
        }
    }
    private void endLoadBack(SegmentNode cornerToUse, int segmentIndex)
    {
        cornerToUse.loadSection(area[segmentIndex - 1].getBeginning(), area[segmentIndex - 1].getRotation());
        area[segmentIndex].loadSection(cornerToUse.getBeginning(), cornerToUse.entryAngle());
        forwardTrigger.reposition(area[segmentIndex + segmentsBeforeTurn - 1].getEnd(), area[segmentIndex + segmentsBeforeTurn - 1].getRotation());
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
            area[i].unloadSection();
        }
    }
    private void unloadForward(int unloadAfter)
    {
        for (int i = unloadAfter - 1; i <= area.Length; i++)
        {
            area[i].unloadSection();
        }
    }

    private void buildSpline()
    {
        container = gameObject.AddComponent<SplineContainer>();
        gravitySpline = container.AddSpline();

        BezierKnot[] knots = new BezierKnot[]
        {
            new BezierKnot(new float3(0f, 0f, 0f)),   // Start point
            new BezierKnot(new float3(5f, 2f, 0f)),   // Middle point
            new BezierKnot(new float3(10f, 0f, 0f))  // End point
        };
    }
}
