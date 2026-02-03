using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Platform : MonoBehaviour
{
    List<Transform> availPoints = new List<Transform>();
    List<Transform> takenPoints = new List<Transform>();

    public Transform getPlatformPoint()
    {
        if (availPoints != null && availPoints.Count > 0)
        {
            int rand = Random.Range(0, availPoints.Count);
            Transform point = availPoints.ElementAt(rand);
            availPoints.Remove(point);
            takenPoints.Add(point);

            return point;
        }
        else
        {
            return null;
        }
    }

    public void returnPlatformPoint(Transform point)
    {
        if (takenPoints.Contains(point))
        {
            takenPoints.Remove(point);
            availPoints.Add(point);
        }
    }
}
