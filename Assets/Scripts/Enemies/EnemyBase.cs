using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable, IReturnSelfObject<EnemyBase>
{
    [SerializeField] private Animator animator;
    [SerializeField] protected EnemyData data;
    private float currentHP;
    private bool isDying = false;
    private ObjectPool<EnemyBase> parentPool;

    protected virtual void Awake()
    {
        setHP(data.maxHP);
    }

    protected virtual void Update()
    {
        if (!isDying)
        {
            NotDyingUpdate();
        }
    }

    protected virtual void NotDyingUpdate()
    {
        Move();
        Attack();
    }

    public float getHP()
    {
        return currentHP;
    }

    public void modifyHP(float hpChange)
    {
        currentHP = Mathf.Clamp(currentHP + hpChange, 0f, data.maxHP);
        if (currentHP == 0)
        {
            Die();
        }
    }

    public void setHP(float hp)
    {
        currentHP = Mathf.Clamp(hp, 0f, data.maxHP);
    }

    public void SetEnemyData(EnemyData data)
    {
        this.data = data;
    }

    protected abstract void Move();
    protected abstract void Attack();
    protected abstract void Die();
    public abstract void ReturnSelf();

    public virtual void SetParentPool(ObjectPool<EnemyBase> parentPool)
    {
        this.parentPool = parentPool;
    }
}
