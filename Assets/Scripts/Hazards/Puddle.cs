using UnityEngine;

public class Puddle : MonoBehaviour
{
    private bool plugged = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "WebPlug")
        {
            plugged = true;
            //TODO: change to plugged model
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == GameplayManager.Instance.Player && !plugged)
        {
            Debug.Log("Puddle slow player");
            GameplayManager.Instance.PlayerBody.Slow(0.25f);
        }
    }
}
