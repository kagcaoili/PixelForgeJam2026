using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource sfxSource;
    [Header("Audio Clips")]
    public AudioClip bgMusic;
    public AudioClip[] meowSFX; // randomize between these for cat sounds

    [Header("Volume")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float meowSfx = 0.5f;

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

    public void PlayRandomMeow()
    {
        if (meowSFX.Length == 0) return; // No clips assigned
        int randomIndex = Random.Range(0, meowSFX.Length);
        sfxSource.PlayOneShot(meowSFX[randomIndex], meowSfx);
        Debug.Log("Played meow SFX: " + meowSFX[randomIndex].name);
    }
}
