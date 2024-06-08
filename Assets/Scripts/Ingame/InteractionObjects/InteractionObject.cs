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
    public abstract void OnInteractantEnter(Interactant interactant);
    public abstract void OnInteractantExit(Interactant interactant);
}
