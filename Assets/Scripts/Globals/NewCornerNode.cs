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
    GameObject levelSection;
    private int index;
    private NewSegmentRandomizer randomizer;


    public NewCornerNode(NewSegmentNode[] aheadSegments, NewSegmentNode[] behindSegments, GameObject corner, int index, NewSegmentRandomizer randomizer)
    {
        forwardSegments = aheadSegments;
        backwardSegments = behindSegments;
        cornerPrefab = corner;
        this.index = index;
        this.randomizer = randomizer;
    }
    public void Load()
    {
        randomizer.LoadCorners(index);
    }

    public void LoadCorner()
    {
        levelSection = Instantiate(cornerPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero));
    }

    public void UnloadBehind()
    {

    }

    public void UnloadAhead()
    {

    }
}
