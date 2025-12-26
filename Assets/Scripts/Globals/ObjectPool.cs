using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool<T> where T : Component
{
    public int defaultSize;
    public int maxSize;

    protected IObjectPool<T> object_Pool;
    private GameObject emptyParent;

    [SerializeField] private GameObject objectPrefab;

    public IObjectPool<T> Pool
    {
        get
        {
            if (object_Pool == null)
            {
                object_Pool = new UnityEngine.Pool.ObjectPool<T>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, defaultSize, maxSize);
            }
            return object_Pool;
        }
    }

    public ObjectPool(GameObject objectPrefab, int defaultSize = 10, int maxSize = 2000)
    {
        this.defaultSize = defaultSize;
        this.objectPrefab = objectPrefab;
        this.maxSize = maxSize;
        object_Pool = new UnityEngine.Pool.ObjectPool<T>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, this.defaultSize, this.maxSize);
        T[] temp = new T[defaultSize];
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = Get();
        }
        for (int i = 0; i < temp.Length; i++)
        {
            Return(temp[i]);
        }
    }


    T CreatePooledItem()
    {
        Debug.Log("Created an item");
        objectPrefab.gameObject.SetActive(false);


        GameObject gameObject = Object.Instantiate(objectPrefab);
        T item = gameObject.GetComponent<T>();
        if (item == null)
        {
            throw new System.Exception(gameObject.name + " does not contain " + item.GetType().Name);
        }
        if (item is IReturnSelfObject<T> returnSelfObject)
        {
            returnSelfObject.SetParentPool(this);
        }

        if (emptyParent == null)
        {
            emptyParent = new GameObject(item.GetType().Name);
        }
        gameObject.transform.parent = emptyParent.transform;

        return item;
    }

    // Called when an item is returned to the pool using Release
    void OnReturnedToPool(T obj)
    {
        obj.gameObject.SetActive(false);
    }

    // Called when an item is taken from the pool using Get
    void OnTakeFromPool(T obj)
    {
        obj.gameObject.SetActive(true);
    }

    // If the pool capacity is reached then any items returned will be destroyed.
    // We can control what the destroy behavior does, here we destroy the GameObject.
    void OnDestroyPoolObject(T obj)
    {
        Object.Destroy(obj.gameObject);
    }

    public T Get()
    {
        return object_Pool.Get();
    }

    public void Return(T Object)
    {
        object_Pool.Release(Object);
    }
}
