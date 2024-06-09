using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
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
    const int maxCustomerCount = 3; // 크로아상 바구니에 대기할 수 있는 인원.
                                    // TODO: 총 인원이 아니지만 마땅한 이름이 생각나지 않아서 일단 maxCustomerCount로 명명. 나중에 좋은 이름 생각나면 바꾸자

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
            // 1. 테이블이 언락되어 있으면 바로 테이블로 이동시켜줘야 한다.
            // 2. 테이블에 누군가 있거나 치워지지 않은 상태면 일단 포스기로 와야 한다.
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

        // 손님 퇴장
        pos.DequeueCustomer();
        pos.PullCustomersLine();

        customer.SetActionOnPushCarriableObject(null);
        customer.PushCarriableObject(bag);

        await UniTask.Delay(TimeSpan.FromSeconds(0.4f));
        // TODO: 행복 이모티콘 띄우기
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
