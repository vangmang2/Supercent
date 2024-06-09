using Lean.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolContainer : MonoBehaviour
{
    public static PoolContainer instance { get; private set; }

    Dictionary<Type, ObjectPool> poolDic = new Dictionary<Type, ObjectPool>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        var pools = GetComponentsInChildren<ObjectPool>();
        foreach (var pool in pools)
        {
            poolDic.Add(pool.GetType(), pool);
        }
    }

    public T GetPool<T>() where T : ObjectPool
    {
        return poolDic[typeof(T)] as T;
    }
}

[RequireComponent(typeof(LeanGameObjectPool))]
public abstract class ObjectPool : MonoBehaviour
{
    protected LeanGameObjectPool pool;
    //public abstract ObjectPoolType poolType { get; }

    private void Start()
    {
        pool = GetComponent<LeanGameObjectPool>();
    }
}
