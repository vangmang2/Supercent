using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] RectTransform rtCanvas;
    [SerializeField] Camera uiCamera;
    [SerializeField] UIItemJoystick joystick;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rtCanvas, Input.mousePosition, uiCamera, out var mousePos);
            joystick.SetPosition(mousePos)
                    .SetActive(true);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            joystick.SetActive(false);
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rtCanvas, Input.mousePosition, uiCamera, out var mousePos);
            joystick.SetStickPosition(mousePos);
        }
    }
}
