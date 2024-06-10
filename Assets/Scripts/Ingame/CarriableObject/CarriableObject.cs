using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriableObject : MonoBehaviour
{

    public CarriableObject SetPosition(Vector3 position)
    {
        transform.position = position;
        return this;
    }

    public CarriableObject SetRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
        return this;
    }

    public CarriableObject SetParent(Transform parent)
    {
        transform.SetParent(parent);
        return this;
    }

    public void MoveToTarget(Vector3 targetPos, float duration, Action<CarriableObject> onComplete = null)
    {
        transform.DOMove(targetPos, duration).OnComplete(() =>
        {
            onComplete?.Invoke(this);
        });
    }

    public CarriableObject MoveToTargetWithCurve(Vector3 targetPos, float duration, float targetRot = 90f, float height = 2f, Action<CarriableObject> onComplete = null)
    {
        InvokeMoveToTargetWithCurve(targetPos, duration, targetRot, height, onComplete).Forget();
        return this;
    }


    async UniTaskVoid InvokeMoveToTargetWithCurve(Vector3 targetPos, float duration, float rot, float height, Action<CarriableObject> onComplete)
    {
        var startPos = transform.localPosition;
        var endPos = targetPos;

        var centerPos = (startPos + endPos) * 0.5f;
        centerPos.y += height;

        var startRot = transform.localRotation;
        var targetRot = Quaternion.Euler(0f, rot, 0f);

        var t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime * (1f / duration);
            var ac = Vector3.Lerp(startPos, centerPos, t);
            var cb = Vector3.Lerp(centerPos, endPos, t);
            var acb = Vector3.Lerp(ac, cb, t);

            transform.localPosition = acb;
            transform.localRotation = Quaternion.Lerp(startRot, targetRot, t);
            await UniTask.Yield();
        }

        transform.localPosition = targetPos;
        onComplete?.Invoke(this);
    }


    public void SetActive(bool enable)
    {
        gameObject.SetActive(enable);
    }
}
