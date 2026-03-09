using UnityEngine;
using UnityEngine.Splines;
using static UnityEditor.PlayerSettings;

public class NewCornerNode : MonoBehaviour
{
    NewSegmentNode[] forwardSegments;
    NewSegmentNode[] backwardSegments;
    GameObject cornerPrefab;
    LoadingTrigger forwardTrigger;
    LoadingTrigger backwardTrigger;
    GameObject levelSection;
    private bool active = false;
    private int index;
    private Vector3 position;
    private NewSegmentRandomizer randomizer;
    SplineContainer segmentSplineContainer;

    private float cornerDimension = 28;
    private float pipeLength = 44;
    private float scale = 2.5f;

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
            forwardTrigger = levelSection.transform.Find("Trigger1").GetComponent<LoadingTrigger>();
            forwardTrigger.SetOwner(this);
            backwardTrigger = levelSection.transform.Find("Trigger2").GetComponent<LoadingTrigger>();
            backwardTrigger.SetOwner(this);
            if (segmentSplineContainer == null)
            {
                segmentSplineContainer = levelSection.GetComponent<SplineContainer>();
                if(segmentSplineContainer != null)
                    GameplayManager.Instance.UpdateGravitySpline(segmentSplineContainer);
            }
        }
    }

    public void UnloadCorner()
    {
        if (levelSection)
        {
            Destroy(levelSection);
        }
    }

    public void SetInitialPos(Vector3 newPos)
    {
       position = newPos;
    }

    public void SetPos(Vector3 newPos)
    {
        if ((index % 2) == 0)
        {
            position = newPos + new Vector3(-(cornerDimension * scale), 0, (cornerDimension * scale)) + new Vector3(0, 0, (pipeLength * scale) * backwardSegments.Length);
        }
        else if ((index % 2) == 1)
        {
            position = newPos + new Vector3(-(cornerDimension * scale), 0, (cornerDimension * scale)) + new Vector3(-(pipeLength * scale) * backwardSegments.Length, 0, 0);
        }
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
            if ((index % 2) == 0)
            {
                forwardSegments[i].LoadSection(
                    position + new Vector3(-(cornerDimension * scale),0, (cornerDimension * scale)) + new Vector3(-(pipeLength * scale) * i, 0, 0),
                    new Vector3(0, 270, 0));
            } else if ((index % 2) == 1)
            {
                forwardSegments[i].LoadSection(
                    position + new Vector3(-(cornerDimension * scale), 0, (cornerDimension * scale)) + new Vector3(0, 0, (pipeLength * scale) * i),
                    new Vector3(0, 0, 0));
            }
        }
    }

    public void LoadBehind()
    {
        //calls LoadSection() from backwardSegments
        for (int i = backwardSegments.Length - 1; i > -1; i -= 1)
        {
            if ((index % 2) == 0)
            {
                backwardSegments[i].LoadSection(
                    position + new Vector3(0, 0, -(pipeLength * scale) * (i+1)),
                    new Vector3(0, 0, 0));
            }
            else if ((index % 2) == 1)
            {
                backwardSegments[i].LoadSection(
                    position + new Vector3((pipeLength * scale) * (i+1), 0, 0),
                    new Vector3(0, 270, 0));
            }
        }
    }

    public void UnloadBehind()
    {
        //calls UnloadSection() from backwardSegments
        UnloadSegments(backwardSegments);
    }

    public void UnloadAhead()
    {
        //calls UnloadSection() from forwardSegments
        UnloadSegments(forwardSegments);
    }

    private void UnloadSegments(NewSegmentNode[] segments)
    {
        if(segments != null)
        {
            for (int i = 0; i < segments.Length; i++)
            {
                segments[i].UnloadSection();
            }
        }
    }

    public void TriggerCall()
    {
        if (!active)
        {
            Load();
        }
    }
}
