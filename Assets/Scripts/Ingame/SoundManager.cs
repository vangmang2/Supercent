using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    AudioSource[] audioArr;
    Dictionary<SoundType, SoundSource> soundDic = new Dictionary<SoundType, SoundSource>();

    int currAudioIndex;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioArr = GetComponentsInChildren<AudioSource>();
        var sounds = GetComponentsInChildren<SoundSource>();
        foreach (var sound in sounds)
        {
            soundDic.Add(sound.type, sound);
        }
    }

    public void PlaySFX(SoundType type)
    {
        if (currAudioIndex >= audioArr.Length)
            currAudioIndex = 0;

        var clip = soundDic[type].clip;
        var targetAudio = audioArr[currAudioIndex];
        targetAudio.clip = clip;
        targetAudio.Play();

        currAudioIndex++;
    }
}
