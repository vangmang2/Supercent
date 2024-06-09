using Lean.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperBag : CarriableObject
{
    [SerializeField] Animator animator;

    public Vector3 localPosition => transform.localPosition;

    public void PlayCloseAnim()
    {
        animator.Play("Paper Bag_close", 0);
    }
}
