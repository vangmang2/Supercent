using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum InteractionObjectType
{
    basket,
    pos,
    oven
}
public abstract class InteractionObject : MonoBehaviour
{
    [SerializeField] protected List<Transform> waitingPosList;
    public Vector3 position => transform.position;
    public Vector3 GetPos(int index) => waitingPosList[index].position;
    public abstract InteractionObjectType interactionObjectType { get; }
    public abstract void OnPlayerEnter(Player player);
    public abstract void OnPlayerExit(Player player);
}
