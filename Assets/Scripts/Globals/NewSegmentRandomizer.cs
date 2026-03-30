using UnityEngine;

public class NewSegmentRandomizer : MonoBehaviour
{
    #region Singleton instance
    private static NewSegmentRandomizer instance;

    public static NewSegmentRandomizer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<NewSegmentRandomizer>();
            }
            if (instance == null)
            {
                Debug.LogWarning("NO SEGMENT RANDOMIZER FOUND");
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    #endregion

    NewCornerNode[] corners;
    NewSegmentNode[][] segments;
    [SerializeField] GameObject[] cornerPrefabs;
    [SerializeField] GameObject[] segmentPool;
    [SerializeField] GameObject tutorialLevel;
    NewSegmentNode tutorial;
    [SerializeField] GameObject bossLevel;
    NewSegmentNode boss;

    // in the segment order -1 coresponds to corners other wise they corespond to the segmentPool index
    // the first and last values must be corners

    [SerializeField] int[] segmentOrder;

    [SerializeField] private Vector3 currentPoint = new Vector3(0, 0, 0);

    [SerializeField] private bool debugMode = true;
    [SerializeField] private int areaLength = 3;
    [SerializeField] private int numLevels = 5;
    private int numNests = 2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        tutorial = new NewSegmentNode(tutorialLevel);
        boss = new NewSegmentNode(bossLevel);
    }
    public void StartRun()
    {
        int cornerNum = 0;

        for (int i = 0; i < segmentOrder.Length; i++)
        {
            if (segmentOrder[i] < 0) cornerNum++;
        }

        corners = new NewCornerNode[cornerNum];
        segments = new NewSegmentNode[cornerNum - 1][];
        int segmentCount = 0;
        int segmentGroup = 0;

        if (!debugMode)
        {
            int area = 0;
            int areaProgress = 0;

            for (int i = 1; i < segmentOrder.Length; i++)
            {
                if (segmentOrder[i] >= 0 + numNests)
                {
                    switch (area)
                    {
                        case 0:
                            segmentOrder[i] = UnityEngine.Random.Range(numNests, numLevels + numNests);
                            areaProgress++;

                            break;
                        case 1:
                            segmentOrder[i] = UnityEngine.Random.Range(numNests, numLevels + numNests) + numLevels;
                            areaProgress++;

                            break;
                        default:
                            segmentOrder[i] = UnityEngine.Random.Range(numNests, numLevels + numNests) + (2 * numLevels);
                            areaProgress++;

                            break;
                    }
                    if (areaProgress >= areaLength)
                    {
                        areaProgress = 0;
                        area++;
                    }
                }
                else if (segmentOrder[i] < 0)
                {
                    switch (area)
                    {
                        case 0:
                            segmentOrder[i] = -1;

                            break;
                        case 1:
                            segmentOrder[i] = -2;

                            break;
                        default:
                            segmentOrder[i] = -3;

                            break;
                    }
                }
            }
        }

        for (int i = 1; i < segmentOrder.Length; i++)
        {
            if (segmentOrder[i] >= 0)
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

        corners[0] = new NewCornerNode(segments[0], null, cornerPrefabs[0], 0, this);

        for (int i = 1; i < segmentOrder.Length - 1; i++)
        {
            if (segmentOrder[i] < 0)
            {
                corners[cornerIndex] = new NewCornerNode(segments[cornerIndex], segments[cornerIndex - 1], cornerPrefabs[(segmentOrder[i] * -1) - 1], cornerIndex, this);
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

        corners[corners.Length - 1] = new NewCornerNode(null, segments[segments.Length - 1], cornerPrefabs[(-3 * -1) - 1], corners.Length - 1, this);

        StartLoad();
    }

    public void KillRun()
    {
        if (corners != null)
        {
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i].UnloadCorner();
                if (i < (corners.Length - 1))
                    corners[i].UnloadAhead();
            }
        }

        corners = null;
        segments = null;
        tutorial.UnloadSection();
        boss.UnloadSection();
    }

    private void StartLoad()
    {
        corners[0].SetInitialPos(Vector3.zero);
        LoadCorners(0);
    }

    public void LoadTutorial()
    {
        tutorial.LoadSection(Vector3.zero, Vector3.zero);
        GameplayManager.Instance.UpdateGravitySpline(tutorial.FindSpline());
    }

    private void LoadBoss()
    {
        KillRun();
        GameplayManager.Instance.LoadBoss();
        boss.LoadSection(Vector3.zero, Vector3.zero);
        GameplayManager.Instance.UpdateGravitySpline(boss.FindSpline());
    }

    public void LoadCorners(int index)
    {
        if (index > 0)
        {
            corners[index].LoadBehind();
            corners[index - 1].LoadCorner();
            if ((index - 1) > 0)
            {
                corners[index - 1].UnloadBehind();
                corners[index - 2].UnloadCorner();
            }
        }
        if (index > 0)
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
        else if (!GameplayManager.Instance.WashedOut)
        {
            LoadBoss();
        }
    }
}
