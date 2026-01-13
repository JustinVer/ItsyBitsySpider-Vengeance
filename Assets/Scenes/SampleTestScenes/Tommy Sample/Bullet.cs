using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    private void OnCollisionEnter(Collision collision)
    {
        IDamageable hitObject = collision.gameObject.GetComponent<IDamageable>();
        if (hitObject != null)
        {
            hitObject.modifyHP(-damage);
        }
        Destroy(gameObject);
        //Add partical effects and things
    }
}
