using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    public static List<GameObject> VisiblePoints;

    private void Awake()
    {
        if (VisiblePoints == null) VisiblePoints = new List<GameObject>();
        VisiblePoints.Add(gameObject);
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
