using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

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
