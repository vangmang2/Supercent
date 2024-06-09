using Cysharp.Threading.Tasks;
using Lean.Pool;
using System.Collections.Generic;
using UnityEngine;

public class MoneyStack : MonoBehaviour, IPoolable
{
    [SerializeField] List<Transform> tfMoneyList;
    List<Vector3> originPosList = new List<Vector3>();

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
            originPosList.Add(tfMoney.position);
    }

    public void Init()
    {
        for (int i = 0; i < tfMoneyList.Count; i++)
        {
            var originPos = originPosList[i];
            tfMoneyList[i].position = originPos;
        }
    }

    public void PlayMoneyToPlayerAnim(Vector3 target)
    {
        InvokePlayMoneyToPlayerAnim(target).Forget();
    }

    async UniTaskVoid InvokePlayMoneyToPlayerAnim(Vector3 target)
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
                t += Time.deltaTime * (1f / 0.15f);
                var ab = Vector3.Lerp(startPos, centerPos, t);
                var cb = Vector3.Lerp(centerPos, endPos, t);
                var acb = Vector3.Lerp(ab, cb, t);

                tfMoney.position = acb;
                await UniTask.Yield();
            }
            tfMoney.gameObject.SetActive(false);
        }
    }
}
