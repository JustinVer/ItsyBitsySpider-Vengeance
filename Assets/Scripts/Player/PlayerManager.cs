using UnityEngine;

//idk where else to put this https://www.youtube.com/watch?v=qdskE8PJy6Q&list=PLBQsNXNJ-zCJ5QE8Z8aXu7jDKRjXJGrOi&t=130s
public class PlayerManager : MonoBehaviour
{
    private PlayerBody body;

    private void Start()
    {
        body = GetComponentInChildren<PlayerBody>();
    }
    void Update()
    {
        body.MovementDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        if (Input.GetAxis("Jump") > 0)
        {
            body.Jump();
        }

    }
}
