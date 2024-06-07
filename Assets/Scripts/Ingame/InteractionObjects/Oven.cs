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

    public void Start()
    {
        cts = new CancellationTokenSource();
        Produce().Forget();
    }

    async UniTaskVoid Produce()
    {
        await UniTask.WaitUntil(() => currCroassantCount < maxCroassant, cancellationToken: cts.Token);
        await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: cts.Token);

        pool.Spawn(spawnPos, Quaternion.identity, OnSpawnCroassant);
        currCroassantCount++;

        Produce().Forget();
    }

    void OnSpawnCroassant(Croassant croassant)
    {
        croassant.MoveToTarget(new Vector3(-8.97500038f, 15.3870001f, -30.3199997f), 1.5f, (croassant) =>
        {
            croassant.SetColliderEnable(true);
            croassant.SetGravityEnable(true);
            croassant.AddForce(force);
        });
    }



    public override void OnPlayerTouched(Player player)
    {

    }

    private void OnDestroy()
    {
        cts.Cancel();
    }
}
