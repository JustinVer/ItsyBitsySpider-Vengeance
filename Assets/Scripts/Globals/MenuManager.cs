using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class MenuManager : MonoBehaviour
{
    #region Singleton instance
    private static MenuManager instance;

    public static MenuManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<MenuManager>();
            }
            if (instance == null)
            {
                Debug.LogWarning("NO MENU MANAGER FOUND");
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    #endregion

    #region sceneManagement
    [SerializeField] private string gameplayScene;
    [SerializeField] private string mainMenuName;
    [SerializeField] private LoadScreen loadScreen;
    [SerializeField] private OptionsMenu optionsMenu;

    private bool isLoading = false;
    private string currentLevelName;
    private int currentLevelIndex = 0;

    private SaveData saveData = new SaveData();

    private void Start()
    {
        optionsMenu.LoadSettings();
        saveData = FileHandler.LoadGame();
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Confined;
        // Hide the cursor
        Cursor.visible = true;
        LoadMainMenu();
        loadScreen.gameObject.SetActive(false);
    }

    private IEnumerator LoadLevel(string levelName)
    {
        Debug.Log("CURRENT SCENE: " + currentLevelName);
        Debug.Log("LOADING: " + levelName);

        if (isLoading)
        {
            yield break;
        }
        isLoading = true;

        loadScreen.gameObject.SetActive(true);
        loadScreen.StartLoading();

        //Check if a level is already loaded and unload if needed
        if (!string.IsNullOrEmpty(currentLevelName))
        {
            yield return SceneManager.UnloadSceneAsync(currentLevelName);
        }

        //Load new level
        AsyncOperation asyncLoad;
        if (levelName != gameplayScene)
        {
            asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        }
        else
        {
            asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
        }
        yield return new WaitUntil(() => !asyncLoad.isDone);
        yield return loadScreen.FinishLoading();

        currentLevelName = levelName;

        Debug.Log("LOADED: " + levelName);

        //Wait one frame before saying loaded is done just to make sure everything is done loading in the scene
        yield return null;
        yield return null;
        Debug.Log("SETS NEW ACTIVE SCENE");
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));
        isLoading = false;

        loadScreen.gameObject.SetActive(false);
        Debug.Log("FINISHES LOAD");
    }

    public void StartNewGame()
    {
        FileHandler.CleanSave();
        saveData.InitilizeData();
        saveData.currentLevel = 0;
        FileHandler.SaveGame(saveData);
        Debug.Log("NEW GAME");
        StartCoroutine(LoadLevel(gameplayScene));
    }

    public void ContinueGame()
    {
        currentLevelIndex = saveData.currentLevel;
        StartNewGame();
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadLevel(mainMenuName));
    }

    public void OptionsMenu(bool turnOn)
    {
        optionsMenu.gameObject.SetActive(turnOn);
    }
    #endregion
}
