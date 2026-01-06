using UnityEngine;

public class GameOver : MonoBehaviour
{
    public void Start()
    {
        GameplayManager.Instance.onLevelReset += sceneReset;
    }
    public void PlayAgain()
    {
        GameplayManager.Instance.ResetScene();
    }

    public void MainMenu()
    {
        MenuManager.Instance.LoadMainMenu();
    }

    public void sceneReset()
    {
        this.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (GameplayManager.Instance != null)
            GameplayManager.Instance.onLevelReset -= sceneReset;
    }
}
