using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerBody body;

    private void Start()
    {
        body = GetComponentInChildren<PlayerBody>();
    }
    void Update()
    {
        body.MovementDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}
