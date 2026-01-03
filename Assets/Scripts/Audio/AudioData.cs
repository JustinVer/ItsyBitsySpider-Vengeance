using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewAudioData", menuName = "Audio/Audio Data")]
public class AudioData : ScriptableObject
{
    [Header("Audio Clip")]
    public AudioClip[] clips;

    [Header("Volume & Pitch")]
    [Range(0f, 1f)] public float volume = 1f;
    [Range(-3f, 3f)] public float pitch = 1f;
    [Range(0f, 1f)] public float volumeVariance = 0f;
    [Range(0f, 1f)] public float pitchVariance = 0f;

    [Header("Looping & Play Settings")]
    public bool loop = false;
    public bool playOnAwake = false;

    [Header("3D Sound Settings")]
    [Range(0f, 1f)] public float spatialBlend = 1f;   // 1 = fully 3D
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
    public float minDistance = 1f;
    public float maxDistance = 500f;

    [Header("Priority")]
    [Range(0, 256)] public int priority = 128; // 0 = highest priority

    [Header("Doppler")]
    [Range(0f, 5f)] public float dopplerLevel = 1f;

    [Header("Stereo Pan")]
    [Range(-1f, 1f)] public float stereoPan = 0f;

    [Header("Mixer Group")]
    public AudioMixerGroup mixerGroup;

    /// <summary>
    /// Applies all settings to an AudioSource.
    /// </summary>
    public void ApplyTo(AudioSource source, int clipNum = 0)
    {
        if (source == null) return;

        source.clip = clips[clipNum];
        source.volume = volume * (1f + Random.Range(-volumeVariance, volumeVariance));
        source.pitch = pitch * (1f + Random.Range(-pitchVariance, pitchVariance));

        source.loop = loop;
        source.playOnAwake = playOnAwake;

        source.spatialBlend = spatialBlend;
        source.rolloffMode = rolloffMode;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;

        source.priority = priority;
        source.dopplerLevel = dopplerLevel;
        source.panStereo = stereoPan;
        source.outputAudioMixerGroup = mixerGroup;
    }
}
