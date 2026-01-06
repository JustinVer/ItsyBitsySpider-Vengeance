using UnityEngine;

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
}
