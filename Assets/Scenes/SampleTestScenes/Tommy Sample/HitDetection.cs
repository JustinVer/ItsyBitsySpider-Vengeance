using UnityEngine;

public class HitDetection : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null && collision.gameObject.tag == "Bullet")
        {
            // ADJUST ENEMY HEALTH IN DESIGNATED SCRIPT
            print("Hit");
			Destroy(collision.gameObject);
		}
    }
}
