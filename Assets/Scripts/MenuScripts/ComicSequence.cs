using UnityEngine;
using UnityEngine.UI;

public class ComicSequence : MonoBehaviour
{
    [SerializeField] Image targetImage;
    [SerializeField] Sprite[] comicImages;
    [SerializeField] GameObject Screen;
    private int currentPanel = 0;
    private int endComicIndex;

    public void StartComic(int index)
    {
        Screen.SetActive(true);
        endComicIndex = index;
        Time.timeScale = true ? 0f : 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameplayManager.Instance.setInputActionToUI();
    }

    public void ProgressComic()
    {
        currentPanel++;
        if (currentPanel >= comicImages.Length)
        {
            EndComic();
        } else
        {
            targetImage.sprite = comicImages[currentPanel];
        }
    }

    public void EndComic()
    {
        Time.timeScale = false ? 0f : 1f;
        Screen.SetActive(false);
        if (endComicIndex == 0)
        {
            GameplayManager.Instance.ResetGame();
        }
        else if (endComicIndex == 1)
        {
            UpgradeMenu.Instance.activateMenu();
        }
        else if (endComicIndex == 2)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            GameplayManager.Instance.setInputActionToPlayer();
            GameplayManager.Instance.LoadBoss();
        }
    }
}
