using System.Collections.Generic;
using UnityEngine;

public class SegmentNode : MonoBehaviour
{
    private Quaternion rotation = Quaternion.identity;
    [SerializeField] private List<Platform> platforms;
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
    public Vector3 getBeginning() { return transform.position; }
    public Vector3 getEnd() { return pipeEnd.position; }
    public Quaternion getRotation() { return rotation; }
    public Quaternion exitAngle()
    {
        Vector3 directionToTarget = pipeEnd.position - orientationPoint.position;

        return Quaternion.LookRotation(directionToTarget);
    }
    public Quaternion entryAngle()
    {
        Vector3 directionToTarget = levelSection.transform.position - orientationPoint.position;

        return Quaternion.LookRotation(directionToTarget);
    }
    public void findPlatforms()
    {
        //TODO search for things marked as platforms
    }

    public List<Platform> getPlatforms() { return platforms; }

    public void loadSection(Vector3 lastEnd, Quaternion lastRotation)
    {
        Vector3 rotationVector = lastRotation.eulerAngles;
        rotationVector.z = Random.Range(0, 361);
        lastRotation = Quaternion.Euler(rotationVector);

        if (levelSection)
        {
            levelSection.transform.position = lastEnd;
            levelSection.transform.rotation = lastRotation;
        }
        else
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
