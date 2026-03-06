using System.Collections.Generic;
using UnityEngine;

public class NewSegmentRandomizer : MonoBehaviour
{
    NewCornerNode[] corners;
    NewSegmentNode[][] segments;
    [SerializeField] int[] segmentOrder;
    [SerializeField] GameObject corner;
    [SerializeField] GameObject[] segmentPool;

    [SerializeField] private Vector3 currentPoint = new Vector3(0, 0, 0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int cornerNum = 0;

        for (int i = 0; i < segmentOrder.Length; i++)
        {
            if (segmentOrder[i] == -1) cornerNum++;
        }
        corners = new NewCornerNode[cornerNum];
        segments = new NewSegmentNode[cornerNum + 1][];

        int index = 0;
        for (int i = 0; i < cornerNum + 1; i++)
        {
            List<int> tempSection = new List<int>();
            for (; ; )
            {
                if (segmentOrder[index] == -1)
                {
                    segments[i] = new NewSegmentNode[tempSection.Count];
                    for (int k = 0; k < tempSection.Count; k++)
                    {
                        segments[i][k] = new NewSegmentNode(segmentPool[tempSection[k]]);
                    }
                    if (i != 0)
                    {
                        corners[i - 1] = new NewCornerNode(segments[i - 1], segments[i], corner, i - 1, this);
                    }
                    index++;
                    break;
                }
                else
                {
                    tempSection.Add(segmentOrder[i]);
                    index++;
                }
            }
        }
    }

    private void StartLoad()
    {
        corners[0].LoadCorner();
        corners[0].LoadAhead();
        corners[1].LoadCorner();
        corners[1].LoadAhead();
        corners[2].LoadCorner();
    }

    public void LoadCorners(int index)
    {
        if (index > 0)
        {
            corners[index].LoadBehind();
            corners[index - 1].LoadCorner();
            corners[index - 1].UnloadBehind();
        }

        corners[index].LoadCorner();

        if (index < corners.Length)
        {
            corners[index].LoadAhead();
            corners[index + 1].LoadCorner();
            corners[index + 1].UnloadAhead();
        }
    }
}
