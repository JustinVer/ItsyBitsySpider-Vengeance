using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] GameObject parent;
    void Update()
    {
        Vector3 forwards = GameplayManager.Instance.GetForward(parent.transform.position).normalized;
        Vector3 up = -GameplayManager.Instance.GetGravity(parent.transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(forwards, up);
        transform.rotation = rotation;
    }
}
