using System;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class NewSegmentNode : MonoBehaviour
{
    private Vector3 position = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    GameObject thisSegment;
    GameObject segmentPrefab;
    Spline gravitySpline;

    public NewSegmentNode(GameObject levelSegment)
    {
        segmentPrefab = levelSegment;
    }
    public void LoadSection(Vector3 pos, Vector3 rot)
    {
        if (position == null)
            position = pos;
        if (rotation == null)
            rotation = rot;

        if (!thisSegment)
        {
            thisSegment = Instantiate(segmentPrefab, position, Quaternion.Euler(rotation));
            if (gravitySpline == null)
            {
                gravitySpline = thisSegment.GetComponent<SplineContainer>().Splines[0];
                GameplayManager.Instance.UpdateGravitySpline(gravitySpline);
            }
        }
    }
    public void UnloadSection()
    {
        if(thisSegment)
            Destroy(thisSegment);
    }
}
