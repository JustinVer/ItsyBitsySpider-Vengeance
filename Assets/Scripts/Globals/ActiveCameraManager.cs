using UnityEngine;

public class ActiveCameraManager : MonoBehaviour
{
    #region Singleton instance
    private static ActiveCameraManager instance;

    public static ActiveCameraManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<ActiveCameraManager>();
            }
            if (instance == null)
            {
                Debug.LogWarning("NO CAMERA MANAGER FOUND");
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    #endregion

    public GameObject menuCamera;
    public GameObject playerCamera;

    void Start()
    {
        menuCamera.SetActive(true);
        playerCamera.SetActive(false);
    }
    public void SwitchCameras()
    {
        menuCamera.SetActive(!menuCamera.activeSelf);
        playerCamera.SetActive(!playerCamera.activeSelf);
    }
}
