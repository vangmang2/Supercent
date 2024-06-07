using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObjectCollisionDetector : MonoBehaviour
{
    [SerializeField] InteractionObject io;

    private void OnCollisionEnter(Collision collision)
    {
        var player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            io.OnPlayerEnter(player);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        var player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            io.OnPlayerExit(player);
        }
    }
}
