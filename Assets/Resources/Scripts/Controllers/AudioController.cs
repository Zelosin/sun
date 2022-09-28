using System;
using UnityEngine;

public class AudioController : MonoBehaviour {

    [SerializeField]
    private Sound[] sounds;
    
    private void Awake() {
        foreach (var sound in sounds) {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.loop = sound.loop;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
        }
    }

    public void play(string name) {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        sound.source.volume = SettingsStore.SOUND_VOLUME;
        sound.source.Play();
    }
}