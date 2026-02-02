using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Unity.VisualScripting;
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
    private Transform orientationPoint;

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
    public Quaternion exitAngle() 
    {
        Vector3 directionToTarget = pipeEnd.position - orientationPoint.position;

        return Quaternion.LookRotation(directionToTarget);
    }
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
        Vector3 rotationVector = lastRotation.eulerAngles;
        rotationVector.z = Random.Range(0, 361);
        lastRotation = Quaternion.Euler(rotationVector);

        if (levelSection)
        {
            levelSection.transform.position = lastEnd;
            levelSection.transform.rotation = lastRotation;
        } else
        {
            levelSection = Instantiate(levelPrefab, lastEnd, lastRotation);
        }
        pipeEnd = levelSection.transform.Find("end");
        orientationPoint = levelSection.transform.Find("orientationPoint");
        rotation = lastRotation;
    }

    public void unloadSection()
    {
        if (levelSection)
        {
            Destroy(levelSection);
        }
    }
}
