using System.Collections.Generic;
using UnityEngine;

public class Pos : InteractionObject
{
    public override InteractionObjectType interactionObjectType => InteractionObjectType.pos;
    public int currPaymentWaitingCount { get; private set; }
    public int currTableWaitingCount { get; private set; }

    public Vector3 paymentWatingStartPos => new Vector3(-0.800000012f, 0f, 1.38999999f);
    public Vector3 tableWatingStartPos => new Vector3(0.949999988f, 0f, 1.38999999f);
    public float lineGap => 1.25f;


    List<Interactant> casherList = new List<Interactant>();
    Queue<Customer> customerQueue = new Queue<Customer>();

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

    float cooldown;
    List<Croassant> paymentCroassantList = new List<Croassant>();
    private void Update()
    {
        cooldown += Time.deltaTime;
        if (cooldown >= 2f)
        {
            foreach (var casher in casherList)
            {
                if (customerQueue.Count <= 0)
                    continue;

                //var customer = customerQueue.Dequeue();
                
                //var croassants = customer.currCroassantCount
            }
            cooldown = 0f;
        }
    }
}
