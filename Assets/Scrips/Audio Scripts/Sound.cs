using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class Sound {

    public string audioName;
    public AudioClip clip;
    public AudioMixerGroup mixerGroup;

    [Range(0, 1)] public float volume = 1f;
    [Range(.3f,3f)] public float pitch;
    public bool loop;

    [HideInInspector] public AudioSource source;

}