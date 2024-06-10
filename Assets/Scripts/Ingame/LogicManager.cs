using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicManager : MonoBehaviour
{
    public static LogicManager instance { get; private set; }

    [SerializeField] GameObject goEtcPrice;

    private void Awake()
    {
        instance = this;
    }

    public void SetActiveEtcPrice(bool enable)
    {
        goEtcPrice.SetActive(enable);
    }

    // Start is called before the first frame update
    void Start()
    {
        // 1. ¿Àºì °­Á¶
        TutorialManager.instance.PlayTutorial();   
    }
}
