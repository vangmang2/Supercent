using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ITutorialTarget 
{
    int tutorialIndex { get; }
    Transform tutorialTarget { get; }
    GameObject goTargetPoint { get; }
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance { get; private set; }

    List<ITutorialTarget> targets;
    [SerializeField] Transform tfPointDirector, tfLevel;

    public int tutorialIndex { get; private set; }
    Transform currTarget;

    private void Awake()
    {
        instance = this;
        var tutorialTargets = tfLevel.GetComponentsInChildren<ITutorialTarget>();
        targets = tutorialTargets.OrderBy((target) => target.tutorialIndex).ToList();
    }

    public void PlayTutorial()
    {
        if (tutorialIndex > 0)
        {
            // 이전 포인트 제거
            var prev = targets[tutorialIndex - 1];
            prev.goTargetPoint.SetActive(false);
        }
        if (tutorialIndex >= targets.Count)
        {
            currTarget = null;
            return;
        }

        var next = targets[tutorialIndex];
        currTarget = next.tutorialTarget;
        next.goTargetPoint.SetActive(true);


        tutorialIndex++;
    }

    private void Update()
    {
        if (currTarget == null)
        {
            tfPointDirector.gameObject.SetActive(false);
        }
        else
        {
            tfPointDirector.gameObject.SetActive(true);
            var rot = Quaternion.LookRotation((tfPointDirector.position -  currTarget.position).normalized, tfPointDirector.up);
            rot.x = 0f;
            rot.z = 0f;
            tfPointDirector.rotation = rot;
        }
    }
}
