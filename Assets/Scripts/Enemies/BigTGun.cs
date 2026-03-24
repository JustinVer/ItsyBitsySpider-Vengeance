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

    [SerializeField] private AudioClip shoot;
    [SerializeField, Range(0, 1)] private float shootVolume = 0.5f;

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
        AudioManager.Instance.PlaySound(shoot, shootVolume, muzzle.transform.position);

        for (int i = 0; i < numBulletsPerVolley; i++)
        {
            Vector3 muzzleInverse = new Vector3();
            muzzleInverse.x = 0.9f - muzzle.forward.x;
            muzzleInverse.y = 0.9f - muzzle.forward.y;
            muzzleInverse.z = 0.9f - muzzle.forward.z;

            Vector3 spreadAmount = Random.insideUnitSphere;
            spreadAmount.x = spreadAmount.x * Mathf.Abs(spreadAmount.x);
            spreadAmount.y = spreadAmount.y * Mathf.Abs(spreadAmount.y);
            spreadAmount.z = spreadAmount.z * Mathf.Abs(spreadAmount.z);


            Vector3 spreadDirection = new Vector3(spreadAmount.x * muzzleInverse.x, spreadAmount.y * muzzleInverse.y, spreadAmount.z * muzzleInverse.z);

            GameObject newBullet = Instantiate(bullet, muzzle.position + (spreadDirection / 2.5f), Quaternion.identity);

            spreadDirection *= maxFireAngle;

            newBullet.transform.forward = spreadDirection + muzzle.forward;
            newBullet.GetComponent<Rigidbody>().linearVelocity = ProjectileVelocity * newBullet.transform.forward;

            Debug.Log("Gun spread amount " + spreadAmount + " " + spreadDirection + " " + (spreadDirection + muzzle.forward) + " " + (ProjectileVelocity * newBullet.transform.forward));
        }

    }

    private Vector3 getPlayerProjectionPosition()
    {
        return GameplayManager.Instance.PlayerBody.transform.position + (GameplayManager.Instance.GetGravity(GameplayManager.Instance.PlayerBody.transform.position).normalized * spreadDistanceDivider) + ((GameplayManager.Instance.PlayerBody.LinearVelocity() * fireVelocityDistanceMultiplier * Mathf.Abs(Vector3.Distance(GameplayManager.Instance.PlayerBody.transform.position, this.transform.position))) / ProjectileVelocity);
    }
}