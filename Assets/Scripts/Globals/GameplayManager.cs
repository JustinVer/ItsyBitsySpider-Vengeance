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
    public Spline GravitySpline => gravitySpline;
    public GameObject Player => player;
    public PlayerBody PlayerBody => playerBody;
    public PlayerInput PlayerInput => playerInput;

    [SerializeField] private GameObject player;
    [SerializeField] private PlayerBody playerBody;
    [SerializeField] private PlayerManager playerManager;

    public event Action onLevelReset;

    public LayerMask NotPlayerOrEnemyMask;

    [SerializeField] private float countdownTime = 180f;

    [SerializeField] GameObject water;
    private float waterPosition = 1;
    [SerializeField] private float waterSpeed;
    private float waterPercentSpeed;
    private bool washedOut = false;

    [SerializeField] private float washOutRange;
    [SerializeField] private float washOutStrength;

    #region sceneManagement
    [SerializeField] private LoadScreen loadScreen;
    [SerializeField] private GameOver gameOver;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private GameObject HUD;
    private HUDController HUDController;

    private bool isLoading = false;
    private string currentLevelName;

    private SaveData saveData = new SaveData();

    private void Start()
    {
        saveData = FileHandler.LoadGame();
        HUD.SetActive(true);
        HUDController = HUD.GetComponent<HUDController>();

        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        // Hide the cursor
        Cursor.visible = false;
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

        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        // Hide the cursor
        Cursor.visible = false;
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
    public void UpdateGravitySpline(Spline splineToAdd)
    {
        gravitySpline.Add(splineToAdd);
    }
    public Vector3 GetForward(Vector3 position) //TODO fix bugs with this
    {
        float3 nearestPoint;
        float t = 1;
        SplineUtility.GetNearestPoint<Spline>(gravitySpline, (float3)position, out nearestPoint, out t);
        Vector3 tan = ((Vector3)gravitySpline.EvaluateTangent(t)).normalized;
        Debug.DrawLine(position, position + tan, Color.green);
        return tan;
    }

    private void washOut()
    {
        if (!washedOut)
        {
            washedOut = true;
            water.SetActive(true);
            float length = gravitySpline.GetLength();
            waterPercentSpeed = waterSpeed / length;
        }
        water.transform.position = SplineUtility.EvaluatePosition<Spline>(gravitySpline, waterPosition);
        waterPosition -= waterPercentSpeed * Time.deltaTime;

        Vector3 waterPoint = water.transform.position - water.transform.right * 10;

        if (Vector3.Distance(waterPoint, playerBody.transform.position) <= washOutRange)
        {
            playerManager.InputEnabled = false;

            Vector3 targetPoint = playerBody.transform.position + (waterPoint - playerBody.transform.position) * 10f * Time.deltaTime ;

            if (Vector3.Distance(targetPoint, playerBody.transform.position) > Vector3.Distance(waterPoint, playerBody.transform.position))
            {
                targetPoint = waterPoint;
            }

            playerBody.transform.position = targetPoint;
        }
    }

    private void Update()
    {
        if (!washedOut) countdownTime -= Time.deltaTime;
    
        HUDController.TimeInSeconds = countdownTime;

       if (countdownTime < 1)
        {
            washOut();
        }
    }
}
