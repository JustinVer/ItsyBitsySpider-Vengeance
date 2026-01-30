using System.Collections.Generic;
using UnityEngine;

public class SegmentNode
{
    private Vector3 Position = Vector3.zero;
    private Vector3 Rotation = Vector3.zero;
    private int segmentID;
    private List<GameObject> platforms;

    public SegmentNode(int segment)
    {
        segmentID = segment;
    }
    public Vector3 getPosition() { return Position; }
    public void setPosition(Vector3 newPos) { Position = newPos; }
    public Vector3 getRotation() { return Rotation; }
    public void setRotation(Vector3 newRot) { Rotation = newRot; }
    public int getSegmentID() { return segmentID; }

    public void findPlatforms()
    {
        //TODO search for  things marked as platforms
    }

    public List<GameObject> getPlatforms() { return platforms; }
}
