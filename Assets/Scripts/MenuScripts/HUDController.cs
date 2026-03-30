using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] Slider playerHealth;
    [SerializeField] TMP_Text rainTimer;
    [SerializeField] TMP_Text scoreUI;
    [SerializeField] GameObject rainCloud;
    [SerializeField] GameObject[] webIcons;
    [SerializeField] GameObject grappleCurser;
    [SerializeField] RectTransform[] movingWebIcons;
    private bool[] movingWebIconMoving;
    private Vector2 webIconEndPosition;
    [SerializeField] private float webIconMoveTime = 3.0f;
    [SerializeField] private RectTransform canvasTransform;

    private float timeInSeconds = 180f;
    public float TimeInSeconds
    {
        get { return timeInSeconds; }
        set
        {
            timeInSeconds = value;
        }
    }

    private int web;

    private void Awake()
    {
        if (movingWebIconMoving == null)
        {
            movingWebIconMoving = new bool[movingWebIcons.Length];
            RectTransform parentTransform = webIcons[0].transform.parent.GetComponent<RectTransform>();
            webIconEndPosition = parentTransform.anchoredPosition - (parentTransform.sizeDelta * 0.5f);
        }
    }

    private void OnEnable()
    {
        playerHealth.value = 1;
        Update();
    }

    private void Update()
    {
        if (GameplayManager.Instance.Player && GameplayManager.Instance.Player.gameObject.activeInHierarchy)
        {
            playerHealth.value = Mathf.Clamp01(GameplayManager.Instance.PlayerBody.getHP() / GameplayManager.Instance.PlayerBody.getMaxHP());
        }

        string displayTime = "3:00";
        if (timeInSeconds <= 0 && timeInSeconds > -1) timeInSeconds -= 1;
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        if (timeInSeconds >= 0) displayTime = string.Format("{0:0}:{1:00}", Math.Abs(minutes), Math.Abs(seconds));
        else displayTime = string.Format("-{0:0}:{1:00}", Math.Abs(minutes), Math.Abs(seconds));

        rainTimer.SetText(displayTime);

        string displayScore = "Score: " + GameplayManager.Instance.score;
        scoreUI.SetText(displayScore);

        float colorNum = Mathf.Clamp(((timeInSeconds) / 255), 0, 1);
        rainCloud.GetComponent<Image>().color = new Color(colorNum, colorNum, colorNum, 1);
        UpdateWebDisplay(GameplayManager.Instance.PlayerBody.CurrentWebs);

        Vector2 curserPos = Vector2.zero;
        if (GameplayManager.Instance.PlayerBody.ValidGrapplePoint)
        {
            grappleCurser.SetActive(true);
            Vector3 grapplePos = GameplayManager.Instance.PlayerBody.TargetGrapplePoint;
            Vector2 viewportPos = GameplayManager.Instance.PlayerManager.Playercam.WorldToViewportPoint(grapplePos);

            Vector2 screenPos = new Vector2(
                ((viewportPos.x * canvasTransform.sizeDelta.x) - (canvasTransform.sizeDelta.x * 0.5f)),
                ((viewportPos.y * canvasTransform.sizeDelta.y) - (canvasTransform.sizeDelta.y * 0.5f)));
            grappleCurser.GetComponent<RectTransform>().anchoredPosition = screenPos;
        }
        else
        {
            grappleCurser.SetActive(false);
        }
    }

    public void UpdateWebDisplay(float webNum)
    {
        for (int i = 0; i < webIcons.Length; i++)
        {
            if (i < webNum) webIcons[i].SetActive(true);
            else webIcons[i].SetActive(false);
        }
    }

    public IEnumerator gainWeb(Vector3 worldPosition)
    {
        int i = 0;
        for (; i < movingWebIconMoving.Length; i++)
        {
            if (!movingWebIconMoving[i])
            {
                movingWebIconMoving[i] = true;
                break;
            }
            if (i == movingWebIconMoving.Length - 1)
            {
                yield break;
            }
        }
        movingWebIcons[i].gameObject.SetActive(true);
        Vector2 viewportPos = GameplayManager.Instance.PlayerManager.Playercam.WorldToViewportPoint(worldPosition);
        Vector2 screenStartPos = new Vector2(
                ((viewportPos.x * canvasTransform.sizeDelta.x) - (canvasTransform.sizeDelta.x * 0.5f)),
                ((viewportPos.y * canvasTransform.sizeDelta.y) - (canvasTransform.sizeDelta.y * 0.5f)));
        movingWebIcons[i].anchoredPosition = screenStartPos;

        float t = 0.0f;

        while (true)
        {
            t += Time.deltaTime / webIconMoveTime;
            if (t < 1)
            {
                movingWebIcons[i].anchoredPosition = Vector2.Lerp(screenStartPos, webIconEndPosition, t);
                yield return null;
            }
            else
            {
                break;
            }
        }
        movingWebIconMoving[i] = false;
        GameplayManager.Instance.PlayerBody.CurrentWebs++;
        UpdateWebDisplay(GameplayManager.Instance.PlayerBody.CurrentWebs);
        movingWebIcons[i].gameObject.SetActive(false);
    }


}
