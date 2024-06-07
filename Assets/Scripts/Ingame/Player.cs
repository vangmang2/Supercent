using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    const int maxStack = 8;

    [SerializeField] Transform tfBody;
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] float speed;
    [SerializeField] Animator animator;

    public Transform parent => transform;

    Stack<Croassant> croassants = new Stack<Croassant>();
    public int currCroassantCount => croassants.Count;
    public Vector3 stackStartPos => new Vector3(0f, 0.643999994f, 0.470999986f);
    public float stackGap => 0.277f;

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

    public bool CanPushCroassant()
    {
        return (croassants.Count < maxStack);
    }

    public void PushCroassant(Croassant croassant)
    {
        croassants.Push(croassant);
    }
}
