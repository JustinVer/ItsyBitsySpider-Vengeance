using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    string folderPath = "Assets/Screenshots/";
    bool shot = false;

    // Update is called once per frame
    void Update()
    {
        if (!shot)
        {
            string screenshotName = "Screenshot_" + System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".png";
            ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName), 2); // takes the sceenshot, the "2" is for the scaled resolution, you can put this to 600 but it will take really long to scale the image up
            Debug.Log(folderPath + screenshotName);
            shot = true;
        }
        }
    }
