using Cysharp.Threading.Tasks;
using Lean.Pool;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using IPoolable = Lean.Pool.IPoolable;

public class MoneyStack : MonoBehaviour, IPoolable
{
    [SerializeField] List<Transform> tfMoneyList;
    List<Vector3> originPosList = new List<Vector3>();
    CancellationTokenSource cts;
    public void OnSpawn()
    {
        Init();
    }

    public void OnDespawn()
    {
    }

    private void Awake()
    {
        foreach (var tfMoney in tfMoneyList)
            originPosList.Add(tfMoney.localPosition);
    }

    public void Init()
    {
        for (int i = 0; i < tfMoneyList.Count; i++)
        {
            var originPos = originPosList[i];
            var tfMoney = tfMoneyList[i];
            tfMoney.localPosition = originPos;
            tfMoney.gameObject.SetActive(true);
        }
    }

    public void PlayMoneyToPlayerAnim(Vector3 target, Action<MoneyStack> callback)
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        InvokePlayMoneyToPlayerAnim(target, callback).Forget();
    }

    async UniTaskVoid InvokePlayMoneyToPlayerAnim(Vector3 target, Action<MoneyStack> callback)
    {
        foreach (var tfMoney in tfMoneyList)
        {
            var startPos = tfMoney.position;
            var endPos = target;
            var centerPos = (startPos + endPos) * 0.5f;
            centerPos.y += 3f;

            var t = 0f;
            while (t <= 1f)
            {
                t += Time.deltaTime * (1f / 0.02f);
                var ab = Vector3.Lerp(startPos, centerPos, t);
                var cb = Vector3.Lerp(centerPos, endPos, t);
                var acb = Vector3.Lerp(ab, cb, t);

                tfMoney.position = acb;
                await UniTask.Yield(cts.Token);
            }
            tfMoney.gameObject.SetActive(false);
        }
        callback?.Invoke(this);
    }

    void OnDestroy() 
    {
        cts?.Cancel();
    }
}
