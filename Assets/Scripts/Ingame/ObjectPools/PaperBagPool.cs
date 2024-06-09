using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperBagPool : ObjectPool
{
    public PaperBag Spawn(Vector3 pos, Quaternion rot, Action<PaperBag> onSpawn)
    {
        var customer = pool.Spawn<PaperBag>(pos, rot, null, true);
        onSpawn?.Invoke(customer);
        return customer;
    }

    public void Despawn(PaperBag target)
    {
        pool.Despawn(target.gameObject);
    }
}
