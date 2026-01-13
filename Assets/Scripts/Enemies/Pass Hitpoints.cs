using UnityEngine;

public class PassHitpoints : MonoBehaviour, IDamageable
{
    [SerializeField]
    private MonoBehaviour parentObject; // works

    public IDamageable parent => parentObject as IDamageable;

    public float getHP()
    {
        return parent.getHP();
    }

    public void modifyHP(float hpChange)
    {
        parent.modifyHP(hpChange);
    }

    public void setHP(float hp)
    {
        parent.setHP(hp);
    }
}
