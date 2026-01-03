using UnityEngine;

public class GameOver : MonoBehaviour
{
    public void Start()
    {
        GameManager.Instance.onLevelReset += sceneReset;
    }
    public void PlayAgain()
    {
        GameManager.Instance.ResetScene();
    }

    public void MainMenu()
    {
        GameManager.Instance.LoadMainMenu();
    }

    public void sceneReset()
    {
        this.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.onLevelReset -= sceneReset;
    }
}
