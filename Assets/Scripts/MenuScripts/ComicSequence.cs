using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ComicSequence : MonoBehaviour
{
    [SerializeField] Image targetImage;
    [SerializeField] Image backImage;
    [SerializeField] Sprite[] comicImages;
    [SerializeField] GameObject Screen;
    [SerializeField] Button progress;
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
        }
        else
        {
            backImage.sprite = comicImages[currentPanel - 1];
            targetImage.color = new UnityEngine.Color(1, 1, 1, 0);
            targetImage.sprite = comicImages[currentPanel];
            StartCoroutine(FadeIn(targetImage, 0.5f));
        }
    }

    IEnumerator FadeIn(Image img, float duration)
    {
        progress.interactable = false;
        float elapsed = 0f;
        UnityEngine.Color startColor = img.color;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            startColor.a = Mathf.Clamp01(elapsed / duration);
            img.color = startColor;
            yield return null;
        }
        progress.interactable = true;
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
