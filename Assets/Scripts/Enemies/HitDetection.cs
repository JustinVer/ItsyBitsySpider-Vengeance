using UnityEngine;

public class HitDetection : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // GET THE HEALTH MANAGER SCRIPT FOR ENEMYS
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null && collision.gameObject.tag == "Bullet")
        {
            // ADJUST ENEMY HEALTH IN DESIGNATED SCRIPT
            print("Hit");
        }
    }
}
