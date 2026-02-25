using UnityEngine;

public class TestDamageScript : MonoBehaviour, IDamageable
{
    private float currentHP = 30;
    public float getHP()
    {
        return currentHP;
    }

    public void hitEffect(Vector3 position, Vector3 forwardDirection)
    {
        throw new System.NotImplementedException();
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
