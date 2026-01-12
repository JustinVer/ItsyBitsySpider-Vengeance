using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable, IReturnSelfObject<EnemyBase>
{
    [SerializeField] private float MaxHP = 30f;
    [SerializeField] private Animator animator;
    private float currentHP;
    private ObjectPool<EnemyBase> parentPool;

    protected virtual void Start()
    {
        setHP(MaxHP);
    }
    public float getHP()
    {
        return currentHP;
    }

    public void modifyHP(float hpChange)
    {
        currentHP = Mathf.Clamp(currentHP + hpChange, 0f, MaxHP);
        if (currentHP == 0)
        {
            Die();
        }
    }

    public void setHP(float hp)
    {
        currentHP = Mathf.Clamp(hp, 0f, MaxHP);
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
