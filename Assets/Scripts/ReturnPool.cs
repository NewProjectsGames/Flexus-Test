using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnPool : MonoBehaviour
{
    public ObjectPool objectPool;

    public ObjectPool Pool
    {
        private get => objectPool;
        set => objectPool = value;
    }

    private void OnDisable()
    {
        Pool.ReturnToPool(gameObject);
    }
}