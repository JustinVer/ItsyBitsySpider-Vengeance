using UnityEngine;

public class Gun : MonoBehaviour
{

    [SerializeField] GameObject bullet;
    [SerializeField] Transform muzzle;

    [SerializeField] float shotForce;

    [SerializeField] Camera playerCamera;

    [SerializeField] float rotationSpeed = 180f;

    [SerializeField] Vector3 hitPosition;

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(camPoint.position + camPoint.forward * 100f);
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 999f, GameplayManager.Instance.NotPlayerOrEnemyMask, QueryTriggerInteraction.Ignore);

        hitPosition = hit.point;

        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(hit.point - this.transform.position, GameplayManager.Instance.GetGravity(this.transform.position)), rotationSpeed * Time.deltaTime);
        //Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow, 1f);
        //Debug.DrawLine(this.transform.position, hit.point, Color.red, 1f);

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
