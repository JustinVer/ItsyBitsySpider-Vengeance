using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] Slider playerHealth, enemiesRemaining;
    [SerializeField] TMP_Text rainTimer;
    [SerializeField] GameObject webIcons;

    private int maxNumEnemies = 1;

    private bool timerActive = true;
    private float timeInSeconds = 10f;

    private int web;

    private void Start()
    {
        GameplayManager.Instance.onLevelReset += SceneReset;
    }
    private void OnEnable()
    {
        playerHealth.value = 1;
        enemiesRemaining.value = 1;

        Update();
    }

    private void Update()
    {
        if (GameplayManager.Instance.Player && GameplayManager.Instance.Player.gameObject.activeInHierarchy)
        {
            playerHealth.value = Mathf.Clamp01(GameplayManager.Instance.PlayerBody.getHP() / GameplayManager.Instance.PlayerBody.getMaxHP());
        }

        if (LevelManager.Instance)
        {
            if (maxNumEnemies < LevelManager.Instance.numEnemies)
            {
                maxNumEnemies = LevelManager.Instance.numEnemies;
            }
            Debug.Log("MAx numbers of enemies: " + LevelManager.Instance.numEnemies + " " + maxNumEnemies);
            enemiesRemaining.value = (float)LevelManager.Instance.numEnemies / (float)maxNumEnemies;
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

    public void SceneReset()
    {
        maxNumEnemies = 1;
    }

    private void OnDestroy()
    {
        if (GameplayManager.Instance != null)
            GameplayManager.Instance.onLevelReset -= SceneReset;
    }
}
