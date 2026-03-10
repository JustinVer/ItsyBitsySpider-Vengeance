using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] Slider playerHealth;
    [SerializeField] TMP_Text rainTimer;
    [SerializeField] GameObject rainCloud;
    [SerializeField] GameObject[] webIcons;
    [SerializeField] GameObject grappleCurser;
    private bool timerActive = true;

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

        if (timerActive)
        {
            string displayTime = "3:00";
            if (timeInSeconds <= 0 && timeInSeconds > -1) timeInSeconds -= 1;
            int minutes = (int)(timeInSeconds / 60);
            int seconds = (int)(timeInSeconds % 60);
            if (timeInSeconds >= 0) displayTime = string.Format("{0:0}:{1:00}", Math.Abs(minutes), Math.Abs(seconds));
            else displayTime = string.Format("-{0:0}:{1:00}", Math.Abs(minutes), Math.Abs(seconds));

            rainTimer.SetText(displayTime);

            float colorNum = Mathf.Clamp(((timeInSeconds) / 255), 0, 1);
            rainCloud.GetComponent<Image>().color = new Color(colorNum, colorNum, colorNum, 1);
        }
        UpdateWebDisplay(GameplayManager.Instance.PlayerBody.CurrentWebs);

        Vector2 curserPos = Vector2.zero;
        if (GameplayManager.Instance.PlayerBody.ValidGrapplePoint)
        {
            grappleCurser.SetActive(true);
            RectTransform canvasRect = GetComponentInChildren<RectTransform>();
            Vector3 grapplePos = GameplayManager.Instance.PlayerBody.TargetGrapplePoint;
            Vector2 viewportPos = GameplayManager.Instance.PlayerManager.Playercam.WorldToViewportPoint(grapplePos);
            Vector2 screenPos = new Vector2(
                ((viewportPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
                ((viewportPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));
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


}
