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

    public bool CanPause = false;
    private bool hasChanged = false;
    private bool isPaused = false;

    private void Update()
    {
        if (GameplayManager.Instance.Escape && !hasChanged && CanPause)
        {
            hasChanged = true;
            isPaused = !isPaused;
            PauseGame(isPaused);
        }
        if (!GameplayManager.Instance.Escape)
        {
            hasChanged = false;
        }
    }
    private void PauseGame(bool pause)
    {
        isPaused = pause;
        pauseCanvas.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        LockUnlockCursor(isPaused);
        if (isPaused)
        {
            GameplayManager.Instance.setInputActionToUI();
        }
        else
        {
            GameplayManager.Instance.setInputActionToPlayer();
        }
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
        Time.timeScale = 1f;
        GameplayManager.Instance.ResetGame();
        pauseCanvas.SetActive(false);
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
