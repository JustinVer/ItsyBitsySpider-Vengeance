using UnityEngine;

public interface IDamageable
{
    public void modifyHP(float hpChange);
    public float getHP();
    public void setHP(float hp);
}
