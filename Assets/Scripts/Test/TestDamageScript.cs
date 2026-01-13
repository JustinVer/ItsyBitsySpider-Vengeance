using UnityEngine;

public class TestDamageScript : MonoBehaviour, IDamageable
{
    private float currentHP = 30;
    public float getHP()
    {
        return currentHP;
    }

    public void modifyHP(float hpChange)
    {
        currentHP = currentHP + hpChange;
    }

    public void setHP(float hp)
    {
        currentHP = hp;
    }
}
