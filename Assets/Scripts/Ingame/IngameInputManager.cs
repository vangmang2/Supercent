using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameInputManager : MonoBehaviour
{
    [SerializeField] UIItemJoystick joystick;
    [SerializeField] Player player;

    private void Start()
    {
        joystick.SubscribeOnMove(player.MoveToTargetRot);
    }

    private void Update()
    {
        player.SetMoveAnimation(Input.GetMouseButton(0));
    }
}
