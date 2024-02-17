using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObjects<T> where T : Component
{
    Dictionary<T, List<T>> poolDict;
    
    public PooledObjects()
    {
        poolDict = new Dictionary<T, List<T>>();
    }

    public void EnsureQuantity(T prototype, int count)
    {
        var pool = GetOrCreatePool(prototype);
        int prevCount = pool.Count;
        for (int i = 0; i < count - prevCount; i++) {
            T newObj = Object.Instantiate(prototype);
            newObj.gameObject.SetActive(false);
            Object.DontDestroyOnLoad(newObj);
            pool.Add(newObj);
        }
    }

    public T GetPooled(T prototype, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        var pool = GetOrCreatePool(prototype);
        T unused = pool.Find(x => !x.gameObject.activeSelf);
        if (!unused) {
            unused = Object.Instantiate(prototype, position, rotation, parent);
            pool.Add(unused);
            Object.DontDestroyOnLoad(unused);
        } else {
            unused.transform.position = position;
            unused.transform.rotation = rotation;
            if (unused.transform.parent != parent)
                unused.transform.parent = parent;
            unused.gameObject.SetActive(true);
        }
        return unused;
    }

    List<T> GetOrCreatePool(T prototype)
    {
        if (!poolDict.TryGetValue(prototype, out var pool))
        {
            pool = new List<T>();
            poolDict[prototype] = pool;
        }
        return pool;
    }
}
