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
    [SerializeField] private float spreadDistanceDivider = 1.0f;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = folloeTransform.position;
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(getPlayerProjectionPosition() - folloeTransform.position, folloeTransform.up), rotationSpeed * Time.deltaTime);
    }

    public void Shoot()
    {

        muzzleFlash1.Play();
        muzzleFlash2.Play();

        for (int i = 0; i < numBulletsPerVolley; i++)
        {
            Vector3 muzzleInverse = new Vector3();
            muzzleInverse.x = 1 - muzzle.forward.x;
            muzzleInverse.y = 1 - muzzle.forward.y;
            muzzleInverse.z = 1 - muzzle.forward.z;

            Vector3 spreadAmount = Random.insideUnitSphere;
            Vector3 spreadDirection = new Vector3(spreadAmount.x * muzzleInverse.x, spreadAmount.y * muzzleInverse.y, spreadAmount.z * muzzleInverse.z);
            spreadDirection = spreadDirection.normalized * maxFireAngle;

            GameObject newBullet = Instantiate(bullet, muzzle.position + spreadDirection / 5.0f, Quaternion.identity);
            newBullet.transform.forward = spreadDirection + muzzle.forward;
            newBullet.GetComponent<Rigidbody>().linearVelocity = ProjectileVelocity * newBullet.transform.forward;

        }

    }

    private Vector3 getPlayerProjectionPosition()
    {
        return GameplayManager.Instance.PlayerBody.transform.position + (GameplayManager.Instance.GetGravity(GameplayManager.Instance.PlayerBody.transform.position).normalized * spreadDistanceDivider) + ((GameplayManager.Instance.PlayerBody.LinearVelocity() * fireVelocityDistanceMultiplier * Mathf.Abs(Vector3.Distance(GameplayManager.Instance.PlayerBody.transform.position, this.transform.position))) / ProjectileVelocity);
    }
}