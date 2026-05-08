using UnityEngine;

/// <summary>
/// Attach to the STAR PARENT PREFAB (same GameObject as SphereCollider/StarRotation).
///
/// SETUP:
/// 1. Attach to your normal star prefab AND shooting star prefab.
/// 2. Assign all 6 AudioClips in the Inspector:
///      White Clip   → "white"  audio asset
///      Pink Clip    → "pink"   audio asset
///      Yellow Clip  → "yellow" audio asset
///      Red Clip     → "red"    audio asset
///      Green Clip   → "green"  audio asset
///      Blue Clip    → "blue"   audio asset
/// </summary>
public class StarAudio : MonoBehaviour
{
    [Header("Star Sound Clips")]
    public AudioClip whiteClip;
    public AudioClip pinkClip;
    public AudioClip yellowClip;
    public AudioClip redClip;
    public AudioClip greenClip;
    public AudioClip blueClip;

    [Range(0f, 1f)]
    public float volume = 1f;

    private AudioClip cachedClip;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = volume;
    }

    /// <summary>
    /// Called by StarSpawn right after it picks a color for this star.
    /// </summary>
    public void SetClip(AudioClip clip)
    {
        cachedClip = clip;
    }

    /// <summary>
    /// Called by StarSpawn.MoveSequence() when the star is captured.
    /// </summary>
    public void PlayCaptureSound()
    {
        if (audioSource == null || cachedClip == null)
        {
            Debug.LogWarning($"StarAudio on '{gameObject.name}': Cannot play — clip not set.");
            return;
        }
        audioSource.clip = cachedClip;
        audioSource.Play();
    }
}