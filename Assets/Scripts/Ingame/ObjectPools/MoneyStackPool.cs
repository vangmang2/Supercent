using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyStackPool : ObjectPool
{
    public MoneyStack Spawn(Vector3 pos, Quaternion rot, Action<MoneyStack> onSpawn)
    {
        var moneyStack = pool.Spawn<MoneyStack>(pos, rot, null, true);
        onSpawn?.Invoke(moneyStack);
        return moneyStack;
    }

    public void Despawn(MoneyStack moneyStack)
    {
        pool.Despawn(moneyStack.gameObject);
    }
}
