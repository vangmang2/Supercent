using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Pos : InteractionObject
{
    [SerializeField] MoneyPillar moneyPillar;

    public override InteractionObjectType interactionObjectType => InteractionObjectType.pos;
    public int currPaymentWaitingCount { get; private set; }
    public int currTableWaitingCount { get; private set; }

    public Vector3 paymentWatingStartPos => new Vector3(-0.800000012f, 0f, 1.38999999f);
    public Vector3 tableWatingStartPos => new Vector3(0.949999988f, 0f, 1.38999999f);
    public float lineGap => 1.25f;

    PaperBagPool paperBagPool => PoolContainer.instance.GetPool<PaperBagPool>();
    List<Interactant> casherList = new List<Interactant>();
    Queue<Customer> customerQueue = new Queue<Customer>();

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

    public Pos IncreaseTableWatingCount()
    {
        currTableWaitingCount++;
        return this;
    }

    public Pos DecreaseTableWatingCount()
    {
        currTableWaitingCount--;
        return this;
    }

    public void IncreaseMoney(int amount)
    {
        moneyPillar.IncreaseMoney(amount);
        moneyPillar.SpawnMoneyStack();
    }

    public override void OnInteractantEnter(Interactant interactant)
    {
        cooldown = 1.5f;
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
            }
            cooldown = 0f;
        }
    }
}
