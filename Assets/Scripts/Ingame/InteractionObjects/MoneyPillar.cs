using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPillar : InteractionObject
{
    public override InteractionObjectType interactionObjectType => throw new System.NotImplementedException();
    public MoneyStackPool moneyStackPool => PoolContainer.instance.GetPool<MoneyStackPool>();

    Stack<MoneyStack> moneyStacks = new Stack<MoneyStack>();
    Stack<int> moneyAmountStack = new Stack<int>();

    const float stackGap = 0.27f;

    int stackCount;

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
        moneyAmountStack.Push(amount);
    }

    public override Vector3 GetPos(int index)
    {
        return Vector3.zero;
    }

    Player player;
    public override void OnInteractantEnter(Interactant interactant)
    {
        player = interactant as Player;
    }

    public override void OnInteractantExit(Interactant interactant)
    {
        player = null;
    }

    float cooldown;
    private void Update()
    {
        cooldown += Time.deltaTime;
        if (cooldown >= 0.15f)
        {
            if (player == null)
                return;

            if (moneyStacks.Count <= 0)
                return;

            var moneyStack = moneyStacks.Pop();
            var moneyAmount = moneyAmountStack.Pop();
            stackCount--;
            moneyStack.PlayMoneyToPlayerAnim(player.position);
            player.IncreaseMoney(moneyAmount);
            cooldown = 0f;
        }
    }
}
