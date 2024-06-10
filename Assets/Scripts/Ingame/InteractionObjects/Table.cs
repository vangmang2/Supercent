using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Table : InteractionObject, ITutorialTarget
{
    [SerializeField] TMP_Text txtAmount;
    [SerializeField] ParticleSystem vfx_Clean;
    [SerializeField] List<GameObject> goLockList, goUnlockList;
    [SerializeField] GameObject goTrash, goTutorialPoint;
    [SerializeField] MoneyPillar moneyPillar;
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

    public int tutorialIndex => 4;
    public Transform tutorialTarget => transform;
    public GameObject goTargetPoint => goTutorialPoint;

    public void SetActiveTrash(bool enable)
    {
        isClean = !enable;
        goTrash.SetActive(enable);
    }

    public void PlayCleanSfx()
    {
        vfx_Clean.Play();
    }

    private void Start()
    {
        isClean = true;
        txtAmount.SetText(currUnlockAmount.ToString());
    }

    public Table EnqueueCustomer(Customer customer)
    {
        waitingQueue.Enqueue(customer);
        return this;
    }

    public Table Dequeue()
    {
        waitingQueue.Dequeue();
        return this;
    }

    public Table IncreaseMoney(int amount, int loopCount = 1, float delay = 0.1f)
    {
        InvokeIncreaseMoney(amount, loopCount, delay).Forget();
        return this;
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
    public override Vector3 GetPos(int index)
    {
        return Vector3.zero;
    }

    Player player;
    public override void OnInteractantEnter(Interactant interactant)
    {
        player = interactant as Player;

        if (!isUnlocked)
            return;

        if (isClean)
            return;

        SetActiveTrash(false);
        PlayCleanSfx();
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
        if (TutorialManager.instance.tutorialIndex == 5)
        {
            TutorialManager.instance.PlayTutorial();
        }

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

                LogicManager.instance.SetActiveEtcPrice(true);
                FollowingCamera.instance.SetEnableCanFollow(false)
                               .MoveToTarget(new Vector3(-8.69999981f, 11.3550005f, -5.6500001f), 1f, (followingCamera) =>
                               {
                                   followingCamera.SetEnableCanFollow(true);
                               });
            }
            cooldown = 0f;
        }
    }
}
