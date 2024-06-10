using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Video;

// 손님 행동 로직
// 1. 입장
// 2. 크로아상 바구니로 이동
// 3. 크로아상이 나올 때 까지 대기
// 4. 크로아상을 받고 포스기로 이동, 손님 내부적으로 테이크 아웃, 매장식사 결정
// 5. 계산
// 6. 테이크 아웃일 경우 입구로 이동, 디스폰
// 7. 매장 식사의 경우 테이블로 이동
// 8. 테이블에서 식사
// 9. 테이블 식사후 입구로 이동, 디스폰


public class CustomerManager : MonoBehaviour
{
    const int maxCroassantWaitingCount = 3; // 크로아상 바구니에 대기할 수 있는 인원.
    const int maxCustomerCount = 6;         // 매장에 존재할 수 있는 손님 수

    [SerializeField] InteractionObjectManager ioManager;
    CustomerPool customerPool => PoolContainer.instance.GetPool<CustomerPool>();
    CroassantPool croassantPool => PoolContainer.instance.GetPool<CroassantPool>();
    int currCroassantWaitingCount;
    int currCustomerCount;
    int rotationCount;
    CancellationTokenSource cts;

    private void Start()
    {
        cts = new CancellationTokenSource();
        InvokeCustomerSpawn().Forget();
    }

    async UniTaskVoid InvokeCustomerSpawn()
    {
        await UniTask.WaitUntil(() => currCroassantWaitingCount < maxCroassantWaitingCount && currCustomerCount < maxCustomerCount);
        await UniTask.Delay(TimeSpan.FromSeconds(2f));

        var customer = customerPool.Spawn(new Vector3(0f, 0.916666746f, 15.666667f), Quaternion.Euler(0f, 180f, 0f), null);
        customer.MoveToTarget(ioManager.GetPos<Basket>(rotationCount))
                .SetActionOnMoveEnd(OnMoveToCroassantEnd);
        currCroassantWaitingCount++;
        currCustomerCount++;
        rotationCount++;

        if (rotationCount >= maxCroassantWaitingCount)
            rotationCount = 0;

        InvokeCustomerSpawn().Forget();
    }

    void OnMoveToCroassantEnd(Customer customer)
    {
        var rot = Quaternion.LookRotation(ioManager.GetPos<Basket>() - customer.position, customer.up);
        rot.x = 0f;
        rot.z = 0f;
        customer.RotateTo(rot, 0.2f);

        customer.SetNeedsToCroassant()
                .ShowNeeds(true);

        customer.SetActionOnWaitForAction(OnCroassantReady)
                .WaitForAction_Until(IsCroassantReady);

        var basekt = ioManager.GetInteractionObject<Basket>();
        basekt.AddInteractant(customer);
    }

    bool IsCroassantReady(Customer customer)
    {
        return customer.currentNeeds.currentValue >= customer.currentNeeds.targetValue;
    }

    void OnCroassantReady(Customer customer)
    {
        customer.SetAgentAvoidQuality(ObstacleAvoidanceType.NoObstacleAvoidance);
        currCroassantWaitingCount--;

        var basekt = ioManager.GetInteractionObject<Basket>();
        basekt.RemoveInteractant(customer);

        if (customer.currentNeeds.isGoingToTable)
        {
            // 1. 테이블이 언락되어 있으면 바로 테이블로 이동시켜줘야 한다.
            // 2. 테이블에 누군가 있거나 치워지지 않은 상태면 일단 포스기로 와야 한다.
            var table = ioManager.GetInteractionObject<Table>();
            table.EnqueueCustomer(customer);

            if (table.isUnlocked && 
                table.isClean &&
                table.nextCustomer.Equals(customer))
            {
                GoToTable(customer);
            }
            else
            {
                customer.SetNeedsToPos()
                        .ShowNeeds(false);
                var pos = ioManager.GetInteractionObject<Pos>();

                customer.MoveToTarget(pos.tableWatingStartPos + new Vector3(0f, 0f, pos.lineGap * pos.currTableWaitingCount))
                        .SetActionOnMoveEnd((customer) =>
                        {
                            OnMoveToPosEnd(customer);
                            customer.SetNeedsToTable()
                                    .ShowNeeds(false);
                        });

                pos.EnqueueTableWaitingCustomer(customer);
                customer.WaitForAction_Until(IsTableReady)
                        .SetActionOnWaitForAction(GoToTableWithUpdateLine);
            }
        }
        else
        {
            customer.SetNeedsToPos()
                    .ShowNeeds(false);
            var pos = ioManager.GetInteractionObject<Pos>();

            customer.MoveToTarget(pos.paymentWatingStartPos + new Vector3(0f, 0f, pos.lineGap * pos.currPaymentWaitingCount))    
                    .SetActionOnMoveEnd(OnMoveToPosEnd);

            pos.SetActionOnPay(OnPayToPos)
               .IncreasePaymentWaitingCount()
               .EnqueueCustomer(customer);
        }
    }

