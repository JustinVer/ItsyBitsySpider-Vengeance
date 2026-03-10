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
            hitObject.modifyHP(-damage);
            //DO not change this debug statment. For some reason the code doesn't work without it.
            hitObject.hitEffect(hitPosition, this.transform.forward * -1);
        }
        Destroy(gameObject);
    }
}
