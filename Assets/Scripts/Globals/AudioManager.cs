using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton instance
    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<AudioManager>();
            }
            if (instance == null)
            {
                throw new System.Exception("NO AUDIO MANAGER FOUND");
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    #endregion

    [SerializeField] private int numSources = 15;
    [SerializeField] private AudioSource backgroundAudioSource1;
    [SerializeField] private AudioSource backgroundAudioSource2;
    private int primaryBackgroundSource = 0;
    Coroutine backgroundTransitionCoroutine;
    private float backgroundAudioVolume;

    [SerializeField] private GameObject audioSourcePrefab;

    private Queue<AudioSource> sources = new Queue<AudioSource>();
    private List<AudioSource> usedsources = new List<AudioSource>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        for (int i = 0; i < numSources; i++)
        {
            createNewSource();
        }
        backgroundAudioVolume = backgroundAudioSource1.volume;
        if (GameplayManager.Instance != null)
        {
            GameplayManager.Instance.onLevelReset += sceneReset;
        }
    }

    public AudioSource getSource()
    {
        if (sources.Count == 0)
        {
            createNewSource();
        }
        AudioSource Source = sources.Dequeue();
        usedsources.Add(Source);
        Source.gameObject.SetActive(true);
        return Source;
    }

    public void returnSource(AudioSource Source)
    {
        if (Source.GetType() == typeof(AudioSource) && usedsources.Contains(Source))
        {
            Source.Stop();
            Source.gameObject.SetActive(false);
            usedsources.Remove(Source);
            sources.Enqueue(Source);
        }
    }

    private void createNewSource()
    {
        GameObject Source = Instantiate(audioSourcePrefab, this.transform);
        Source.SetActive(false);
        sources.Enqueue(Source.GetComponent<AudioSource>());
    }

    public void setBackgroundMusic(AudioClip audioClip)
    {
        if (primaryBackgroundSource == 0)
        {
            backgroundAudioSource1.clip = audioClip;
            backgroundAudioSource1.Play();
        }
        else
        {
            backgroundAudioSource2.clip = audioClip;
            backgroundAudioSource2.Play();
        }
    }

    public void transitionBackgroundMusic(AudioClip audioClip, float transitionTime)
    {
        if (primaryBackgroundSource == 0)
        {
            primaryBackgroundSource = 1;
        }
        else
        {
            primaryBackgroundSource = 0;
        }

        setBackgroundMusic(audioClip);

        if (backgroundTransitionCoroutine != null)
        {
            StopCoroutine(backgroundTransitionCoroutine);
        }

        if (primaryBackgroundSource == 0)
        {
            backgroundTransitionCoroutine = StartCoroutine(CrossfadeCoroutine(backgroundAudioSource2, backgroundAudioSource1, transitionTime));
        }
        else
        {
            backgroundTransitionCoroutine = StartCoroutine(CrossfadeCoroutine(backgroundAudioSource1, backgroundAudioSource2, transitionTime));
        }
    }

    private IEnumerator CrossfadeCoroutine(AudioSource currentsource, AudioSource nextSource, float transitionTime)
    {
        // Ensure both tracks are ready
        nextSource.volume = 0f;

        float startingCurrentVolume = currentsource.volume;

        if (!nextSource.isPlaying) nextSource.Play();

        float elapsed = 0f;

        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionTime);

            // Linear fade formula
            currentsource.volume = startingCurrentVolume * (1 - t);
            nextSource.volume = backgroundAudioVolume * t;

            yield return null;
        }

        // Ensure final state
        currentsource.volume = 0f;
        nextSource.volume = backgroundAudioVolume;
        currentsource.Stop();
    }

    public void sceneReset()
    {
        while (usedsources.Count > 0)
        {
            returnSource(usedsources[0]);
        }
    }

    private void OnDestroy()
    {
        if (GameplayManager.Instance != null)
            GameplayManager.Instance.onLevelReset -= sceneReset;
    }
}
