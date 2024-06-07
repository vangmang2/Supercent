using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Oven : InteractionObject
{
    [SerializeField] int maxCroassant;
    [SerializeField] Vector3 force;
    public int currCroassantCount { get; private set; }
    CroassantPool pool => PoolContainer.instance.GetPool<CroassantPool>(ObjectPoolType.croassant);
    Vector3 spawnPos => new Vector3(-8.97500038f, 15.3870001f, -29.6599998f);
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
        croassant.MoveToTarget(new Vector3(-8.97500038f, 15.3870001f, -30.3199997f), 1f, (croassant) =>
        {
            croassant.SetColliderEnable(true);
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
                     .SetColliderEnable(false)
                     .DestroyRigidbody()
                     .MoveToTargetWithCurve(targetPos, 0.5f);

            cooldown = 0f;
        }

    }

    private void OnDestroy()
    {
        cts.Cancel();
    }
}
