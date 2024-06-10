using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    getObject,
    putObject,
    none
}

public class SoundSource : MonoBehaviour
{
    [SerializeField] SoundType _type;
    [SerializeField] AudioClip _clip;

    public SoundType type => _type;
    public AudioClip clip => _clip;
}
