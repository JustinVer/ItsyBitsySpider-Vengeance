using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] GameObject parent;
    void Update()
    {
        Vector3 forwards = GameplayManager.Instance.GetForward(parent.transform.position);
        Vector3 down = GameplayManager.Instance.GetGravity(parent.transform.position);
        Quaternion rotation = Quaternion.LookRotation(transform.position + forwards, -down);
        transform.rotation = rotation;
    }
}
