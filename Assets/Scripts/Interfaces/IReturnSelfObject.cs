using UnityEngine;

public interface IReturnSelfObject<T> where T : Component
{
    public void SetParentPool(ObjectPool<T> parentPool);
    protected void ReturnSelf();
}
