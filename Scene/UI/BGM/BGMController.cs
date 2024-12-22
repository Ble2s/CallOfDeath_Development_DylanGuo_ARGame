using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
    public AudioSource bgmSource;  // Reference to the AudioSource component for playing the background music
    public AudioClip bgmClip;      // The background music file to be played
    public float initialDelay = 2f; // Delay (in seconds) before the music starts playing
    public float volume = 0.5f;     // Initial volume level (range: 0 to 1)

    void Start()
    {
        if (bgmSource == null)
        {
            Debug.LogError("AudioSource is not assigned!"); // Check if the AudioSource is assigned
            return;
        }

        bgmSource.clip = bgmClip;   // Assign the AudioClip to the AudioSource
        bgmSource.loop = true;     // Enable looping for the music
        bgmSource.volume = volume; // Set the initial volume level
        StartCoroutine(PlayMusicWithDelay(initialDelay)); // Start the music with a delay
    }

    // Coroutine to play music after a delay
    private System.Collections.IEnumerator PlayMusicWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        bgmSource.Play();                       // Start playing the music
    }

    // Method to adjust the music volume dynamically
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume); // Ensure the volume is between 0 and 1
        bgmSource.volume = volume;        // Apply the new volume to the AudioSource
    }
}
