using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

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
    private int cornerDir = -1;

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

        loadForward();
    }
    public void loadForward()
    {
        for (int i = lastLoaded + 1; i < debugOrder.Length; i++)
        {
            area[i].loadSection(area[i - 1].getEnd(), area[i - 1].exitAngle());
            if (debugOrder[i-1] == 1)
            {
                nextUnload = lastLoaded-2;
                lastLoaded = i;
                unloadBack(nextUnload);

                forwardTrigger.reposition(area[lastLoaded - 2].getEnd(), area[lastLoaded - 2].exitAngle());
                backwardTrigger.reposition(area[nextUnload + 2].getBeginning(), area[nextUnload + 2].entryAngle());

                break;
            }
        }
    }
    public void loadBackward()
    {
        for (int i = lastLoaded + 1; i < debugOrder.Length; i++)
        {
            area[i].loadSection(area[i - 1].getEnd(), area[i - 1].entryAngle());
            if (debugOrder[i+1] == 1)
            {
                lastLoaded = nextUnload+2;
                nextUnload = i;
                unloadForward(lastLoaded);

                forwardTrigger.reposition(area[nextUnload - 2].getEnd(), area[lastLoaded - 2].exitAngle());
                backwardTrigger.reposition(area[nextUnload + 2].getBeginning(), area[nextUnload + 2].entryAngle());

                break;
            }
        }
    }
    private void unloadBack(int unloadBefore)
    {
        for (int i = unloadBefore-1; i >= 0; i -= 1)
        {
            area[i].unloadSection();
        }
    }
    private void unloadForward(int unloadAfter)
    {
        for (int i = unloadAfter+1; i <= area.Length; i += 1)
        {
            area[i].unloadSection();
        }
    }

    private void buildSpline()
    {
        container = gameObject.AddComponent<SplineContainer>();
        gravitySpline = container.AddSpline();
        float3 knotPos = Vector3.zero;

        BezierKnot[] knots = new BezierKnot[debugOrder.Length+1];
        knots[0] = new BezierKnot(knotPos);
        for(int i = 1; i < knots.Length; i++)
        {
            if (debugOrder[i-1] != 1) {
                if (cornerDir == -1)
                {
                    knotPos += new float3(0, 0, pipeLength);
                } else if (cornerDir == 1)
                {
                    knotPos += new float3(pipeLength, 0, 0);
                }
            } else {
                knotPos += new float3(cornerLength, 0, cornerLength);
                cornerDir *= -1;
            }

            knots[i] = new BezierKnot(knotPos);

            area[i - 1] = new SegmentNode(segmentPool[debugOrder[i-1]]);
            Debug.Log(i);
        }
        gravitySpline.Knots = knots;

        var allKnotsRange = new SplineRange(0, gravitySpline.Count);
        gravitySpline.SetTangentMode(allKnotsRange, TangentMode.AutoSmooth);
    }
}
