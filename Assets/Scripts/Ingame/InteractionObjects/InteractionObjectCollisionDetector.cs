using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObjectCollisionDetector : MonoBehaviour
{
    [SerializeField] InteractionObject io;

    private void OnCollisionEnter(Collision collision)
    {
        var interactant = collision.gameObject.GetComponent<Interactant>();
        if (interactant != null)
        {
            io.OnInteractantEnter(interactant);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        var interactant = collision.gameObject.GetComponent<Interactant>();
        if (interactant != null)
        {
            io.OnInteractantExit(interactant);
        }
    }
}
