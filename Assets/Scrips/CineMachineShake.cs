using Unity.Cinemachine;
using UnityEngine;

public class CineMachineShake : MonoBehaviour {

    public static CineMachineShake Instance {  get; private set; }

    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
    private float shakeTimer;

    private void Awake() {
        cinemachineBasicMultiChannelPerlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
        if(Instance == null) Instance = this;
    }

    public void ShakeCamera(float intensity, float time) {

        cinemachineBasicMultiChannelPerlin.AmplitudeGain = intensity;
        shakeTimer = time;
    
    }


    private void Update() {
        if(shakeTimer > 0) {
            shakeTimer -= Time.deltaTime;
            if(shakeTimer <= 0) {
                shakeTimer = 0;
                cinemachineBasicMultiChannelPerlin.AmplitudeGain = 0f;
            }
        }
    }

}