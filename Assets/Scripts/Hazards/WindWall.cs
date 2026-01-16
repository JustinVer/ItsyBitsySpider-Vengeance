using UnityEngine;

public class WindWall : MonoBehaviour
{
    private bool plugged = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "WebPlug")
        {
            plugged = true;
            //TODO: change to plugged model
        } 
        else if (other.gameObject == GameplayManager.Instance.Player && !plugged) 
        {
            //TODO: knockback code
        }
    }
}
