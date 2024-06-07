using Lean.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LeanGameObjectPool))]
public class CroassantPool : ObjectPool
{
    LeanGameObjectPool pool;

    private void Start()
    {
        pool = GetComponent<LeanGameObjectPool>();
    }

    public override ObjectPoolType poolType => ObjectPoolType.croassant;

    public Croassant Spawn(Vector3 pos, Quaternion rot, Action<Croassant> onSpawn)
    {
        var croassant = pool.Spawn<Croassant>(pos, rot, null, true);
        onSpawn?.Invoke(croassant);
        return croassant;
    }

    public void Despawn(Croassant croassant)
    {
        pool.Despawn(croassant.gameObject);
    }
}
