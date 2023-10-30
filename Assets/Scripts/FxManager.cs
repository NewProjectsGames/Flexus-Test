using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FxManager : MonoBehaviour
{
    public static FxManager Instance;
    [SerializeField] private ObjectPool explosionFxPool;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Explosion(Vector3 _position, Vector3 _normal)
    {
        GameObject fx = explosionFxPool.GetPooledObject();
        fx.SetActive(false);
        fx.transform.position = _position;
        fx.transform.rotation = Quaternion.LookRotation(_normal);
        fx.SetActive(true);
    }
}