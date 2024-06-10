using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPillar : InteractionObject, ITutorialTarget
{
    [SerializeField] GameObject tutorialPoint;
    public MoneyStackPool moneyStackPool => PoolContainer.instance.GetPool<MoneyStackPool>();

    Stack<MoneyStack> moneyStacks = new Stack<MoneyStack>();
    Stack<int> moneyAmountStack = new Stack<int>();

    const float stackGap = 0.27f;
    public bool isPlayerOn { get; private set; }

    public int tutorialIndex => 3;
    public Transform tutorialTarget => transform;
    public GameObject goTargetPoint => tutorialPoint;

    public void SpawnMoneyStack()
    {
        var targetPos = transform.position + new Vector3(0f, moneyStacks.Count * stackGap, 0f);
        var moneyStack = moneyStackPool.Spawn(targetPos, Quaternion.identity, null);

        moneyStacks.Push(moneyStack);
    }

    public void DespawnMoneyStack(MoneyStack moneyStack)
    {
        moneyStackPool.Despawn(moneyStack);
    }

    public void IncreaseMoney(int amount)
    {
        moneyAmountStack.Push(amount);
    }

    public override Vector3 GetPos(int index)
    {
        return Vector3.zero;
    }

    Player player;
    bool hasTutorialEnter;
    public override void OnInteractantEnter(Interactant interactant)
    {
        player = interactant as Player;
        if (!hasTutorialEnter)
        {
            if (TutorialManager.instance.tutorialIndex == 4)
            {
                TutorialManager.instance.PlayTutorial();
                hasTutorialEnter = true;
            }
        }
    }

    public override void OnInteractantExit(Interactant interactant)
    {
        player = null;
    }

    float cooldown;
    private void Update()
    {
        isPlayerOn = player != null;
        cooldown += Time.deltaTime;
        if (cooldown >= 0.02f)
        {
            if (player == null)
                return;

            if (moneyStacks.Count <= 0)
                return;

            var moneyStack = moneyStacks.Pop();
            var moneyAmount = moneyAmountStack.Pop();
            moneyStack.PlayMoneyToPlayerAnim(player.position, DespawnMoneyStack);
            player.IncreaseMoney(moneyAmount);
            cooldown = 0f;
        }
    }
}
