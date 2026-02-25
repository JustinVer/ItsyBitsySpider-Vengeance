using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        Vector3 hitPosition = other.ClosestPoint(this.transform.position);
        IDamageable hitObject = other.gameObject.GetComponent<IDamageable>();
        if (hitObject != null)
        {
            Debug.Log("Bullet hit " + hitObject);
            hitObject.modifyHP(-damage);
            hitObject.hitEffect(hitPosition);
        }
        Destroy(gameObject);
    }
}
