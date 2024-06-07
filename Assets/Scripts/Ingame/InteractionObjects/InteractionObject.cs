using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObject : MonoBehaviour
{
    public virtual void OnPlayerEnter(Player player) { }
    public virtual void OnPlayerExit(Player player) { }
}
