using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] Button continueButton;
    private void OnEnable()
    {
        AudioManager.Instance.transitionBackgroundMusic(mainMenuMusic, 0.5f);
        if (FileHandler.SavePresent)
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
        }
        //GameManager.Instance.PlayerInput.enabled = false;
    }
    public void NewGame()
    {
        GameManager.Instance.StartNewGame();
    }

    public void Continue()
    {
        GameManager.Instance.ContinueGame();
    }

    public void Options()
    {
        GameManager.Instance.OptionsMenu(true);
    }

    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        Application.Quit();
#endif
    }
}
