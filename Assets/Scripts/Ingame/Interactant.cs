using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 주는자, 가져가는자
public enum InteractantType
{
    giver,
    getter
}


public abstract class Interactant : MonoBehaviour
{
    public abstract InteractantType interactantType { get; }
    int maxStack;

    protected Stack<CarriableObject> coStack = new Stack<CarriableObject>();
    public int currCOCount => coStack.Count;
    public Vector3 position => transform.position;
    public Vector3 stackStartPos => new Vector3(0f, 0.643999994f, 0.470999986f);
    public float stackGap => 0.277f;
    public Transform parent => transform;

    Action<Interactant> onPushCO;

    public void SetActionOnPushCarriableObject(Action<Interactant> callback)
    {
        onPushCO = callback;
    }

    protected void SetMaxStackCount(int count)
    {
        maxStack = count;
    }

    public bool CanPushCroassant()
    {
        return (coStack.Count < maxStack);
    }

    public void PushCarriableObject(CarriableObject carriableObject, SoundType soundType = SoundType.getObject)
    {
        coStack.Push(carriableObject);
        onPushCO?.Invoke(this);
        SoundManager.instance.PlaySFX(soundType);
    }

    public CarriableObject PopCarriableObject(SoundType soundType = SoundType.putObject)
    {
        SoundManager.instance.PlaySFX(soundType);
        return coStack.Pop();
    }
}
