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

    protected Stack<Croassant> croassants = new Stack<Croassant>();
    public int currCroassantCount => croassants.Count;
    public Vector3 stackStartPos => new Vector3(0f, 0.643999994f, 0.470999986f);
    public float stackGap => 0.277f;
    public Transform parent => transform;

    Action<Interactant> onPushCroassant;

    public void SetActionOnPushCroassant(Action<Interactant> callback)
    {
        onPushCroassant = callback;
    }

    protected void SetMaxStackCount(int count)
    {
        maxStack = count;
    }

    public bool CanPushCroassant()
    {
        return (croassants.Count < maxStack);
    }

    public void PushCroassant(Croassant croassant)
    {
        croassants.Push(croassant);
        onPushCroassant?.Invoke(this);
        SoundManager.instance.PlaySFX(SoundType.getObject);
    }

    public Croassant PopCroassant()
    {
        SoundManager.instance.PlaySFX(SoundType.putObject);
        return croassants.Pop();
    }
}
