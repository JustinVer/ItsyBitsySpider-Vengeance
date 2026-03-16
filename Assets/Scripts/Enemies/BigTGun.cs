using UnityEngine;

public class BigTGun : MonoBehaviour
{

    [SerializeField] GameObject bullet;
    [SerializeField] Transform muzzle;

    [SerializeField] float rotationSpeed = 180f;

    [SerializeField] Transform folloeTransform;

    [SerializeField] private ParticleSystem muzzleFlash1;
    [SerializeField] private ParticleSystem muzzleFlash2;

    Vector3 hitPosition;

    bool canFire = true;

    [SerializeField] private float ProjectileVelocity = 10f;
    [SerializeField] private float fireVelocityDistanceMultiplier = 0.5f;
    [SerializeField] private float maxFireAngle = 8f;
    [SerializeField] private int numBulletsPerVolley = 10;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = folloeTransform.position;
        this.transform.rotation = Quaternion.LookRotation(getPlayerProjectionPosition() - folloeTransform.position, GameplayManager.Instance.GetGravity(this.transform.position));
    }

    public void Shoot()
    {

        muzzleFlash1.Play();
        muzzleFlash2.Play();

        for (int i = 0; i < numBulletsPerVolley; i++)
        {
            Vector3 spreadDirection = Quaternion.Euler(Mathf.Pow(Random.value, 3) * maxFireAngle, Mathf.Pow(Random.value, 3) * maxFireAngle, 0) * muzzle.forward;
            if (i % 4 == 0)
            {
                spreadDirection.x *= -1;
            }
            else if (i % 4 == 1)
            {
                spreadDirection.y *= -1;
            }
            else if (i % 4 == 2)
            {
                spreadDirection.x *= -1;
                spreadDirection.y *= -1;
            }
            GameObject newBullet = Instantiate(bullet, muzzle.position, Quaternion.identity);
            newBullet.GetComponent<Rigidbody>().linearVelocity = ProjectileVelocity * spreadDirection;
            newBullet.transform.forward = spreadDirection;

        }

    }

    private Vector3 getPlayerProjectionPosition()
    {
        return GameplayManager.Instance.PlayerBody.transform.position + ((GameplayManager.Instance.PlayerBody.LinearVelocity() * fireVelocityDistanceMultiplier * Mathf.Abs(Vector3.Distance(GameplayManager.Instance.PlayerBody.transform.position, this.transform.position))) / ProjectileVelocity);
    }
}