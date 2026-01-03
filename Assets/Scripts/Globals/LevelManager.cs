using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region Singleton instance
    private static LevelManager instance;



    public static LevelManager Instance
    {
        get
        {
            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }
    #endregion

    public int numEnemies { get { return enemies.Count; } private set { } }
    private HashSet<GameObject> enemies = new HashSet<GameObject>();
    [SerializeField] private bool mustKillEnemies = true;
    private bool completed = false;
    [SerializeField] private AudioClip backgroundClip;

    private void Awake()
    {
        instance = this;
        AudioManager.Instance.transitionBackgroundMusic(backgroundClip, 1.2f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == GameManager.Instance.Player.gameObject && (enemies.Count == 0 || !mustKillEnemies) && !completed)
        {
            GameManager.Instance.LevelComplete();
            completed = true;
        }
    }

    public void RegisterEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }

    public void DeRegesterEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
    }
}
