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

// �մ� �ൿ ����
// 1. ����
// 2. ũ�ξƻ� �ٱ��Ϸ� �̵�
// 3. ũ�ξƻ��� ���� �� ���� ���
// 4. ũ�ξƻ��� �ް� ������� �̵�, �մ� ���������� ����ũ �ƿ�, ����Ļ� ����
// 5. ���
// 6. ����ũ �ƿ��� ��� �Ա��� �̵�, ����
// 7. ���� �Ļ��� ��� ���̺�� �̵�
// 8. ���̺��� �Ļ�
// 9. ���̺� �Ļ��� �Ա��� �̵�, ����


public class CustomerManager : MonoBehaviour
{
    const int maxCroassantWaitingCount = 3; // ũ�ξƻ� �ٱ��Ͽ� ����� �� �ִ� �ο�.
    const int maxCustomerCount = 6;         // ���忡 ������ �� �ִ� �մ� ��

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
            // 1. ���̺��� ����Ǿ� ������ �ٷ� ���̺�� �̵�������� �Ѵ�.
            // 2. ���̺� ������ �ְų� ġ������ ���� ���¸� �ϴ� ������� �;� �Ѵ�.
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

        // �մ� ����
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
