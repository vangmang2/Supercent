using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class Basket : InteractionObject
{
    const int maxStackCount = 8;
    public override InteractionObjectType interactionObjectType => InteractionObjectType.basket;

    public int currWaitingCount { get; private set; }

    public int currCroassantCount { get; private set; }
    public Vector3 stackStartPos => new Vector3(-0.37505f, 0.263999999f, 0.588999987f);
    public Vector3 stackGap => new Vector3(0.75f, 0f, -0.354f);


    Stack<Croassant> croassants = new Stack<Croassant>();
    List<Interactant> interactants = new List<Interactant>();

    public Basket IncreaseWaitingCount()
    {
        currWaitingCount++;
        return this;
    }

    public Basket DecreaseWaitingCount()
    {
        currWaitingCount--;
        return this;
    }

    public Vector3 TargetWaitingPosition(int index)
    {
        return waitingPosList[index].position;
    }

    public override void OnInteractantEnter(Interactant interactant)
    {
        AddInteractant(interactant);
    }

    public override void OnInteractantExit(Interactant interactant)
    {
        RemoveInteractant(interactant);
    }

    public void AddInteractant(Interactant interactant)
    {
        interactants.Add(interactant);
    }

    public void RemoveInteractant(Interactant interactant)
    {
        interactants.Remove(interactant);
    }

    float cooldown;
    private void Update()
    {
        cooldown += Time.deltaTime;
        if (cooldown >= 0.15f)
        {
            foreach (var interactant in interactants)
            {
                if (interactant.interactantType == InteractantType.giver)
                {
                    if (interactant.currCroassantCount <= 0)
                        continue;

                    if (currCroassantCount >= maxStackCount)
                        continue;

                    var croassant = interactant.PopCroassant();
                    var x = currCroassantCount % 2;
                    var z = currCroassantCount / 2;
                    var targetPos = stackStartPos + new Vector3(x * stackGap.x, 0f, z * stackGap.z);

                    croassants.Push(croassant);
                    currCroassantCount++;

                    croassant.SetParent(transform)
                             .MoveToTargetWithCurve(targetPos, 0.2f);
                }
                else
                {
                    if (!interactant.CanPushCroassant())
                        continue;

                    if (croassants.Count <= 0)
                        continue;

                    var croassant = croassants.Pop();
                    var targetPos = interactant.stackStartPos + new Vector3(0f, interactant.stackGap * interactant.currCroassantCount);

                    interactant.PushCroassant(croassant);
                    currCroassantCount--;

                    croassant.SetParent(interactant.parent)
                             .SetActiveCollider(false)
                             .DestroyRigidbody()
                             .MoveToTargetWithCurve(targetPos, 0.2f);
                }
            }
            cooldown = 0f;
        }
    }
}
