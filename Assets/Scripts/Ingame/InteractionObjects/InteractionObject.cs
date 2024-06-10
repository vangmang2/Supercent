using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class InteractionObject : MonoBehaviour
{
    [SerializeField] protected List<Transform> waitingPosList;
    public Vector3 position => transform.position;
    public abstract Vector3 GetPos(int index);
    public abstract void OnInteractantEnter(Interactant interactant);
    public abstract void OnInteractantExit(Interactant interactant);
}
