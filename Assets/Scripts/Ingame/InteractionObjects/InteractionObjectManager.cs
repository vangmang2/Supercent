using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractionObjectManager : MonoBehaviour
{
    //public static InteractionObjectManager instance { get; private set; }

    [SerializeField] List<InteractionObject> ioList;
    Dictionary<InteractionObjectType, InteractionObject> ioDic = new Dictionary<InteractionObjectType, InteractionObject>();

    private void Awake()
    {
        //instance = this;
    }

    private void Start()
    {
        foreach (var io in ioList)
        {
            ioDic.Add(io.interactionObjectType, io);
        }
    }

    public Vector3 GetPos(InteractionObjectType type)
    {
        return ioDic[type].position;
    }

    public Vector3 GetPos(InteractionObjectType type, int index)
    {
        return ioDic[type].GetPos(index);
    }

    public T GetInteractionObject<T>(InteractionObjectType type) where T : InteractionObject
    {
        return ioDic[type] as T;
    }
}
