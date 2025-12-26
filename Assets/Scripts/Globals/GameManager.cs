using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class GameManager : MonoBehaviour
{
    #region Singleton instance
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameManager>();
            }
            if (instance == null)
            {
                Debug.LogWarning("NO GAME MANAGER FOUND");
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    #endregion

    public event Action onLevelReset;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    //[SerializeField] private HumanBody player;
    [SerializeField] private PlayerInput playerInput;
    //public HumanBody Player => player;
    public PlayerInput PlayerInput => playerInput;

    #region sceneManagement
    //[SerializeField] private string gameplayScene;
    //[SerializeField] private string mainMenuName;
    //[SerializeField] private LoadScreen loadScreen;
    //[SerializeField] private GameOver gameOver;
    //[SerializeField] private GameObject victoryScreen;
    //[SerializeField] private PauseMenu pauseMenu;
    //[SerializeField] private OptionsMenu optionsMenu;

    private bool isLoading = false;
    private string currentLevelName;
    private int currentLevelIndex = 0;

    //private SaveData saveData = new SaveData();

    private void Start()
    {
        //optionsMenu.LoadSettings();
        //saveData = FileHandler.LoadGame();
        //LoadMainMenu();
    }

    private IEnumerator LoadLevel(string levelName)
    {
        if (isLoading)
        {
            yield break;
        }
        isLoading = true;
        //pauseMenu.CanPause = false;
        //player.transform.parent.gameObject.SetActive(false);

        //loadScreen.gameObject.SetActive(true);
        //loadScreen.StartLoading();

        //Check if a level is already loaded and unload if needed
        if (!string.IsNullOrEmpty(currentLevelName))
        {
            yield return SceneManager.UnloadSceneAsync(currentLevelName);
        }

        //Load new level
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => !asyncLoad.isDone);
        //yield return loadScreen.FinishLoading();

        currentLevelName = levelName;
        //player.transform.parent.gameObject.SetActive(true);
        //player.transform.position = Vector3.zero;
        //if (levelName == mainMenuName)
        //{
        //    playerInput.enabled = false;
        //}
        //else
        //{
        //    playerInput.enabled = true;
        //}

        //Wait one frame before saying loaded is done just to make sure everything is done loading in the scene
        yield return null;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));
        isLoading = false;

        //loadScreen.gameObject.SetActive(false);
        //pauseMenu.CanPause = true;
    }

    public void ResetScene()
    {
        onLevelReset?.Invoke();
    }

    public void StartNewGame()
    {
        //FileHandler.CleanSave();
        //saveData.InitilizeData();
        //saveData.currentLevel = 0;
        //FileHandler.SaveGame(saveData);
        //StartCoroutine(LoadLevel(gameplayScene));
    }

    public void ContinueGame()
    {
        //currentLevelIndex = saveData.currentLevel;
        //if (currentLevelIndex < levelNames.Length)
        //{
        //    onLevelReset?.Invoke();
        //    StartCoroutine(LoadLevel(levelNames[currentLevelIndex]));
        //}
        //else
        //{
        //    StartNewGame();
        //}
    }

    public void LoadMainMenu()
    {
        //onLevelReset?.Invoke();
        //StartCoroutine(LoadLevel(mainMenuName));
    }

    public void GameOver()
    {
        //gameOver.gameObject.SetActive(true);
        //playerInput.enabled = false;
    }

    public void OptionsMenu(bool turnOn)
    {
        //optionsMenu.gameObject.SetActive(turnOn);
    }


    #endregion
}