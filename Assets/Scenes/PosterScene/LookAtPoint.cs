using UnityEngine;

public class LookAtPoint : MonoBehaviour
{
    [SerializeField] Transform target;
    void Start()
    {
        transform.rotation = Quaternion.LookRotation(target.position);   
    }
}
