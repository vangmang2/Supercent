using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Table : InteractionObject
{
    [SerializeField] TMP_Text txtAmount;
    [SerializeField] List<GameObject> goLockList, goUnlockList;
    int currUnlockAmount = 5;

    Queue<Customer> waitingQueue = new Queue<Customer>();

    public bool isUnlocked { get; private set; }
    public bool isClean { get; private set; }
    public int currQueueCount => waitingQueue.Count;

    public Customer nextCustomer
    {
        get
        {
            return waitingQueue.Count > 0 ? waitingQueue.Peek() : null;
        }
    }


    private void Start()
    {
        isClean = true;
        txtAmount.SetText(currUnlockAmount.ToString());
    }

    public void EnqueueCustomer(Customer customer)
    {
        waitingQueue.Enqueue(customer);
    }

    public void Dequeue()
    {
        waitingQueue.Dequeue();
    }

    public override Vector3 GetPos(int index)
    {
        return Vector3.zero;
    }

    Player player;
    public override void OnInteractantEnter(Interactant interactant)
    {
        player = interactant as Player;
    }

    public override void OnInteractantExit(Interactant interactant)
    {
        player = null;
    }

    [Button]
    void ShowLockedObject()
    {
        foreach (var go in goLockList)
        {
            go.SetActive(true);
        }

        foreach (var go in goUnlockList)
        {
            go.SetActive(false);
        }
    }

    [Button]
    void ShowUnlockedObject()
    {
        foreach (var go in goLockList)
        {
            go.SetActive(false);
        }

        foreach (var go in goUnlockList)
        {
            go.SetActive(true);
        }
    }

    float cooldown = 0f;
    private void Update()
    {
        if (currUnlockAmount <= 0)
            return;

        cooldown += Time.deltaTime;
        if (cooldown >= 0.05f)
        {
            if (player == null)
                return;
            if (player.money <= 0)
                return;

            player.DecreaseMoney(1);
            currUnlockAmount -= 1;
            txtAmount.SetText(currUnlockAmount.ToString());

            if (currUnlockAmount <= 0)
            {
                isUnlocked = true;
                // 테이블 언락
                ShowUnlockedObject();
            }
            cooldown = 0f;
        }
    }
}
