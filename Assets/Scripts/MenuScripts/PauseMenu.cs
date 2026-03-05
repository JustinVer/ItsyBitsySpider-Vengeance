using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject pausePanel;

    [SerializeField] AudioMixer audioMixer;

    [SerializeField] Slider masterVolume;
    [SerializeField] Slider soundEffectsVolume;
    [SerializeField] Slider backgroundMusicVolume;

    public bool CanPause = true;
    private bool isPaused = false;

    private void Update()
    {
        if (GameplayManager.Instance.Escape && CanPause)
        {
            isPaused = !isPaused;
            PauseGame(isPaused);
        }
    }
    private void PauseGame(bool pause)
    {
        isPaused = pause;
        pauseCanvas.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        LockUnlockCursor(isPaused);
        GameplayManager.Instance.PlayerInput.enabled = !isPaused;
    }

    private void LockUnlockCursor(bool isPaused)
    {
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void Continue()
    {
        isPaused = !isPaused;
        PauseGame(isPaused);
    }

    public void Settings()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ExitSettings()
    {
        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void SaveGame()
    {
        //FileHandler.SaveGame();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        Application.Quit();
#endif
    }
    public void masterVolumeSlider()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume.value);
        audioMixer.SetFloat("MasterVolume", masterVolume.value);
        PlayerPrefs.Save();
    }
    public void soundEffectsVolumeSlider()
    {
        PlayerPrefs.SetFloat("SoundEffectsVolume", soundEffectsVolume.value);
        audioMixer.SetFloat("SoundEffectsVolume", soundEffectsVolume.value);
        PlayerPrefs.Save();
    }
    public void backgroundVolumeSlider()
    {
        PlayerPrefs.SetFloat("BackgroundMusicVolume", backgroundMusicVolume.value);
        audioMixer.SetFloat("BackgroundMusicVolume", backgroundMusicVolume.value);
        PlayerPrefs.Save();
    }
}
