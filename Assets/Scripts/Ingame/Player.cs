using System.Collections.Generic;
using UnityEngine;

public class Player : Interactant
{
    [SerializeField] Transform tfBody;
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] float speed;
    [SerializeField] Animator animator;

    public override InteractantType interactantType => InteractantType.giver;

    private void Start()
    {
        SetMaxStackCount(8);
    }

    public void MoveToTargetRot(float rot)
    {
        rigidBody.rotation = Quaternion.Euler(0f, -rot * Mathf.Rad2Deg + 90f, 0f);
        rigidBody.position += tfBody.forward * speed * Time.deltaTime;
    }

    public Player SetMoveAnimation(bool enable)
    {
        animator.SetBool("Move", enable);
        return this;
    }

    private void Update()
    {
        animator.SetBool("IsHoldingSomething", croassants.Count > 0);
    }
}
