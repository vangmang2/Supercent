
using System;
using UnityEngine;

public class CroassantPool : ObjectPool
{
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
