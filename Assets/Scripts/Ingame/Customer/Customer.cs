using UnityEngine;
using UnityEngine.AI;
using Lean.Pool;
using Random = UnityEngine.Random;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using System.Collections.Generic;

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
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;

    public CurrentNeeds currentNeeds;

    public Vector3 position => transform.position;
    public Vector3 up => transform.up;

    HUDCustomerNeeds customerNeeds;
    Action<Customer> onMoveEnd;
    Action<Customer> onWaitForAction;
    public CustomerStep customerStep { get; private set; } // ���ܿ� ���� �մ��� �ൿ�� �޶�����
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

    public void SetActionOnMoveEnd(Action<Customer> callback)
    {
        onMoveEnd = callback;
    }

    public void SetActionOnWaitForAction(Action<Customer> callback)
    {
        onWaitForAction = callback;
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
        SetMaxStackCount(currentNeeds.targetValue);
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


    public Customer MoveToCroassant(Vector3 target)
    {
        MoveToTarget(target);
        return this;
    }

    public void MoveToTarget(Vector3 target)
    {
        agent.SetDestination(target);
        InvokeMoveToTarget().Forget();
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