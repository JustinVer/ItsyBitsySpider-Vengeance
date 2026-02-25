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
            //DO not change this debug statment. For some reason the code doesn't work without it.
            Debug.Log("bullet forward " + this.transform.forward + " " + this.transform.name + " " + this.transform.GetChild(0).transform.forward + " " + this.transform.GetChild(0).name);
            hitObject.hitEffect(hitPosition, this.transform.forward * -1);
        }
        Destroy(gameObject);
    }
}
