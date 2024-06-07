using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : InteractionObject
{
    const int maxStackCount = 8;

    public int currCroassantCount { get; private set; }
    public Vector3 stackStartPos => new Vector3(-0.37505f, 0.263999999f, 0.588999987f);
    public Vector3 stackGap => new Vector3(0.75f, 0f, -0.354f);

    Stack<Croassant> croassants = new Stack<Croassant>();

    Player player;
    public override void OnPlayerEnter(Player player)
    {
        this.player = player;
    }

    public override void OnPlayerExit(Player player)
    {
        this.player = null;
    }

    float cooldown;
    private void Update()
    {
        cooldown += Time.deltaTime;
        if (cooldown >= 0.1f)
        {
            if (player == null)
                return;

            if (player.currCroassantCount <= 0)
                return;

            if (currCroassantCount >= maxStackCount)
                return;

            var croassant = player.PopCroassant();
            var x = currCroassantCount % 2;
            var z = currCroassantCount / 2;
            var targetPos = stackStartPos + new Vector3(x * stackGap.x, 0f, z * stackGap.z);


            croassants.Push(croassant);
            currCroassantCount++;

            croassant.SetParent(transform)
                     .MoveToTargetWithCurve(targetPos, 0.2f);

            cooldown = 0f;
        }
    }
}
