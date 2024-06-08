using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class Oven : InteractionObject
{
    public override InteractionObjectType interactionObjectType => InteractionObjectType.oven;

    [SerializeField] int maxCroassant;
    public int currCroassantCount { get; private set; }
    CroassantPool pool => PoolContainer.instance.GetPool<CroassantPool>(ObjectPoolType.croassant);
    Vector3 spawnPos => new Vector3(-5.94999981f, 1.74300003f, -2.3900001f);


    CancellationTokenSource cts;

    Stack<Croassant> croassants = new Stack<Croassant>();

    public void Start()
    {
        cts = new CancellationTokenSource();
        Produce().Forget();
    }

    async UniTaskVoid Produce()
    {
        await UniTask.WaitUntil(() => currCroassantCount < maxCroassant, cancellationToken: cts.Token);
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: cts.Token);

        pool.Spawn(spawnPos, Quaternion.identity, OnSpawnCroassant);
        currCroassantCount++;

        Produce().Forget();
    }

    void OnSpawnCroassant(Croassant croassant)
    {
        InvokePushCroassant(croassant).Forget();
        croassant.MoveToTarget(new Vector3(-5.94999981f, 1.74300003f, -3.60100007f), 1f, (croassant) =>
        {
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

    Player player;
    public override void OnPlayerEnter(Player player)
    {
        this.player = player;
    }

    public override void OnPlayerExit(Player player)
    {
        this.player = null;
    }

    float cooldown;
    private void Update()
    {
        cooldown += Time.deltaTime;
        if (cooldown >= 0.1f)
        {
            if (player == null)
                return;

            if (!player.CanPushCroassant())
                return;

            if (croassants.Count <= 0)
                return;

            var croassant = croassants.Pop();
            var targetPos = player.stackStartPos + new Vector3(0f, player.stackGap * player.currCroassantCount);

            player.PushCroassant(croassant);
            currCroassantCount--;

            croassant.SetParent(player.parent)
                     .SetActiveCollider(false)
                     .DestroyRigidbody()
                     .MoveToTargetWithCurve(targetPos, 0.2f);

            cooldown = 0f;
        }

    }

    private void OnDestroy()
    {
        cts.Cancel();
    }
}
