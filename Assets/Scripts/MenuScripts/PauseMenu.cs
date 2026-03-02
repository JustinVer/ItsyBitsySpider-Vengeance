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
        LockUnlockCursor(isPaused);
        GameplayManager.Instance.PlayerInput.enabled = !isPaused;
    }

    private void LockUnlockCursor(bool isPaused)
    {
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else
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
        //TODO: go to settings menu
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
}
