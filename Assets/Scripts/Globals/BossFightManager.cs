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
    private int numEnemies = 0;

}
