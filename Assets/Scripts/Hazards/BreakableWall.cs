using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Break Wall");
        if (collision.gameObject.CompareTag("cocoon"))
        {
            this.Break();
        }
    }

    private void Break()
    {
        gameObject.SetActive(false);
    }
}
