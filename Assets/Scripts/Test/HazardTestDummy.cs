using UnityEngine;

public class HazardTestDummy : MonoBehaviour
{
    // Public variable for speed, adjustable in the Inspector
    [SerializeField] private float speed = 10.0f;

    // The direction the object will move (can be changed in the Inspector)
    // Default is forward (positive Z-axis)
    [SerializeField] private Vector3 direction = new Vector3(0, 0, 1);

    // Update is called once per frame
    void Update()
    {
        // Calculate the movement amount for this frame
        Vector3 movement = direction * speed * Time.deltaTime;

        // Apply the movement to the object's position
        transform.Translate(movement);
    }
}
