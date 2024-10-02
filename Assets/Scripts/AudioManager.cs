using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource; // Assign in the Inspector
    public AudioClip[] songs; // Array of songs to switch between

    private bool isPlaying = false; // Track the playback state
    private int currentSongIndex = 0; // Index of the current song
    [SerializeField] private Slider musicSlider;
    public GameObject dropstext;
    public GameObject sliderobject;

    private void Start()
    {
        if (musicSource == null)
        {
            Debug.LogError("AudioSource is not assigned.");
            return;
        }

        if (songs.Length == 0)
        {
            Debug.LogError("No songs assigned.");
            return;
        }

        // Ensure the music starts paused
        musicSource.Pause();
        isPlaying = false;
    }

    // Call this function to toggle play/pause
    public void ToggleMusic()
    {
        if (musicSource == null)
        {
            Debug.LogError("AudioSource is not assigned.");
            return;
        }

        if (isPlaying)
        {
            musicSource.Pause();
        }
        else
        {
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }

        isPlaying = !isPlaying;
    }
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        musicSource.volume = volume;
    }
    // Call this function to change the song
    public Text musictext;
    public string song0;
    public string song1;
    public void ChangeSong(int index)
    {
        dropstext.SetActive(false);
        sliderobject.SetActive(true);
        if (index < 0 || index >= songs.Length)
        {
            Debug.LogError("Invalid song index.");
            return;
        }
        if (index == 0)
        {
            musictext.text = song0;
        }
        if (index == 1)
        {
            musictext.text = song1;
        }
        musicSource.clip = songs[index];
        currentSongIndex = index;
        isPlaying = false;
    }
}
