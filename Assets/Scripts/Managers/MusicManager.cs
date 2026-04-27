using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource;
    [Header("Audio Clips")]
    public AudioClip bgMusic;
    public AudioClip[] meowSFX; // randomize between these for cat sounds

    [Header("Volume")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    void Start()
    {
        PlayMusic(bgMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        musicSource.PlayOneShot(clip, musicVolume);
    }
}
