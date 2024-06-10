using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class Oven : InteractionObject, ITutorialTarget
{

    [SerializeField] int maxCroassant;
    [SerializeField] GameObject tutorialPoint;

    public int currCroassantCount { get; private set; }
    CroassantPool pool => PoolContainer.instance.GetPool<CroassantPool>();
    Vector3 spawnPos => new Vector3(-5.98999977f, 1.89999998f, -3.81999993f);

    public int tutorialIndex => 0;
    public Transform tutorialTarget => transform;
    public GameObject goTargetPoint => tutorialPoint;


    CancellationTokenSource cts;

    Stack<Croassant> croassants = new Stack<Croassant>();
    List<Interactant> interactants = new List<Interactant>();

    public void Start()
    {
        cts = new CancellationTokenSource();
        Produce().Forget();
    }

    async UniTaskVoid Produce()
    {
        await UniTask.WaitUntil(() => currCroassantCount < maxCroassant, cancellationToken: cts.Token);
        await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cts.Token);

        pool.Spawn(spawnPos, Quaternion.identity, OnSpawnCroassant);
        currCroassantCount++;

        Produce().Forget();
    }

    void OnSpawnCroassant(Croassant croassant)
    {
        InvokePushCroassant(croassant).Forget();
        croassant.MoveToTarget(new Vector3(-5.98999977f, 1.89999998f, -4.96999979f), 1f, (_croassant) =>
        {
            var croassant = _croassant as Croassant;
            croassant.SetActiveCollider(true);
            var force = new Vector3(Random.Range(-5f, 5f), 0f, -80f);
            croassant.AddForce(force);
        });
    }

    async UniTaskVoid InvokePushCroassant(Croassant croassant)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2.5f));
        croassants.Push(croassant);
    }

    public override void OnInteractantEnter(Interactant interactant)
    {
        interactants.Add(interactant);
    }

    public override void OnInteractantExit(Interactant interactant)
    {
        interactants.Remove(interactant);
    }

    public override Vector3 GetPos(int index)
    {
        return Vector3.zero;
    }

    int tutorialCroassantCount;
    float cooldown;
    private void Update()
    {
        cooldown += Time.deltaTime;
        if (cooldown >= 0.15f)
        {
            foreach (var interactant in interactants)
            {
                if (!interactant.CanPushCroassant())
                    continue;

                if (croassants.Count <= 0)
                    continue;

                var croassant = croassants.Pop();
                var targetPos = interactant.stackStartPos + new Vector3(0f, interactant.stackGap * interactant.currCOCount);

                interactant.PushCarriableObject(croassant);
                currCroassantCount--;

                croassant.SetParent(interactant.parent);
                croassant.SetActiveCollider(false)
                         .DestroyRigidbody()
                         .MoveToTargetWithCurve(targetPos, 0.2f);

                // 2. 바스켓 강조
                if (TutorialManager.instance.tutorialIndex == 1)
                {
                    tutorialCroassantCount++;
                    if (tutorialCroassantCount == 6)
                        TutorialManager.instance.PlayTutorial();
                }
            }

            cooldown = 0f;
        }

    }

    private void OnDestroy()
    {
        cts.Cancel();
    }
}