    bool IsTableReady(Customer customer)
    {
        var table = ioManager.GetInteractionObject<Table>();
        return table.isUnlocked && table.isClean && table.nextCustomer.Equals(customer);
    }

    void GoToTableWithUpdateLine(Customer customer)
    {
        GoToTable(customer);

        var pos = ioManager.GetInteractionObject<Pos>();
        pos.DequeueTableWaitingCustomer()
           .UpdateTableWaitingCustomerLine();
    }

    void GoToTable(Customer customer)
    {
        customer.SetNeedsToTable()
                .ShowNeeds(false)
                .MoveToTarget(new Vector3(5.55000019f, 0.479999989f, 7.05000019f))
                .SetActionOnMoveEnd(StartMeal);
    }

    void StartMeal(Customer customer)
    {
        customer.SetActiveNeeds(false)
                .SetActiveAgent(false)
                .SetPosition(new Vector3(6.71899986f, 1.05999994f, 7.07499981f))
                .SetRotation(Quaternion.Euler(0f, 180f, 0f))
                .SetEnableIsSitting(true);

        customer.ForEachCOStacks((croassant, index) =>
        {
            croassant.SetRotation(Quaternion.Euler(0f, 90f, 0f))
                     .SetPosition(new Vector3(6.67999983f, 1.77999997f, 6.02099991f) + new Vector3(0f, 0.27f * index, 0f));
        });

        customer.PlayAction(FinishMeal, 3f);
    }

    void FinishMeal(Customer customer)
    {
        var table = ioManager.GetInteractionObject<Table>();
        table.Dequeue();

        currCustomerCount--;

        var croassantCount = customer.currCOCount;
        while (customer.currCOCount > 0)
        {
            var croassant = customer.PopCarriableObject(SoundType.none);
            croassantPool.Despawn(croassant as Croassant);
        }

        customer.SetActiveNeeds(true)
                .SetActiveAgent(true)
                .SetEnableIsSitting(false)
                .ShowHappyFace()
                .MoveToTarget(new Vector3(0.310000002f, 0.50999999f, 9.71000004f))
                .SetActionOnMoveEnd(DespawnCustomer);

        table.IncreaseMoney(5, croassantCount)
             .SetActiveTrash(true);
    }

    void OnMoveToPosEnd(Customer customer)
    {
        var rot = Quaternion.Euler(0f, 180f, 0f);
        customer.RotateTo(rot, 0.2f);
    }

    void OnPayToPos(Pos pos, Customer customer, PaperBag bag)
    {
        customer.InstancePaymentCts();
        InvokePayment(pos, customer, bag).Forget();
    }

    async UniTaskVoid InvokePayment(Pos pos,Customer customer, PaperBag bag)
    {
        var croassantCount = customer.currCOCount;
        await UniTask.Delay(TimeSpan.FromSeconds(0.15f), cancellationToken: customer.paymentCts.Token);
        while (customer.currCOCount > 0)
        {
            var croassant = customer.PopCarriableObject();
            croassant.SetParent(bag.transform)
                     .MoveToTargetWithCurve(bag.localPosition, 0.2f, height: 3f, onComplete: (croassant) => croassantPool.Despawn(croassant as Croassant));
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: customer.paymentCts.Token);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(0.15f), cancellationToken: customer.paymentCts.Token);
        bag.PlayCloseAnim();

        // 손님 퇴장
        pos.DequeueCustomer();
        pos.PullCustomersLine();

        customer.SetActionOnPushCarriableObject(null);
        customer.PushCarriableObject(bag);

        await UniTask.Delay(TimeSpan.FromSeconds(0.4f), cancellationToken: customer.paymentCts.Token);

        bag.SetParent(customer.transform);
        bag.MoveToTargetWithCurve(new Vector3(0f, 0.819999993f, 0.730000019f), 0.2f, targetRot: 0f, onComplete: (carriableObject) =>
        {
            currCustomerCount--;
            customer.ShowHappyFace()
                    .MoveToTarget(new Vector3(0.310000002f, 0.50999999f, 9.71000004f))
                    .SetActionOnMoveEnd(DespawnCustomer);

            pos.IncreaseMoney(5, croassantCount);
        });
    }

    void DespawnCustomer(Customer customer)
    {
        customerPool.Despawn(customer);
    }


    private void OnDestroy()
    {
        cts.Cancel();
    }
}
