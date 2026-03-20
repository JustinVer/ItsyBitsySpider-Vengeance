using UnityEngine;

public class ActiveCameraManager : MonoBehaviour
{
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
