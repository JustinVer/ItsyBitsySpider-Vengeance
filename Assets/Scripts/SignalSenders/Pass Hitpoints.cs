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

    public void hitEffect(Vector3 position, Vector3 forwardDirection)
    {
        parent.hitEffect(position, forwardDirection);
    }

    public void modifyHP(int hpChange)
    {
        Debug.Log("pass hit " + parentObject.name + " " + parent);
        parent.modifyHP(hpChange);
    }

    public void setHP(int hp)
    {
        parent.setHP(hp);
    }
}
