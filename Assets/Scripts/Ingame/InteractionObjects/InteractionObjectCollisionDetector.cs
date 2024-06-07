using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObjectCollisionDetector : MonoBehaviour
{
    [SerializeField] InteractionObject io;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            var player = collision.gameObject.GetComponent<Player>();
            if (player != null) io.OnPlayerTouched(player);
        }
    }
}
