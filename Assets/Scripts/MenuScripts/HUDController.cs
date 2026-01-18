using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] Slider playerHealth, enemiesRemaining;

    private int maxNumEnemies = 1;

    private void Start()
    {
        GameplayManager.Instance.onLevelReset += SceneReset;
    }
    private void OnEnable()
    {
        playerHealth.value = 1;
        enemiesRemaining.value = 1;
        Update();
    }

    private void Update()
    {
        if (GameplayManager.Instance.Player && GameplayManager.Instance.Player.gameObject.activeInHierarchy)
        {
            playerHealth.value = Mathf.Clamp01(GameplayManager.Instance.PlayerBody.getHP() / GameplayManager.Instance.PlayerBody.getMaxHP());
        }

        if (LevelManager.Instance)
        {
            if (maxNumEnemies < LevelManager.Instance.numEnemies)
            {
                maxNumEnemies = LevelManager.Instance.numEnemies;
            }
            Debug.Log("MAx numbers of enemies: " + LevelManager.Instance.numEnemies + " " + maxNumEnemies);
            enemiesRemaining.value = (float)LevelManager.Instance.numEnemies / (float)maxNumEnemies;
        }
    }

    public void SceneReset()
    {
        maxNumEnemies = 1;
    }

    private void OnDestroy()
    {
        if (GameplayManager.Instance != null)
            GameplayManager.Instance.onLevelReset -= SceneReset;
    }
}
