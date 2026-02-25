using UnityEngine;

public interface IDamageable
{
    public void modifyHP(int hpChange);
    public float getHP();
    public void setHP(int hp);

    public void hitEffect(Vector3 position);
}
