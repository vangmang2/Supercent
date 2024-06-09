using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IngameUIManager : MonoBehaviour
{
    [SerializeField] RectTransform rtCanvas;
    [SerializeField] Camera uiCamera;
    [SerializeField] UIItemJoystick joystick;
    [SerializeField] Player player;
    [SerializeField] CoinShower coinShower;

    private void Start()
    {
        player.SetActionOnCoinChanged(OnCoinChanged);
    }

    void OnCoinChanged(int coin)
    {
        coinShower.SetTextCoin(coin.ToString())
                  .PlayCoinAnim();
    }

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
        else if (Input.GetMouseButton(0))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rtCanvas, Input.mousePosition, uiCamera, out var mousePos);
            joystick.SetStickPosition(mousePos);
        }
    }
}
