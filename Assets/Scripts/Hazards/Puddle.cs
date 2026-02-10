using UnityEngine;

public class Puddle : MonoBehaviour
{
    private bool plugged = false;
    private bool slowing = false;
    private void Update()
    {
        if (slowing)
        {
            GameplayManager.Instance.PlayerBody.Slow(0.25f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "WebPlug")
        {
            plugged = true;
            //TODO: change to plugged model
        }
        else if (other.gameObject == GameplayManager.Instance.Player && !plugged)
        {
            slowing = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == GameplayManager.Instance.Player && !plugged)
        {
            slowing = false;
        }
    }
}
