using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using Lean;
using Lean.Pool;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class Croassant : CarriableObject, IPoolable
{
    [SerializeField] new Collider collider;
    [SerializeField] Rigidbody rigidBody;

    public Vector3 localPosition;

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

    public void AddForce(Vector3 force)
    {
        rigidBody.AddForce(force);
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
