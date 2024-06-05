using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform tfBody;
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] float speed;
    [SerializeField] Animator animator;

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
}
