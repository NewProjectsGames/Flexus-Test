using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefabToPool;
    public int poolSize = 10;
    private List<GameObject> objectPool;


    private void Start()
    {
        objectPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefabToPool);
            obj.SetActive(false);
            if (obj.TryGetComponent(out ReturnPool returnPool))
            {
                returnPool.objectPool = this;
            }

            objectPool.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < objectPool.Count; i++)
        {
            if (!objectPool[i].activeInHierarchy)
            {
                return objectPool[i];
            }
        }

        return objectPool[0];
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
    }
}