using UnityEngine;
using UnityEngine.Audio;

public class AudioHandler : MonoBehaviour
{
    AudioSource audioSource;

    bool followObject;
    Transform sourceObject;
    Vector3 offset;

    bool playCalled = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayHandler(AudioData audioData, Transform parent, AudioMixerGroup mixerGroup = null, int clipNum = 0, bool followParent = false, Vector3 offset = default(Vector3))
    {
        playCalled = true;

        followObject = followParent;
        sourceObject = parent;
        this.transform.position = parent.position + offset;
        this.transform.rotation = parent.rotation;
        this.offset = offset;
        audioData.ApplyTo(audioSource, clipNum);

        audioSource.Play();
    }

    public void StopHandler()
    {
        playCalled = false;
        followObject = false;
        audioSource.Stop();
        AudioManager.Instance.returnSource(audioSource);
    }

    private void Update()
    {
        if (followObject)
        {
            this.transform.position = sourceObject.position + offset;
        }

        if (playCalled && !audioSource.isPlaying)
        {
            StopHandler();
        }
    }
}
