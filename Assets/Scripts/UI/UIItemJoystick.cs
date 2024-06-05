using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemJoystick : MonoBehaviour
{
    [SerializeField] RectTransform rtBody, rtStick;
    [SerializeField] float maxDistance;

    Vector2 currentPosition;

    event Action<float> onMove;

    private void Start()
    {
        SetActive(false);
    }

    public void SubscribeOnMove(Action<float> callback)
    {
        onMove += callback;
    }

    public void UnsubscribeOnMove(Action<float> callback)
    {
        onMove -= callback;
    }

    public UIItemJoystick SetPosition(Vector2 position)
    {
        rtBody.anchoredPosition = position;
        currentPosition = position;
        return this;
    }

    public UIItemJoystick SetActive(bool enable)
    {
        gameObject.SetActive(enable);
        return this;
    }

    public UIItemJoystick SetStickPosition(Vector2 position)
    {
        var point = position - currentPosition;
        var distance = Mathf.Min(point.magnitude, maxDistance);
        var deg = Mathf.Atan2(point.y, point.x);
        var targetPos = new Vector2(Mathf.Cos(deg) * distance, Mathf.Sin(deg) * distance);

        rtStick.anchoredPosition = targetPos;
        onMove?.Invoke(deg);
        return this;
    }
}
