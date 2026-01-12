using UnityEngine;

public class SegmentNode
{
    private Vector3 Position;
    private Vector3 Rotation;
    private int segmentID;

    public SegmentNode(Vector3 p, Vector3 r, int segment)
    {
        Position = p;
        Rotation = r;
        segmentID = segment;
    }
    public Vector3 getPosition() { return Position; }
    public Vector3 getRotation() {  return Rotation; }
    public int getSegmentID() {  return segmentID; }
}
