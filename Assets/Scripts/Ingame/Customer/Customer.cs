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

public enum CustomerStep
{
    waitForAction,
    moveToBasket,
    moveToPos,
    moveToTable,
    moveToEntrance,
}

public class Customer : Interactant, IPoolable
{
    static int TableingCount = 0;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;

    public CurrentNeeds currentNeeds;

    public Vector3 position => transform.position;
    public Vector3 destination => agent.destination;
    public Vector3 up => transform.up;

    HUDCustomerNeeds customerNeeds;
    Action<Customer> onMoveEnd;
    Action<Customer> onWaitForAction;
    public CustomerStep customerStep { get; private set; } // 스텝에 따라 손님의 행동이 달라진다
    public override InteractantType interactantType => InteractantType.getter;

    CancellationTokenSource cts;

    void Start()
    {
        SetActionOnPushCroassant(OnPushCroassant);
    }

    void OnPushCroassant(Interactant interactant)
    {
        var customer = interactant as Customer;
        customer.currentNeeds.currentValue++;
        customer.ShowNeeds(true);
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

    public void WaitForAction_Until(Func<Customer, bool> predicate)
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        InvokeWaitForAction_Until(predicate).Forget();
    }

    async UniTaskVoid InvokeWaitForAction_Until(Func<Customer, bool> predicate)
    {
        await UniTask.WaitUntil(() => (bool)predicate?.Invoke(this), cancellationToken: cts.Token);
        onWaitForAction?.Invoke(this);
    }

    public void OnSpawn()
    {
        customerNeeds = CustomerNeedsManager.instance.Spawn(transform);
        customerNeeds.SetActive(false);
    }


    private void Update()
    {
        animator.SetBool("Move", agent.velocity.magnitude > 0.1f);
        animator.SetBool("IsHoldingSomething", croassants.Count > 0);
    }

    public Customer SetNeedsToCroassant()
    {
        currentNeeds.needs = Needs.croassant;
        currentNeeds.targetValue = Random.Range(2, 4);
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

    public Customer ShowNeeds(bool showValue)
    {
        customerNeeds.SetNeedsSprite(currentNeeds.needs)
                     .SetValueText((currentNeeds.targetValue - currentNeeds.currentValue).ToString())
                     .SetActiveValue(showValue)
                     .SetActive(true);
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
        await UniTask.WaitUntil(() => agent.remainingDistance <= 0.05f);
        onMoveEnd?.Invoke(this);

    }

    public void RotateTo(Quaternion rot, float duration)
    {
        transform.DORotateQuaternion(rot, duration);
    }

    public void DespawnHUDNeeds()
    {
        CustomerNeedsManager.instance.Despawn(customerNeeds);
    }

    public void OnDespawn()
    {
        SetAgentActive(false);
    }

    public Customer SetDestination(Vector3 target)
    {
        agent.SetDestination(target);
        return this;
    }

    public Customer SetAgentActive(bool enable)
    {
        agent.enabled = enable;
        return this;
    }
}
