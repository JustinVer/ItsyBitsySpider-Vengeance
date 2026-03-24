using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable, IReturnSelfObject<EnemyBase>, IDeathAnimation
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected EnemyData data;
    protected float currentHP;
    protected bool isDying = false;
    protected ObjectPool<EnemyBase> parentPool;
    [SerializeField] private ParticleSystem damageParticle;

    [SerializeField] protected AudioClip death;
    [SerializeField, Range(0, 1)] protected float deathVolume = 0.5f;

    protected virtual void Awake()
    {
        setHP(data.maxHP);
        if (damageParticle)
        {
            damageParticle.transform.parent = null;
            damageParticle.gameObject.SetActive(true);
        }
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

    public void modifyHP(int hpChange)
    {
        currentHP = Mathf.Clamp(currentHP + hpChange, 0f, data.maxHP);
        if (currentHP <= 1.0f)
        {
            Die();
        }
    }

    public void setHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0f, data.maxHP);
    }

    public virtual void SetEnemyData(EnemyData data)
    {
        this.data = data;
        if (currentHP > data.maxHP)
        {
            currentHP = data.maxHP;
        }
    }

    protected abstract void Move();
    protected abstract void Attack();
    protected abstract void Die();
    public abstract void EndDeath();
    public abstract void ReturnSelf();

    public virtual void SetParentPool(ObjectPool<EnemyBase> parentPool)
    {
        this.parentPool = parentPool;
    }

    public void hitEffect(Vector3 position, Vector3 forwardDirection)
    {
        damageParticle.transform.position = position;
        damageParticle.transform.forward = forwardDirection;
        damageParticle.Play();
    }
}
