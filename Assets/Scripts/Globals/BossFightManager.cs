using UnityEngine;

public class BossFightManager : MonoBehaviour
{
    #region Singleton instance
    private static BossFightManager instance;

    public static BossFightManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<BossFightManager>();
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

    private float timer = 0.0f;
    [SerializeField] private Transform[] enemySpawnPositions;
    [SerializeField] private GameObject[] enemyPrefabs;


    public void SummonRandomEnemies(int numEnemies)
    {
        float spawnChance = ((float)numEnemies) / ((float)enemySpawnPositions.Length);
        Debug.Log("Boss summon spawn chance " + spawnChance);
        for (int i = 0; i < enemySpawnPositions.Length && numEnemies > 0; i++)
        {
            if (enemySpawnPositions.Length - i >= numEnemies || Random.value < spawnChance)
            {
                int enemyToSpawn = Random.Range(0, enemyPrefabs.Length);

                Instantiate(enemyPrefabs[enemyToSpawn], enemySpawnPositions[i].position, Quaternion.identity);
                numEnemies--;
            }
        }
    }
}
