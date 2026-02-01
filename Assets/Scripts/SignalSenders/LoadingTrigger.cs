using System.Collections;
using UnityEngine;

public class LoadingTrigger : MonoBehaviour
{
    private SegmentRandomizer segmentRandomizer;
    private bool forward;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        segmentRandomizer = Object.FindFirstObjectByType<SegmentRandomizer>();
        if(this.name == "forwardTrigger")
        {
            forward = true;
        } else
        {
            forward = false;
        }
    }
    public void reposition(Vector3 newPos, Quaternion newRot)
    {
        this.transform.rotation = newRot;
        this.transform.position = newPos - new Vector3(0, 0, 10);
        StartCoroutine(DelayedAction(3f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (forward == true)
        {
            segmentRandomizer.loadForward();
        }
    }
    IEnumerator DelayedAction(float waitTime)
    {
        // Wait for the specified number of seconds
        yield return new WaitForSeconds(waitTime);

        segmentRandomizer.loadForward();
    }

}
