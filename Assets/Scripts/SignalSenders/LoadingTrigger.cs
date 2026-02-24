using System.Collections;
using UnityEngine;

public class LoadingTrigger : MonoBehaviour
{
    private NewCornerNode corner;
    private bool forward;

    public void SetOwner(NewCornerNode newOwner)
    {
        corner = newOwner;
        StartCoroutine(DelayedAction(10f));
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other == GameplayManager.Instance.Player)
        {
            corner.Load();
        }
    }
    IEnumerator DelayedAction(float waitTime)
    {
        // Wait for the specified number of seconds
        yield return new WaitForSeconds(waitTime);

        corner.Load();
    }
}
