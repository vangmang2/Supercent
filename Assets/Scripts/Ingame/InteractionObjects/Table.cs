using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Table : InteractionObject
{
    int currUnlockAmount = 30;
    [SerializeField] TMP_Text txtAmount;
    [SerializeField] List<GameObject> goLockList, goUnlockList;


    public override InteractionObjectType interactionObjectType => InteractionObjectType.table;

    public override Vector3 GetPos(int index)
    {
        return Vector3.zero;
    }

    public override void OnInteractantEnter(Interactant interactant)
    {
    }

    public override void OnInteractantExit(Interactant interactant)
    {
    }

    [Button]
    void ShowLockedObject()
    {
        foreach (var go in goLockList)
        {
            go.SetActive(true);
        }

        foreach (var go in goUnlockList)
        {
            go.SetActive(false);
        }
    }

    [Button]
    void ShowUnlockedObject()
    {
        foreach (var go in goLockList)
        {
            go.SetActive(false);
        }

        foreach (var go in goUnlockList)
        {
            go.SetActive(true);
        }
    }
}
