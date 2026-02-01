using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class SegmentNode : MonoBehaviour
{
    private Quaternion rotation = Quaternion.identity;
    private List<GameObject> platforms;
    private GameObject forwardTrigger;
    private GameObject backwardTrigger;
    private GameObject levelPrefab;
    private GameObject levelSection;
    private Transform pipeEnd;

    public SegmentNode(GameObject level)
    {
        levelPrefab = level;

        if (pipeEnd)
        {
            Debug.Log("found end");
        }

    }
    public Vector3 getEnd() { return pipeEnd.position; }
    public Quaternion getRotation() { return rotation; }
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

    public void loadSection(Vector3 lastEnd, Quaternion lastRotation)
    {
        levelSection = Instantiate(levelPrefab, lastEnd, lastRotation);
        pipeEnd = levelSection.transform.Find("end");
        rotation = lastRotation;
    }
}
