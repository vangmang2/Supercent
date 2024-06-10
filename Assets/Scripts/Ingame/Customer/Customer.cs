using UnityEngine;
using UnityEngine.AI;
using Lean.Pool;
using Random = UnityEngine.Random;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Sirenix.Utilities;

public enum Needs
{
    croassant,
    pos,
    table
}

public struct CurrentNeeds
{
    public Needs needs;
    public int targetValue;
    public int currentValue;
    public bool isGoingToTable;
}

public class Customer : Interactant, IPoolable
{
    static int TableingCount = 0;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;

    public CurrentNeeds currentNeeds;

    public Vector3 destination => agent.destination;
    public Vector3 up => transform.up;

    HUDCustomerStates customerNeeds;
    Action<Customer> onMoveEnd;
    Action<Customer> onWaitForAction;
    public override InteractantType interactantType => InteractantType.getter;
    bool isSitting;
    CancellationTokenSource cts;

    void OnPushCroassant(Interactant interactant)
    {
        var customer = interactant as Customer;
        customer.currentNeeds.currentValue++;
        customer.ShowNeeds(true);
    }

    public Customer SetEnableIsSitting(bool enable)
    {
        isSitting = enable;
        return this;
    }

    public Customer SetActionOnMoveEnd(Action<Customer> callback)
    {
        onMoveEnd = callback;
        return this;
    }

    public Customer SetActionOnWaitForAction(Action<Customer> callback)
    {
        onWaitForAction = callback;
        return this;
    }

    public Customer SetAgentAvoidQuality(ObstacleAvoidanceType type)
    {
        agent.obstacleAvoidanceType = type;
        return this;
    }

    public Customer SetActiveAgent(bool enable)
    {
        agent.enabled = enable;
        return this;
    }

    public Customer SetPosition(Vector3 position)
    {
        transform.position = position;
        return this;
    }

    public Customer SetRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
        return this;
    }

    public void PlayAction(Action<Customer> callback, float delay)
    {
        InvokePlayAction(callback, delay).Forget();
    }

    async UniTaskVoid InvokePlayAction(Action<Customer> callback, float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        callback?.Invoke(this);
    }

    public Customer WaitForAction_Until(Func<Customer, bool> predicate)
    {
        InvokeWaitForAction_Until(predicate).Forget();
        return this;
    }

    async UniTaskVoid InvokeWaitForAction_Until(Func<Customer, bool> predicate)
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        await UniTask.WaitUntil(() => (bool)predicate?.Invoke(this), cancellationToken: cts.Token);
        onWaitForAction?.Invoke(this);
    }

    public void OnSpawn()
    {
        isSitting = false;
        onMoveEnd = null;
        onWaitForAction = null;

        customerNeeds = CustomerStatesManager.instance.Spawn(transform);
        customerNeeds.SetActive(false);

        SetActionOnPushCarriableObject(OnPushCroassant);
        SetAgentAvoidQuality(ObstacleAvoidanceType.HighQualityObstacleAvoidance);
    }

    public void OnDespawn()
    {
        CustomerStatesManager.instance.Despawn(customerNeeds);

        // 종이 가방도 디스폰해줘야 함
        if (currentNeeds.isGoingToTable)
            return;
        var bag = coStack.Pop() as PaperBag;
        PoolContainer.instance.GetPool<PaperBagPool>().Despawn(bag);
    }

    public void ForEachCOStacks(Action<CarriableObject, int> callback)
    {
        int index = 0;
        foreach (var co in coStack)
        {
            callback?.Invoke(co, index);
            index++;
        }
    }

    private void Update()
    {
        animator.SetBool("Move", agent.velocity.magnitude > 0.1f);
        animator.SetBool("IsHoldingSomething", coStack.Count > 0);
        animator.SetBool("IsSitting", isSitting);
    }

    public Customer SetNeedsToCroassant()
    {
        currentNeeds.needs = Needs.croassant;
        currentNeeds.targetValue = Random.Range(1, 4);
        currentNeeds.currentValue = 0;

        currentNeeds.isGoingToTable = TableingCount == 2;

        if (TableingCount >= 2)
        {
            TableingCount = 0;
        }
        TableingCount++;

        SetMaxStackCount(currentNeeds.targetValue);
        return this;
    }

    public Customer SetNeedsToPos()
    {
        currentNeeds.needs = Needs.pos;
        return this;
    }

    public Customer SetNeedsToTable()
    {
        currentNeeds.needs = Needs.table;
        return this;
    }

    public Customer ShowNeeds(bool showValue)
    {
        customerNeeds.SetActiveNeeds(true)
                     .SetActiveHappyFace(false)
                     .SetNeedsSprite(currentNeeds.needs)
                     .SetValueText((currentNeeds.targetValue - currentNeeds.currentValue).ToString())
                     .SetActiveValue(showValue)
                     .SetActive(true);
        return this;
    }

    public Customer ShowHappyFace()
    {
        customerNeeds.SetActiveHappyFace(true)
                     .SetActiveNeeds(false);
        return this;
    }

    public Customer SetActiveNeeds(bool enable)
    {
        customerNeeds.SetActive(enable);
        return this;
    }

    public Customer MoveToTarget(Vector3 target)
    {
        agent.SetDestination(target);
        InvokeMoveToTarget().Forget();
        return this;
    }

    async UniTaskVoid InvokeMoveToTarget()
    {
        await UniTask.DelayFrame(1);

        await UniTask.WaitUntil(() => (transform.position - agent.destination).magnitude <= 0.1f);
        onMoveEnd?.Invoke(this);

    }

    public void RotateTo(Quaternion rot, float duration)
    {
        transform.DORotateQuaternion(rot, duration);
    }

    public Customer SetDestination(Vector3 target)
    {
        agent.SetDestination(target);
        return this;
    }

    void OnDestroy()
    {
        cts?.Cancel();
    }
}
