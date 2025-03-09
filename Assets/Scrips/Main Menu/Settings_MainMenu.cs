using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings_MainMenu : MonoBehaviour {

    [SerializeField] private int volumeOffSet = 80;

    [Header("Audio Mixer Related")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string[] volumeStrings;
    [SerializeField] private Slider[] volumeSliders;
    [SerializeField] private TMP_Text[] volumeTexts;

    [SerializeField] private Toggle muteToggle;


    private void Start() {
        // 0 master, 1 music, sfx 2
        for(int i = 0; i < volumeSliders.Length; i++) {
            volumeSliders[i].value = PlayerPrefs.GetFloat("Volume" + i, 20);
            mixer.SetFloat(volumeStrings[i], PlayerPrefs.GetFloat("Volume" + i));
            int index = i;
            volumeSliders[i].onValueChanged.AddListener(value => OnValueChanged(value, index));

            volumeTexts[i].text = ((int)(PlayerPrefs.GetFloat("Volume" + i, 20) + volumeOffSet)).ToString();
        }

        muteToggle.onValueChanged.AddListener(OnMuteToggle);
        muteToggle.gameObject.GetComponent<CustomToggle>().ChangeState(LoadMuteState());

        AudioManager.instance.PlaySound("Background");
    }

    private void OnMuteToggle(bool value) {
        if(value) {
            mixer.SetFloat(volumeStrings[0], -80); // setting to mute master volume
            PlayerPrefs.SetInt("Mute", 1);
        } else {
            mixer.SetFloat(volumeStrings[0], PlayerPrefs.GetFloat("Volume0", 20));
            PlayerPrefs.SetInt("Mute", 0);
        }
    }

    private void OnValueChanged(float value, int index) {
        PlayerPrefs.SetFloat("Volume" + index, value);
        volumeTexts[index].text = ((int)volumeSliders[index].value + volumeOffSet).ToString();

        mixer.SetFloat(volumeStrings[index], value);
    }

    public bool LoadMuteState() {
        return PlayerPrefs.GetInt("Mute", 0) == 1; // Convert int back to bool (default is false)
    }

}