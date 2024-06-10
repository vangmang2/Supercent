using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    public static FollowingCamera instance { get; private set; }

    [SerializeField] Transform tfTarget;

    bool canFollow = true;

    private void Awake()
    {
        instance = this; 
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!canFollow)
            return;

        transform.position = tfTarget.position + new Vector3(0, 10.9300003f, -8.89000034f);
    }

    public FollowingCamera SetEnableCanFollow(bool enable)
    {
        canFollow = enable;
        return this;
    }

    public void MoveToTarget(Vector3 target, float delay, Action<FollowingCamera> callback)
    {
        transform.DOKill();
        transform.DOMove(target, 1f).SetEase(Ease.Linear).SetDelay(delay);
        transform.DOMove(tfTarget.position + new Vector3(0, 10.9300003f, -8.89000034f), 1f).SetEase(Ease.Linear).SetDelay(2f + delay).OnComplete(() =>
        {
            callback?.Invoke(this);
        });
    }
}
