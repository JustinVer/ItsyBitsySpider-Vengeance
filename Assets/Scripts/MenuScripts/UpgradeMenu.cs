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

    public void activateMenu()
    {
        upgradeMenu.SetActive(true);
        ActiveCameraManager.Instance.SwitchCameras();
    }

    public void StartRun()
    {
        NewSegmentRandomizer.Instance.StartRun();
        ActiveCameraManager.Instance.SwitchCameras();
        GameplayManager.Instance.SetupForStart();
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

        ActiveCameraManager.Instance.SwitchCameras();
    }

    public void QuitGame()
    {

    }
}
