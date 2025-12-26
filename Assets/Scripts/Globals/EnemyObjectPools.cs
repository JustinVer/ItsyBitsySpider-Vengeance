using UnityEngine;

public class EnemyObjectPools : MonoBehaviour
{
    #region Singleton instance
    private static EnemyObjectPools instance;

    public static EnemyObjectPools Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<EnemyObjectPools>();
            }
            if (instance == null)
            {
                Debug.LogWarning("NO ENEMY OBJECT POOL FOUND");
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    #endregion
    public GameObject prefab;

    private ObjectPool<Transform> pool;

    public ObjectPool<Transform> Pool
    {
        get
        {
            if (pool == null)
            {
                pool = new ObjectPool<Transform>(prefab, 10);
            }
            return pool;
        }
    }

    private void Awake()
    {
        if (pool == null)
        {
            pool = new ObjectPool<Transform>(prefab);
        }
    }
}
