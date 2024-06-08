using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using Lean;
using Lean.Pool;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class Croassant : MonoBehaviour, IPoolable
{
    [SerializeField] new Collider collider;
    [SerializeField] Rigidbody rigidBody;

    public Croassant SetActiveCollider(bool enable)
    {
        collider.enabled = enable;
        return this;
    }

    public Croassant DestroyRigidbody()
    {
        Destroy(rigidBody);
        return this;
    }

    public Croassant SetParent(Transform parent)
    {
        transform.SetParent(parent);
        return this;
    }

    public void AddForce(Vector3 force)
    {
        rigidBody.AddForce(force);
    }

    public void MoveToTarget(Vector3 targetPos, float duration, Action<Croassant> onComplete = null) 
    {
        transform.DOMove(targetPos, duration).OnComplete(() =>
        {
            onComplete?.Invoke(this);
        });
    }

    public void MoveToTargetWithCurve(Vector3 targetPos, float duration, float targetRot = 90f)
    {
        InvokeMoveToTargetWithCurve(targetPos, duration, targetRot).Forget();
    }

    async UniTaskVoid InvokeMoveToTargetWithCurve(Vector3 targetPos, float duration, float rot)
    {
        var startPos = transform.localPosition;
        var endPos = targetPos;

        var centerPos = (startPos + endPos) * 0.5f ;
        centerPos.y += 2f;

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
    }

    public void OnSpawn()
    {

    }

    public void OnDespawn()
    {
        SetActiveCollider(false);
        rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.mass = 0.15f;
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }
}
