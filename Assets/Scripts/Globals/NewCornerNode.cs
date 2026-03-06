using UnityEngine;

public class NewCornerNode : MonoBehaviour
{
    NewSegmentNode[] forwardSegments;
    NewSegmentNode[] backwardSegments;
    GameObject cornerPrefab;
    GameObject forwardTrigger;
    GameObject backwardTrigger;
    GameObject levelSection;
    private bool active = false;
    private int index;
    private Vector3 position;
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
        if (!levelSection)
        {
            levelSection = Instantiate(cornerPrefab, position, Quaternion.Euler(new Vector3(0, 270 * (index % 2), 180 * (index % 2))));
        }
    }

    public void SetPos(Vector3 newPos)
    {
        position = newPos;
    }

    public Vector3 GetPos()
    {
        return position;
    }

    public void LoadAhead()
    {
        //calls LoadSection() from forwardSegments
        for (int i = 0; i < forwardSegments.Length; i++)
        {
            forwardSegments[i].LoadSection(new Vector3(), new Vector3(0, 270 * (index % 2), 0));
        }
    }

    public void LoadBehind()
    {
        //calls LoadSection() from backwardSegments
    }

    public void UnloadBehind()
    {
        //calls UnloadSection() from backwardSegments
    }

    public void UnloadAhead()
    {
        //calls UnloadSection() from forwardSegments
    }

    public void TriggerCall()
    {
        if (!active)
        {
            Load();
        }
    }
}
