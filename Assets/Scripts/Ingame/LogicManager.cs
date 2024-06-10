using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 1. ¿Àºì °­Á¶
        TutorialManager.instance.PlayTutorial();   
    }
}
