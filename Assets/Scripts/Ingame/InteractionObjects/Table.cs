using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Table : InteractionObject
{
    [SerializeField] TMP_Text txtAmount;
    [SerializeField] List<GameObject> goLockList, goUnlockList;
    int currUnlockAmount = 5;

    public bool canUse { get; private set; }
    public override InteractionObjectType interactionObjectType => InteractionObjectType.table;

    private void Start()
    {
        txtAmount.SetText(currUnlockAmount.ToString());
    }

    public override Vector3 GetPos(int index)
    {
        return Vector3.zero;
    }

    Player player;
    public override void OnInteractantEnter(Interactant interactant)
    {
        player = interactant as Player;
    }

    public override void OnInteractantExit(Interactant interactant)
    {
        player = null;
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

    float cooldown = 0f;
    private void Update()
    {
        if (currUnlockAmount <= 0)
            return;

        cooldown += Time.deltaTime;
        if (cooldown >= 0.05f)
        {
            if (player == null)
                return;
            if (player.money <= 0)
                return;

            player.DecreaseMoney(1);
            currUnlockAmount -= 1;
            txtAmount.SetText(currUnlockAmount.ToString());

            if (currUnlockAmount <= 0)
            {
                canUse = true;
                // 테이블 언락
                ShowUnlockedObject();
            }
            cooldown = 0f;
        }
    }
}
