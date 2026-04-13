using UnityEngine;

public class MusicManager : MonoBehaviour
{
    #region Singleton instance
    private static MusicManager instance;

    public static MusicManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<MusicManager>();
            }
            if (instance == null)
            {
                Debug.LogWarning("NO GAMEPLAY MANAGER FOUND");
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    #endregion



    [SerializeField] private AudioClip[] tracks;
    [SerializeField] private AudioSource backgroundAudioSource1;
    [SerializeField] private AudioSource backgroundAudioSource2;

    public void playTrack(int trackIndex)
    {
        if (trackIndex < 0 || trackIndex >= tracks.Length) return;


    }
}
