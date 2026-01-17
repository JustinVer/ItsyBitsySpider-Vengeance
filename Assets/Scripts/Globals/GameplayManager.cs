using System;
using System.Collections;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

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

    [SerializeField] private const float GRAVITY_STRENGTH = 9.8f;
    [SerializeField] private Spline gravitySpline;
    public GameObject Player => player;
    public PlayerInput PlayerInput => playerInput;

    [SerializeField] private GameObject player;

    public event Action onLevelReset;

    public LayerMask NotPlayerOrEnemyMask;

    #region sceneManagement
    [SerializeField] private LoadScreen loadScreen;
    [SerializeField] private GameOver gameOver;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private GameObject HUD;

    private bool isLoading = false;
    private string currentLevelName;

    private SaveData saveData = new SaveData();

    private void Start()
    {
        saveData = FileHandler.LoadGame();
        HUD.SetActive(true);
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

    public Vector3 GetGravity(Vector3 position)
    {
        if (gravitySpline.Knots.Count() == 0)
        {
            return Vector3.down * GRAVITY_STRENGTH;
        }


        float3 nearestPoint;
        float t = 1;


        SplineUtility.GetNearestPoint<Spline>(gravitySpline, (float3)position, out nearestPoint, out t);
        Debug.DrawLine(position, nearestPoint, Color.red);
        return (position - ((Vector3)nearestPoint)).normalized * GRAVITY_STRENGTH;

    }
    public Vector3 GetForward(Vector3 position) //TODO fix bugs with this
    {
        float3 nearestPoint;
        float t = 1;
        SplineUtility.GetNearestPoint<Spline>(gravitySpline, (float3)position, out nearestPoint, out t);
        Vector3 tan = gravitySpline.EvaluateTangent(t);
        Debug.DrawLine(position, position + tan, Color.green);
        return tan;
    }
}
