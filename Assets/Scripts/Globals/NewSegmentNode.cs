using UnityEngine;
using UnityEngine.Splines;

public class NewSegmentNode : MonoBehaviour
{
    private Vector3 Position = Vector3.zero;
    private Vector3 Rotation = Vector3.zero;
    GameObject thisSegment;
    Spline gravitySpline;

    public NewSegmentNode(GameObject levelSegment)
    {
        thisSegment = levelSegment;
        gravitySpline = thisSegment.GetComponent<SplineContainer>().Splines[0];
    }
    public void LoadSection()
    {

    }
    public void UnloadSection()
    {

    }
    public void SetPosition(Vector3 newPos)
    {

    }
    public void SetRotation(Vector3 newRot)
    {

    }
}
