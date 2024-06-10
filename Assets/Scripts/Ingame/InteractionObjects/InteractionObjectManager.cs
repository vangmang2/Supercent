using System;
using System.Collections.Generic;
using UnityEngine;


public class InteractionObjectManager : MonoBehaviour
{    
    [SerializeField] List<InteractionObject> ioList;
    Dictionary<Type, InteractionObject> ioDic = new Dictionary<Type, InteractionObject>();

    public static InteractionObjectManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach (var io in ioList)
        {
            ioDic.Add(io.GetType(), io);
        }
    }

    public Vector3 GetPos<T>() where T : InteractionObject
    {
        return ioDic[typeof(T)].position;
    }

    public Vector3 GetPos<T>(int index) where T : InteractionObject
    {
        return ioDic[typeof(T)].GetPos(index);
    }

    public T GetInteractionObject<T>() where T : InteractionObject
    {
        return ioDic[typeof(T)] as T;
    }
}
