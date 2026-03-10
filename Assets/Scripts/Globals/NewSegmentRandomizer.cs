using System.Collections.Generic;
using UnityEngine;

public class NewSegmentRandomizer : MonoBehaviour
{
    NewCornerNode[] corners;
    NewSegmentNode[][] segments;
    [SerializeField] GameObject corner;
    [SerializeField] GameObject[] segmentPool;

    // in the segment order -1 coresponds to corners other wise they corespond to the segmentPool index
    // the first and last values must be corners

    [SerializeField] int[] segmentOrder;

    [SerializeField] private Vector3 currentPoint = new Vector3(0, 0, 0);

    [SerializeField] private bool debugMode = true;
    [SerializeField] private int areaLength = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int cornerNum = 0;

        for (int i = 0; i < segmentOrder.Length; i++)
        {
            if (segmentOrder[i] == -1) cornerNum++;
        }

        corners = new NewCornerNode[cornerNum];
        segments = new NewSegmentNode[cornerNum - 1][];
        int segmentCount = 0;
        int segmentGroup = 0;

        if (!debugMode)
        {
            for (int i = 1; i < segmentOrder.Length; i++)
            {
                if (segmentOrder[i] != -1)
                {
                    //TODO:generate random level index
                }
            }
        }

        for (int i = 1; i < segmentOrder.Length; i++)
        {
            if (segmentOrder[i] != -1)
                segmentCount++;
            else
            {
                segments[segmentGroup] = new NewSegmentNode[segmentCount];
                segmentGroup++;
                segmentCount = 0;
            }
        }

        int cornerIndex = 1;
        segmentGroup = 0;
        segmentCount = 0;

        corners[0] = new NewCornerNode(segments[0], null, corner, 0, this);

        for (int i = 1; i < segmentOrder.Length-1; i++)
        {
            if (segmentOrder[i] == -1)
            {
                corners[cornerIndex] = new NewCornerNode(segments[cornerIndex], segments[cornerIndex-1], corner, cornerIndex, this);
                cornerIndex++;
                segmentGroup++;
                segmentCount = 0;
            }
            else
            {
                segments[segmentGroup][segmentCount] = new NewSegmentNode(segmentPool[segmentOrder[i]]);
                segmentCount++;
            }
        }

        corners[corners.Length-1] = new NewCornerNode(null, segments[segments.Length - 1], corner, corners.Length - 1, this);

        StartLoad();
    }

    private void StartLoad()
    {
        corners[0].SetInitialPos(Vector3.zero);
        corners[0].LoadCorner();
        corners[0].LoadAhead();
        corners[1].SetPos(corners[0].GetPos());
        corners[1].LoadCorner();
    }

    public void LoadCorners(int index)
    {
        if (index > 0)
        {
            corners[index].LoadBehind();
            corners[index - 1].LoadCorner();
            if((index - 1) > 0)
            {
                corners[index - 1].UnloadBehind();
                corners[index - 2].UnloadCorner();
            }
        }
        if(index>0)
            corners[index].SetPos(corners[index - 1].GetPos());
        corners[index].LoadCorner();

        if (index < corners.Length - 1)
        {
            corners[index].LoadAhead();
            corners[index + 1].SetPos(corners[index].GetPos());
            corners[index + 1].LoadCorner();
            if ((index + 1) < corners.Length - 1)
            {
                corners[index + 1].UnloadAhead();
                corners[index + 2].UnloadCorner();
            }
        }
    }
}
