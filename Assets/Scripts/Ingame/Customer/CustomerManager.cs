using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

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
    CustomerPool pool => PoolContainer.instance.GetPool<CustomerPool>(ObjectPoolType.customer);
    int currCustomerCount;
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
        customer.MoveToCroassant(ioManager.GetPos(InteractionObjectType.basket, currCustomerCount))
                .SetActionOnMoveEnd(OnMoveToCroassantEnd);
        currCustomerCount++;

        InvokeCustomerSpawn().Forget();
    }

    void OnMoveToCroassantEnd(Customer customer)
    {
        var rot = Quaternion.LookRotation(ioManager.GetPos(InteractionObjectType.basket) - customer.position, customer.up);
        rot.x = 0f;
        rot.z = 0f;
        customer.RotateTo(rot, 0.2f);
    }

    private void OnDestroy()
    {
        cts.Cancel();
    }
}
