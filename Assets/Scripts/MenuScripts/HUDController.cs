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
        playerHealth.value = 0;
        enemiesRemaining.value = 0;
    }

    private void Update()
    {
        if (GameplayManager.Instance.Player && GameplayManager.Instance.Player.gameObject.activeInHierarchy)
        {
            //playerHealth.value = Mathf.Clamp01(GameManager.Instance.Player.CurrentHealth / GameManager.Instance.Player.Maxhealth);
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
