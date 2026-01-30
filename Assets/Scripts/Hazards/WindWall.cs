using Unity.VisualScripting;
using UnityEngine;

public class WindWall : MonoBehaviour
{
    private bool plugged = false;
    PlayerBody body;
    Vector3 bottomPoint;
    [SerializeField] float launchForce = 30f;

    void Start()
    {
        body = GameplayManager.Instance.Player.GetComponentInChildren<PlayerBody>();
        bottomPoint = gameObject.transform.position;
        bottomPoint.y = 0; //this is wrong
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
            Vector3 launchDirection = other.transform.position - bottomPoint;
            launchDirection = launchDirection.normalized;
            launchDirection = launchDirection * launchForce;
            body.ApplyForce(launchDirection, ForceMode.Impulse);
        }
    }
}
