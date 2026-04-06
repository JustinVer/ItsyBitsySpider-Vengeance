using UnityEngine;
using UnityEngine.Splines;

public class NewSegmentNode : MonoBehaviour
{
    private Vector3 position = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    GameObject thisSegment;
    GameObject segmentPrefab;
    SplineContainer segmentSplineContainer = null;

    public NewSegmentNode(GameObject levelSegment)
    {
        segmentPrefab = levelSegment;
    }
    public void LoadSection(Vector3 pos, Vector3 rot)
    {
        if (!thisSegment)
        {
            position = pos;
            rotation = rot;
            thisSegment = Instantiate(segmentPrefab, position, Quaternion.Euler(rotation));
            /*if (segmentSplineContainer == null)
            {
                segmentSplineContainer = thisSegment.GetComponent<SplineContainer>();
                if (segmentSplineContainer != null)
                    GameplayManager.Instance.UpdateGravitySpline(segmentSplineContainer);
            }*/
        }
    }
    public void UnloadSection()
    {
        if (thisSegment)
            Destroy(thisSegment);
    }

    public SplineContainer FindSpline()
    {
        segmentSplineContainer = thisSegment.GetComponentInChildren<SplineContainer>();

        return segmentSplineContainer;
    }
}
