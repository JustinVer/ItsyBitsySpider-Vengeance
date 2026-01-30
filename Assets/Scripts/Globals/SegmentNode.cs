using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SegmentNode : MonoBehaviour
{
    private Vector3 Position = Vector3.zero;
    private Vector3 Rotation = Vector3.zero;
    private List<GameObject> platforms;
    private GameObject forwardTrigger;
    private GameObject backwardTrigger;
    private GameObject levelPrefab;
    private Transform pipeStart;
    private Transform pipeEnd;

    public SegmentNode(GameObject level)
    {
        levelPrefab = level;
        pipeStart = levelPrefab.transform.Find("beginning");
        pipeEnd = levelPrefab.transform.Find("end");
    }
    public Vector3 getPosition() { return Position; }
    public void setPosition(Vector3 newPos) { Position = newPos; }
    public Vector3 getRotation() { return Rotation; }
    public void setRotation(Vector3 newRot) { Rotation = newRot; }

    public void findPlatforms()
    {
        //TODO search for  things marked as platforms
    }

    public void findTriggers()
    {
        forwardTrigger = GameObject.Find("Forward Trigger");
        backwardTrigger = GameObject.Find("Backward Trigger");
    }

    public List<GameObject> getPlatforms() { return platforms; }

    public async Task loadSection(Transform lastEnd)
    {
        AsyncInstantiateOperation op = InstantiateAsync(levelPrefab);

        await op;
    }
}
