using UnityEngine;

public class Gun : MonoBehaviour
{

    [SerializeField] GameObject bullet;
    [SerializeField] Transform muzzle;

	[SerializeField] float shotForce;

	[SerializeField] Transform camPoint;

    // Update is called once per frame
    void Update()
    {
		transform.LookAt(camPoint.position + camPoint.forward * 100f);

		if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject newBullet = Instantiate(bullet, muzzle.position, Quaternion.identity);
        newBullet.GetComponent<Rigidbody>().AddForce(muzzle.forward * shotForce, ForceMode.Impulse);

	}
}
