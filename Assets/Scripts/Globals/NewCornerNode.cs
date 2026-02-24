using UnityEngine;

public class NewCornerNode : MonoBehaviour
{
    NewSegmentNode[] forwardSegments;
    NewSegmentNode[] backwardSegments;
    GameObject cornerPrefab;
    GameObject preForwardTrigger;
    GameObject preBackwardTrigger;
    GameObject forwardTrigger;
    GameObject backwardTrigger;



    public NewCornerNode(NewSegmentNode[] aheadSegments, NewSegmentNode[] behindSegments, GameObject corner)
    {
        forwardSegments = aheadSegments;
        backwardSegments = behindSegments;
        cornerPrefab = corner;
    }
    public void Load()
    {

    }
}
