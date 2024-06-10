using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Pos : InteractionObject, ITutorialTarget
{
    [SerializeField] MoneyPillar moneyPillar;
    [SerializeField] GameObject tutorialPoint;

    public int tutorialIndex => 2;
    public int currPaymentWaitingCount { get; private set; }
    public int currTableWaitingCount => tableWaitingCustomerQueue.Count;

    public Vector3 paymentWatingStartPos => new Vector3(-0.800000012f, 0f, 1.38999999f);
    public Vector3 tableWatingStartPos => new Vector3(0.949999988f, 0f, 1.38999999f);
    public float lineGap => 1.25f;

    PaperBagPool paperBagPool => PoolContainer.instance.GetPool<PaperBagPool>();

    public Transform tutorialTarget => transform;

    public GameObject goTargetPoint => tutorialPoint;

    List<Interactant> casherList = new List<Interactant>();
    Queue<Customer> customerQueue = new Queue<Customer>();
    Queue<Customer> tableWaitingCustomerQueue = new Queue<Customer>();

    Action<Pos, Customer, PaperBag> onPay;

    public Pos IncreasePaymentWaitingCount()
    {
        currPaymentWaitingCount++;
        return this;
    }

    public Pos DecreasePaymentWaitingCount()
    {
        currPaymentWaitingCount--;
        return this;
    }

    public Pos EnqueueTableWaitingCustomer(Customer customer)
    {
        tableWaitingCustomerQueue.Enqueue(customer);
        return this;
    }

    public Pos DequeueTableWaitingCustomer()
    {
        tableWaitingCustomerQueue.Dequeue();
        return this;
    }

    public void UpdateTableWaitingCustomerLine()
    {
        var index = 0;
        foreach (var customer in tableWaitingCustomerQueue)
        {
            customer.MoveToTarget(tableWatingStartPos + new Vector3(0f, 0f, lineGap * index));
            index++;
        }
    }

    public void IncreaseMoney(int amount, int loopCount = 1, float delay = 0.1f)
    {
        InvokeIncreaseMoney(amount, loopCount, delay).Forget();
    }

    async UniTaskVoid InvokeIncreaseMoney(int amount, int loopCount, float delay)
    {
        for (int i = 0; i < loopCount; i++)
        {
            moneyPillar.IncreaseMoney(amount);
            moneyPillar.SpawnMoneyStack();
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
        }
    }

    public override void OnInteractantEnter(Interactant interactant)
    {
        casherList.Add(interactant);
    }

    public override void OnInteractantExit(Interactant interactant)
    {
        casherList.Remove(interactant);
    }

    public override Vector3 GetPos(int index)
    {
        return Vector3.zero;
    }

    public void EnqueueCustomer(Customer customer)
    {
        customerQueue.Enqueue(customer);
    }

    public Pos SetActionOnPay(Action<Pos, Customer, PaperBag> callback)
    {
        onPay = callback;
        return this;
    }

    public void DequeueCustomer()
    {
        customerQueue.Dequeue();
    }

    public void PullCustomersLine()
    {
        int count = 0;
        foreach (var _customer in customerQueue)
        {
            _customer.MoveToTarget(paymentWatingStartPos + new Vector3(0f, 0f, count * lineGap));
            count++;
        }
        DecreasePaymentWaitingCount();
    }

    float cooldown;
    int tutorialCustomerCount;
    private void Update()
    {
        cooldown += Time.deltaTime;
        if (cooldown >= 2f)
        {
            foreach (var casher in casherList)
            {
                if (customerQueue.Count <= 0)
                    continue;

                var customer = customerQueue.Peek();
                var bag = paperBagPool.Spawn(new Vector3(-1.44000006f, 1.36699998f, 0f), Quaternion.Euler(0f, 90f, 0f), null);
                onPay?.Invoke(this, customer, bag);

                if (TutorialManager.instance.tutorialIndex == 3)
                {
                    tutorialCustomerCount++;
                    if (tutorialCustomerCount == 2)
                    {
                        // 4. 테이블쪽으로 카메라 이동, 포스기 옆 돈 강조
                        TutorialManager.instance.PlayTutorial();
                    }
                }
            }
            cooldown = 0f;
        }
    }
}
