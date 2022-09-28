using UnityEngine;

[System.Serializable]
public class Sound {

    [SerializeField]
    public string name;
    
    [SerializeField]
    public AudioClip clip;

    [Range(0f, 1f)]
    [SerializeField]
    public float volume;
    
    [Range(-3f, 3f)]
    [SerializeField]
    public float pitch;
    
    [HideInInspector]
    public AudioSource source;

    public bool loop;

}