using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour {
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundVolumeSlider;
    [SerializeField] private Slider speechSpeedSlider;

    private void Awake() {
        musicVolumeSlider.onValueChanged.AddListener(newValue => { SettingsStore.MUSIC_VOLUME = newValue; });
        soundVolumeSlider.onValueChanged.AddListener(newValue => { SettingsStore.SOUND_VOLUME = newValue; });
        speechSpeedSlider.onValueChanged.AddListener(newValue => { SettingsStore.SPEECH_SPEED = newValue; });

        musicVolumeSlider.value = SettingsStore.MUSIC_VOLUME;
        soundVolumeSlider.value = SettingsStore.SOUND_VOLUME;
        speechSpeedSlider.value = SettingsStore.SPEECH_SPEED;
    }
}