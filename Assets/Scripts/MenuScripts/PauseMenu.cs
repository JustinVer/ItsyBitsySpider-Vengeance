using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseCanvas;
    public bool CanPause = true;
    private bool isPaused = false;

    private void Update()
    {
        if (Input.GetButtonDown("Cancel") && CanPause)
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
        GameplayManager.Instance.PlayerInput.enabled = !isPaused;
    }

    public void Continue()
    {
        isPaused = !isPaused;
        PauseGame(isPaused);
    }

    public void Settings()
    {
        //TODO: go to settings menu
    }

    public void SaveGame()
    {
        //TODO: Find save system in scripts
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        Application.Quit();
#endif
    }
}
