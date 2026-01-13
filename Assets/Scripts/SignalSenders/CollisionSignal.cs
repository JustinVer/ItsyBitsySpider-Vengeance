using UnityEngine;

public class CollisionSignal : MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour receiverObject; // works

    public ICollisionReciever reciever => receiverObject as ICollisionReciever;

    private void OnCollisionEnter(Collision collision)
    {
        reciever.CollisionSignal(collision);
    }
}
