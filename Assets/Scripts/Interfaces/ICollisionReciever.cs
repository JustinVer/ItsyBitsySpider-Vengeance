using UnityEngine;

public interface ICollisionReciever
{
    public void CollisionSignal(Collision collision);
    public void TriggerSignal(Collider collision);
}
