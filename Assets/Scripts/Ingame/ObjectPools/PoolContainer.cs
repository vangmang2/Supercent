using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectPoolType
{
    croassant,
}

public class PoolContainer : MonoBehaviour
{
    public static PoolContainer instance { get; private set; }

    Dictionary<ObjectPoolType, ObjectPool> poolDic = new Dictionary<ObjectPoolType, ObjectPool>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        var pools = GetComponentsInChildren<ObjectPool>();
        foreach (var pool in pools)
        {
            poolDic.Add(pool.poolType, pool);
        }
    }

    public T GetPool<T>(ObjectPoolType poolType) where T : ObjectPool
    {
        return poolDic[poolType] as T;
    }
}

public abstract class ObjectPool : MonoBehaviour
{
    public abstract ObjectPoolType poolType { get; }
}
