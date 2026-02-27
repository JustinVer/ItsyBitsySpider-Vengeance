using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    private static List<GameObject> visiblePoints;
    public static List<GameObject> VisiblePoints
    {
        get
        {
            if (visiblePoints == null)
            {
                visiblePoints = new List<GameObject>();
            }
            return visiblePoints;
        }

    }

    private void Awake()
    {
        if (visiblePoints == null) visiblePoints = new List<GameObject>();
        visiblePoints.Add(gameObject);
    }

    //private void OnBecameVisible()
    //{
    //    if (!VisiblePoints.Contains(gameObject))
    //    {
    //        VisiblePoints.Add(gameObject);
    //        Debug.Log("VISIBLE");
    //    }
    //}

    //private void OnBecameInvisible()
    //{
    //    if (VisiblePoints.Contains(gameObject))
    //    {
    //        VisiblePoints.Remove(gameObject);
    //        Debug.Log("INVISIBLE");
    //    }
    //}
}
