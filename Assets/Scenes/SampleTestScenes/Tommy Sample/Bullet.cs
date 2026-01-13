using UnityEngine;

public class Bullet : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		if (collision != null && collision.gameObject.tag != "Enemy")
		{
			// Has Hit Terrain
			print("destroy bullet");
			Destroy(gameObject);
			// Likely to have particles here for terrain or dust being kicked up
		}
	}
}
