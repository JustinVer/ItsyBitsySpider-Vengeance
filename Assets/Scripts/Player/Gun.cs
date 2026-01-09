using UnityEngine;

public class Gun : MonoBehaviour
{

    [SerializeField]
    GameObject bullet;
    [SerializeField]
    Transform muzzle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
            print("balls");
        }
    }

    void Shoot()
    {
        GameObject newBullet = Instantiate(bullet);
        newBullet.transform.position = muzzle.position;
    }
}
