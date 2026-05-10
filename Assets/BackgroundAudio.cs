using UnityEngine;

public class BackgroundAudio : MonoBehaviour
{
    [Header("Background Music")]
    public AudioClip backgroundClip;

    [Range(0f, 1f)]
    public float volume = 0.5f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f; // 2D sound

        if (backgroundClip != null)
            audioSource.Play();
        else
            Debug.LogWarning("BackgroundAudio: No background clip assigned!");
    }
}