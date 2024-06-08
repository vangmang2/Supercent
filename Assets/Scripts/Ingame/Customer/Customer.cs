using UnityEngine;
using UnityEngine.AI;
using Lean.Pool;
using Random = UnityEngine.Random;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public enum Needs
{
    croassant,
    pos,
    table
}

public struct CurrentNeeds
{
    public Needs needs;
    public int value;
}

public enum CustomerStep
{
    waitForAction,
    moveToBasket,
    moveToPos,
    moveToTable,
    moveToEntrance,
}

public class Customer : MonoBehaviour, IPoolable
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;

    public Vector3 position => transform.position;
    public Vector3 up => transform.up;
    HUDCustomerNeeds customerNeeds;
    CurrentNeeds currentNeeds;

    Action<Customer> onMoveEnd;

    public CustomerStep customerStep { get; private set; } // 스텝에 따라 손님의 행동이 달라진다

    public void SetActionOnMoveEnd(Action<Customer> callback)
    {
        onMoveEnd = callback;
    }

    public void OnSpawn()
    {
        customerNeeds = CustomerNeedsManager.instance.Spawn(transform);
        customerNeeds.SetActive(false);
    }


    private void Update()
    {
        animator.SetBool("Move", agent.velocity.magnitude > 1f);
    }


    public Customer SetRandomNeeds()
    {
        currentNeeds.needs = Needs.croassant;
        currentNeeds.value = Random.Range(2, 4);
        return this;
    }

    public Customer ShowNeeds(bool showValue)
    {
        customerNeeds.SetNeedsSprite(currentNeeds.needs)
                     .SetValueText(currentNeeds.value.ToString())
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
