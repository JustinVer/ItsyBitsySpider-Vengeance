using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float maxHP = 30f;
    public float moveSpeed = 10;
    public float damage = 20f;
    public float attackRange = 0.4f;
    public float attackCoolDown = 0.5f;
    public float abilityCoolDown = 1f;
    public float detectionDistanceClose = 20f;
    public float detectionDistanceLineOfSight = 40f;
    public float variantIncrease;
}
