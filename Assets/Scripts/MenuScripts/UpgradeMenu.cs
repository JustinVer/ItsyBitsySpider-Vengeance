using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
    #region Singleton instance
    private static UpgradeMenu instance;

    public static UpgradeMenu Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<UpgradeMenu>();
            }
            if (instance == null)
            {
                Debug.LogWarning("NO UPGRADE MENU FOUND");
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    #endregion

    [SerializeField] GameObject upgradeMenu;
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject upgradePanel;

    [SerializeField] GameObject levelLoader;
    public bool betweenRuns = true;

    public void activateMenu()
    {
        upgradeMenu.SetActive(true);
        betweenRuns = true;
        ActiveCameraManager.Instance.GoToCamera1();
    }

    public void StartRun()
    {
        GameplayManager.Instance.SetupForStart();
        NewSegmentRandomizer.Instance.StartRun();
        //NewSegmentRandomizer.Instance.LoadBoss();
        ActiveCameraManager.Instance.GoToCamera2();
        betweenRuns = false;
        upgradeMenu.SetActive(false);
    }

    public void GoToUpgrades()
    {
        mainPanel.SetActive(false);
        upgradePanel.SetActive(true);
    }

    public void ActivateGlide()
    {

    }

    public void ActivateDash()
    {

    }

    public void ActivateGrapple()
    {

    }

    public void ExitUpgrades()
    {
        mainPanel.SetActive(true);
        upgradePanel.SetActive(false);
    }

    public void GoToTestRange()
    {
        //TODO: open the testRange
        GameplayManager.Instance.SetupForStart();
        NewSegmentRandomizer.Instance.LoadTutorial();
        ActiveCameraManager.Instance.SwitchCameras();
        betweenRuns = false;
        GameplayManager.Instance.PauseTimer();
        upgradeMenu.SetActive(false);
    }

    public void QuitGame()
    {
        StartCoroutine(GameplayManager.Instance.LoadLevel("StartGameScene"));

        //#if UNITY_EDITOR
        //        EditorApplication.isPlaying = false;
        //#else
        //        Application.Quit();
        //#endif
    }
}
