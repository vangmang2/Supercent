using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using Lean;
using Lean.Pool;

public class Croassant : MonoBehaviour, IPoolable
{
    [SerializeField] new Collider collider;
    [SerializeField] Rigidbody rigidBody;

    public Croassant SetColliderEnable(bool enable)
    {
        collider.enabled = enable;
        return this;
    }

    public Croassant SetGravityEnable(bool enable)
    {
        rigidBody.useGravity = enable;
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

    public void OnSpawn()
    {

    }

    public void OnDespawn()
    {
        SetColliderEnable(false);
        SetGravityEnable(false);
    }
}
