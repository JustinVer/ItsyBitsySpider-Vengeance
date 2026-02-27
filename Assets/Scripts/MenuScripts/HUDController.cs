using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] Slider playerHealth;
    [SerializeField] TMP_Text rainTimer;
    [SerializeField] GameObject[] webIcons;

    private bool timerActive = true;
    [SerializeField] private float timeInSeconds = 180f;

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
            timeInSeconds -= Time.deltaTime;
            if (timeInSeconds <= 0 && timeInSeconds > -1) timeInSeconds -= 1;
            int minutes = (int)(timeInSeconds / 60);
            int seconds = (int)(timeInSeconds % 60);
            if (timeInSeconds >= 0) displayTime = string.Format("{0:0}:{1:00}", Math.Abs(minutes), Math.Abs(seconds));
            else displayTime = string.Format("-{0:0}:{1:00}", Math.Abs(minutes), Math.Abs(seconds));

            rainTimer.SetText(displayTime);
        }
    }

    public void UpdateWebDisplay(int webNum)
    {
        for (int i = 0; i < webIcons.Length; i++)
        {
            if (i < webNum) webIcons[i].SetActive(true);
            else webIcons[i].SetActive(false);
        }
    }
}
