using UnityEngine;

public class Gun : MonoBehaviour
{

    [SerializeField] GameObject bullet;
    [SerializeField] Transform muzzle;

    [SerializeField] float shotForce;

    [SerializeField] Camera playerCamera;

    [SerializeField] float rotationSpeed = 180f;

    [SerializeField] Transform folloeTransform;

    [SerializeField] int damage = 10;

    [SerializeField] LayerMask fireMask;

    [SerializeField] private ParticleSystem muzzleFlash1;
    [SerializeField] private ParticleSystem muzzleFlash2;

    [SerializeField] private float castWidth = 0.1f;

    Vector3 hitPosition;

    bool canFire = true;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = folloeTransform.position;
        //transform.LookAt(camPoint.position + camPoint.forward * 100f);
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 200f))
        {
            hitPosition = hit.point;
        }
        else
        {
            hitPosition = playerCamera.transform.position + playerCamera.transform.forward * 200f;
        }


        this.transform.rotation = Quaternion.LookRotation(hitPosition - this.transform.position, GameplayManager.Instance.GetGravity(this.transform.position));
        //Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow, 1f);
        //Debug.DrawLine(this.transform.position, hit.point, Color.red, 1f);

        if (GameplayManager.Instance.Fire && canFire)
        {
            Shoot();
            canFire = false;
        }

        if (!GameplayManager.Instance.Fire)
        {
            canFire = true;
        }
    }

    void Shoot()
    {
        GameObject newBullet = Instantiate(bullet, muzzle.position, Quaternion.identity);
        newBullet.GetComponent<Rigidbody>().AddForce(muzzle.forward * shotForce, ForceMode.Impulse);
        newBullet.transform.forward = muzzle.forward;
        muzzleFlash1.Play();
        muzzleFlash2.Play();


        //hitcast
        RaycastHit hit;
        bool hitObjectBool = false;
        if (Physics.BoxCast(playerCamera.transform.position, new Vector3(castWidth, castWidth, castWidth), playerCamera.transform.forward, out hit, Quaternion.identity, (hitPosition - playerCamera.transform.position).magnitude, fireMask))
        {
            hitObjectBool = true;
            IDamageable hitObject = hit.collider.gameObject.GetComponent<IDamageable>();
            Debug.Log("GUN: " + hit.collider.gameObject.name + " " + hitObjectBool + " " + hitPosition + " " + hit.collider.transform.position);
            if (hitObject != null)
            {
                hitObject.modifyHP(-damage);
                hitObject.hitEffect(hitPosition, this.transform.forward * -1);
            }
        }
        else
        {
            Debug.Log("GUN: " + hit + " " + false + " " + hitPosition);
        }
    }
}
