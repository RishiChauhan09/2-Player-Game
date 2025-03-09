using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [HideInInspector] public static AudioManager instance;

    [SerializeField] private Sound[] sounds;

    private static Dictionary<string, float> soundTimeDict;

    public static void Initilize() {
        soundTimeDict = new Dictionary<string, float>();
        soundTimeDict["TankMoving"] = 0f;
    }

    private void Awake() {

        if(instance == null) instance = this;

        foreach(Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.pitch = s.pitch;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.mixerGroup;
        }
    }

    public void PlaySound(string name) {
        if(CanMove(name)) {
            Sound s = Array.Find(sounds, sound => sound.audioName == name);
            if(s == null) {
                Debug.LogWarning("Sound with name " + name + " not found");
                return;
            }
            s.source.Play();
        }
    }

    public void StopSound(string name) {
        Sound s = Array.Find(sounds, sound => sound.audioName == name);
        if(s == null) {
            Debug.LogWarning("Sound with name " + name + " not found");
            return;
        }
        s.source.Stop();
    }

    private static bool CanMove(string name) {
        switch(name) {
            case "TankMoving":
                if(soundTimeDict.ContainsKey(name)) {
                    float lastTimePlayed = soundTimeDict[name];
                    float playerMoveTimer = 1.7f;
                    if(lastTimePlayed + playerMoveTimer < Time.time) {
                        soundTimeDict[name] = Time.time;
                    } else {
                        return false;
                    }
                } else {
                    return true;
                }
                break;

            default:
                return true;
        }
        return true;
    }

}