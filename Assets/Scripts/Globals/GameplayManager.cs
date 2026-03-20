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
    [SerializeField] private string playerInputName = "Player";
    [SerializeField] private string UIInputName = "UI";

    [SerializeField] private const float GRAVITY_STRENGTH = 9.8f;
    [SerializeField] private SplineContainer gravitySplineContainer;
    [SerializeField] private Spline gravitySpline;
    public Spline GravitySpline => gravitySpline;
    public GameObject Player => player;
    public PlayerBody PlayerBody => playerBody;
    public PlayerManager PlayerManager => playerManager;

    [SerializeField] private GameObject player;
    [SerializeField] private PlayerBody playerBody;
    [SerializeField] private PlayerManager playerManager;

    public event Action onLevelReset;

    public LayerMask NotPlayerOrEnemyMask;

    [SerializeField] private float countdownTime = 180f;
    private bool countDownActive = true;
    public double score = 0;

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
    private string mainMenuLevelName = "StartGameScene";

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
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
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

    public void MainMenu()
    {
        StartCoroutine(LoadLevel(mainMenuLevelName));
    }

    public void LevelComplete()
    {

    }
    #endregion
    private void Awake()
    {
        gravitySpline = gravitySplineContainer.Spline;
    }
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
    public void UpdateGravitySpline(SplineContainer segmentContainer)
    {
        Spline segmentSpline = segmentContainer.Splines[0];

        for (int i = 0; i < segmentSpline.Count; i++)
        {
            BezierKnot knot = segmentSpline[i];

            // POSITION
            Vector3 worldPos = segmentContainer.transform.TransformPoint(knot.Position);
            knot.Position = gravitySplineContainer.transform.InverseTransformPoint(worldPos);

            // TANGENTS
            Vector3 worldTanIn = segmentContainer.transform.TransformDirection(knot.TangentIn);
            Vector3 worldTanOut = segmentContainer.transform.TransformDirection(knot.TangentOut);

            knot.TangentIn = gravitySplineContainer.transform.InverseTransformDirection(worldTanIn);
            knot.TangentOut = gravitySplineContainer.transform.InverseTransformDirection(worldTanOut);

            // ROTATION
            //Quaternion worldRot = segmentContainer.transform.rotation * knot.Rotation;
            //knot.Rotation = Quaternion.Inverse(gravitySplineContainer.transform.rotation) * worldRot;

            gravitySpline.Add(knot);
        }
    }
    public void ClearGravitySpline()
    {
        if (gravitySpline != null)
            gravitySpline.Clear();
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

        Vector3 waterPoint = water.transform.position - water.transform.forward * -30;

        water.transform.forward = GetForward(water.transform.position);

        if (Vector3.Distance(water.transform.position, playerBody.transform.position) <= washOutRange)
        {
            playerManager.InputEnabled = false;

            Vector3 targetPoint = playerBody.transform.position + (waterPoint - playerBody.transform.position) * 10f * Time.deltaTime;

            if (Vector3.Distance(targetPoint, playerBody.transform.position) > Vector3.Distance(waterPoint, playerBody.transform.position))
            {
                targetPoint = waterPoint;
            }

            playerBody.transform.position = targetPoint;
        }
    }
    private void SetupForStart()
    {
        player.transform.position = new Vector3(-3, -23, 3);
        player.transform.rotation = new Quaternion();

    }

    private void ResetGame()
    {

    }

    public void PauseTimer()
    {
        countDownActive = !countDownActive;
    }

    private void Update()
    {
        if (!washedOut && countDownActive) countdownTime -= Time.deltaTime;

        HUDController.TimeInSeconds = countdownTime;

        if (countdownTime < 1)
        {
            washOut();
        }
    }

    public void setInputActionToPlayer()
    {
        playerInput.SwitchCurrentActionMap(playerInputName);
    }

    public void setInputActionToUI()
    {
        playerInput.SwitchCurrentActionMap(UIInputName);
    }

    #region inputGathering

    private Vector2 moveVector = Vector2.zero;
    private Vector2 mousePosition = Vector2.zero;

    private bool fire = false;
    private bool grapple = false;
    private bool dash = false;
    private bool plug = false;
    private bool glide = false;
    private bool jump = false;
    private bool escape = false;

    public Vector2 MoveVector { get { return moveVector; } private set { moveVector = value; } }
    public Vector2 MousePosition { get { return mousePosition; } private set { mousePosition = value; } }

    public bool Fire { get { return fire; } private set { fire = value; } }
    public bool Grapple { get { return grapple; } private set { grapple = value; } }
    public bool Dash { get { return dash; } private set { dash = value; } }
    public bool Plug { get { return plug; } private set { plug = value; } }
    public bool Glide { get { return glide; } private set { glide = value; } }
    public bool Jump { get { return jump; } private set { jump = value; } }
    public bool Escape { get { return escape; } private set { escape = value; } }

    public void OnMove(InputValue value)
    {
        moveVector = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        mousePosition = value.Get<Vector2>();
    }
    public void OnFire(InputValue value)
    {
        fire = value.isPressed;
    }

    public void OnGrapple(InputValue value)
    {
        grapple = value.isPressed;
    }

    public void OnDash(InputValue value)
    {
        dash = value.isPressed;
    }
    public void OnPlug(InputValue value)
    {
        plug = value.isPressed;
    }

    public void OnJump(InputValue value)
    {
        jump = value.isPressed;
    }

    public void OnEscape(InputValue value)
    {
        escape = value.isPressed;
    }

    public void OnGlide(InputValue value)
    {
        glide = value.isPressed;
    }


    #endregion
}
