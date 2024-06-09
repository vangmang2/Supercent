using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
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
    const int maxCustomerCount = 3; // ũ�ξƻ� �ٱ��Ͽ� ����� �� �ִ� �ο�.
                                    // TODO: �� �ο��� �ƴ����� ������ �̸��� �������� �ʾƼ� �ϴ� maxCustomerCount�� ���. ���߿� ���� �̸� �������� �ٲ���

    [SerializeField] InteractionObjectManager ioManager;
    CustomerPool pool => PoolContainer.instance.GetPool<CustomerPool>();
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
        await UniTask.WaitUntil(() => currCustomerCount < maxCustomerCount);
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));

        var customer = pool.Spawn(new Vector3(0f, 0.916666746f, 15.666667f), Quaternion.Euler(0f, 180f, 0f), null);
        customer.MoveToTarget(ioManager.GetPos(InteractionObjectType.basket, rotationCount))
                .SetActionOnMoveEnd(OnMoveToCroassantEnd);
        currCustomerCount++;
        rotationCount++;

        if (rotationCount >= maxCustomerCount)
            rotationCount = 0;

        InvokeCustomerSpawn().Forget();
    }

    void OnMoveToCroassantEnd(Customer customer)
    {
        var rot = Quaternion.LookRotation(ioManager.GetPos(InteractionObjectType.basket) - customer.position, customer.up);
        rot.x = 0f;
        rot.z = 0f;
        customer.RotateTo(rot, 0.2f);

        customer.SetNeedsToCroassant()
                .ShowNeeds(true);

        customer.SetActionOnWaitForAction(OnCroassantReady)
                .WaitForAction_Until(IsCroassantReady);

        var basekt = ioManager.GetInteractionObject<Basket>(InteractionObjectType.basket);
        basekt.AddInteractant(customer);
    }

    bool IsCroassantReady(Customer customer)
    {
        return customer.currentNeeds.currentValue >= customer.currentNeeds.targetValue;
    }

    void OnCroassantReady(Customer customer)
    {
        var basekt = ioManager.GetInteractionObject<Basket>(InteractionObjectType.basket);
        basekt.RemoveInteractant(customer);

        if (customer.currentNeeds.isGoingToTable)
        {
            // 1. ���̺��� ����Ǿ� ������ �ٷ� ���̺�� �̵�������� �Ѵ�.
            // 2. ���̺� ������ �ְų� ġ������ ���� ���¸� �ϴ� ������� �;� �Ѵ�.
        }
        else
        {
            customer.SetNeedsToPos()
                    .ShowNeeds(false);
            var pos = ioManager.GetInteractionObject<Pos>(InteractionObjectType.pos);

            customer.MoveToTarget(pos.paymentWatingStartPos + new Vector3(0f, 0f, pos.lineGap * pos.currPaymentWaitingCount))    
                    .SetActionOnMoveEnd(OnMoveToPosEnd);

            pos.SetActionOnPay(OnPay)
               .IncreasePaymentWaitingCount()
               .EnqueueCustomer(customer);

            currCustomerCount--;
        }
    }

    void OnMoveToPosEnd(Customer customer)
    {
        var rot = Quaternion.Euler(0f, 180f, 0f);
        customer.RotateTo(rot, 0.2f);
    }

    void OnPay(Pos pos, Customer customer, PaperBag bag)
    {
        InvokePayment(pos, customer, bag).Forget();
    }

    async UniTaskVoid InvokePayment(Pos pos,Customer customer, PaperBag bag)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.15f));
        while (customer.currCOCount > 0)
        {
            var croassant = customer.PopCarriableObject();
            croassant.SetParent(bag.transform)
                     .MoveToTargetWithCurve(bag.localPosition, 0.2f, height: 3f, onComplete: (croassant) => croassant.SetActive(false));
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        }

        await UniTask.Delay(TimeSpan.FromSeconds(0.15f));
        bag.PlayCloseAnim();

        // �մ� ����
        pos.DequeueCustomer();
        pos.PullCustomersLine();

        customer.SetActionOnPushCarriableObject(null);
        customer.PushCarriableObject(bag);

        await UniTask.Delay(TimeSpan.FromSeconds(0.4f));
        // TODO: �ູ �̸�Ƽ�� ����
        bag.SetParent(customer.transform);
        bag.MoveToTargetWithCurve(new Vector3(0f, 0.819999993f, 0.730000019f), 0.2f, targetRot: 0f, onComplete: (carriableObject) =>
        {
            customer.ShowHappyFace()
                    .MoveToTarget(new Vector3(-0.360000014f, 0.50999999f, 14.1199999f))
                    .SetActionOnMoveEnd(DespawnCustomer);

            void DespawnCustomer(Customer customer)
            {
                pool.Despawn(customer);
            }
        });
    }



    private void OnDestroy()
    {
        cts.Cancel();
    }
}
