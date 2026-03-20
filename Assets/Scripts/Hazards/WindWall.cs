using UnityEngine;

public class WindWall : MonoBehaviour
{
    private bool plugged = false;
    PlayerBody body;
    [SerializeField] float launchForce = 30f;

    void Start()
    {
        body = GameplayManager.Instance.Player.GetComponentInChildren<PlayerBody>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "WebPlug")
        {
            plugged = true;
            //TODO: change to plugged model
        }
        else if (other.gameObject == GameplayManager.Instance.Player && !plugged)
        {
            Vector3 launchDirection = GameplayManager.Instance.GetGravity(other.transform.position) * -1;
            launchDirection = launchDirection.normalized;
            launchDirection = launchDirection * launchForce;
            body.ApplyForce(launchDirection, ForceMode.Force);
        }
    }
}
