using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPillar : InteractionObject
{
    public override InteractionObjectType interactionObjectType => throw new System.NotImplementedException();
    public MoneyStackPool moneyStackPool => PoolContainer.instance.GetPool<MoneyStackPool>();

    Stack<MoneyStack> moneyStacks = new Stack<MoneyStack>();

    const float stackGap = 0.27f;

    int stackCount;
    int moneyAmount;

    public void SpawnMoneyStack()
    {
        var targetPos = transform.position + new Vector3(0f, stackCount * stackGap, 0f);
        var moneyStack = moneyStackPool.Spawn(targetPos, Quaternion.identity, null);

        stackCount++;
        moneyStacks.Push(moneyStack);
    }

    public void DespawnMoneyStack()
    {
        if (moneyStacks.Count <= 0)
            return;
        var moneyStack = moneyStacks.Pop();
        moneyStackPool.Despawn(moneyStack);
    }

    public void IncreaseMoney(int amount)
    {
        moneyAmount += amount;
    }

    public void DecreaseMoney(int amount)
    {
        moneyAmount -= amount;
    }

    public override Vector3 GetPos(int index)
    {
        return Vector3.zero;
    }

    Player player;
    public override void OnInteractantEnter(Interactant interactant)
    {

    }

    public override void OnInteractantExit(Interactant interactant)
    {
    }
}
