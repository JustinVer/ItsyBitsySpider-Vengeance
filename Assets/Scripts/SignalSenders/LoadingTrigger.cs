using System.Collections;
using UnityEngine;

public class LoadingTrigger : MonoBehaviour
{
    private NewCornerNode corner;
    private bool forward;

    public void SetOwner(NewCornerNode newOwner)
    {
        corner = newOwner;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameplayManager.Instance.Player)
        {
            corner.Load();
        }
    }
}
