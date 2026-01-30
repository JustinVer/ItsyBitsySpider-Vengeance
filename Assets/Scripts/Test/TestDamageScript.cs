using UnityEngine;

public class TestDamageScript : MonoBehaviour, IDamageable
{
    private float currentHP = 30;
    public float getHP()
    {
        return currentHP;
    }

    public void modifyHP(int hpChange)
    {
        currentHP = currentHP + hpChange;
    }

    public void setHP(int hp)
    {
        currentHP = hp;
    }
}
