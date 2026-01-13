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

    public GameObject antPrefab;
    private ObjectPool<Transform> antPool;

    public GameObject pillPrefab;
    private ObjectPool<Transform> pillPool;

    public GameObject beetlePrefab;
    private ObjectPool<Transform> beetlePool;

    public ObjectPool<Transform> AntPool
    {
        get
        {
            if (antPool == null)
            {
                antPool = new ObjectPool<Transform>(antPrefab, 10);
            }
            return antPool;
        }
    }
    public ObjectPool<Transform> PillPool
    {
        get
        {
            if (pillPool == null)
            {
                pillPool = new ObjectPool<Transform>(pillPrefab, 10);
            }
            return pillPool;
        }
    }
    public ObjectPool<Transform> BeetlePool
    {
        get
        {
            if (beetlePool == null)
            {
                beetlePool = new ObjectPool<Transform>(beetlePrefab, 10);
            }
            return beetlePool;
        }
    }

    private void Awake()
    {
        if (antPool == null)
        {
            antPool = new ObjectPool<Transform>(antPrefab);
        }
        if (pillPool == null)
        {
            pillPool = new ObjectPool<Transform>(pillPrefab);
        }
        if (beetlePool == null)
        {
            beetlePool = new ObjectPool<Transform>(beetlePrefab);
        }
    }
}
