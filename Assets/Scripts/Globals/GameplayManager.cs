using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class GameplayManager : MonoBehaviour
{
    #region Singleton instance
    private static GameplayManager instance;

    public static GameplayManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameplayManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            if (instance == null)
            {
                Debug.LogWarning("NO GAMEPLAY MANAGER FOUND");
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    #endregion

    //[SerializeField] private HumanBody player;
    [SerializeField] private PlayerInput playerInput;
    public GameObject Player => player;
    public PlayerInput PlayerInput => playerInput;

    private GameObject player;

    public event Action onLevelReset;

    #region sceneManagement
    [SerializeField] private string gameplayScene;
    [SerializeField] private LoadScreen loadScreen;
    [SerializeField] private GameOver gameOver;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private PauseMenu pauseMenu;

    private bool isLoading = false;
    private string currentLevelName;
    private int currentLevelIndex = 0;

    private SaveData saveData = new SaveData();

    private void Start()
    {
        saveData = FileHandler.LoadGame();
    }

    private IEnumerator LoadLevel(string levelName)
    {
        if (isLoading)
        {
            yield break;
        }
        isLoading = true;
        pauseMenu.CanPause = false;
        //player.transform.parent.gameObject.SetActive(false);

        loadScreen.gameObject.SetActive(true);
        loadScreen.StartLoading();

        //Check if a level is already loaded and unload if needed
        if (!string.IsNullOrEmpty(currentLevelName))
        {
            yield return SceneManager.UnloadSceneAsync(currentLevelName);
        }

        //Load new level
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => !asyncLoad.isDone);
        yield return loadScreen.FinishLoading();

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

        loadScreen.gameObject.SetActive(false);
        pauseMenu.CanPause = true;
    }

    public void ResetScene()
    {
        onLevelReset?.Invoke();
    }

    public void GameOver()
    {
        gameOver.gameObject.SetActive(true);
        playerInput.enabled = false;
    }

    public void LevelComplete()
    {

    }
    #endregion

    public Vector3 GetGravity(GameObject obj)
    {
        return Vector3.down;
    }
}
