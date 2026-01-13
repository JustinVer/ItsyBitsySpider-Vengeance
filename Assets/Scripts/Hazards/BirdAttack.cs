using System.Collections;
using UnityEngine;

public class BirdAttack : MonoBehaviour
{
    [SerializeField] GameObject beak;
    Vector3 targetPosition;
    [SerializeField] float attackDuration = 0.25f;
    //number indicates what the bird should be doing
    //0 = Idle, 1 = poking forward, 2 = retreating
    private int attackStage = 0;
    private Vector3 homePos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        homePos = beak.transform.position;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (ReferenceEquals(GameplayManager.Instance.Player, collision.gameObject))
        {
            if (attackStage == 0)
            {
                targetPosition = collision.transform.position;
                attackStage = 1;
                beak.transform.LookAt(collision.transform.position);
                beak.transform.Rotate(90, 0, 0);
                StartCoroutine(moveBeak(collision.transform.position, attackDuration));
            }
        }
    }

    IEnumerator moveBeak(Vector3 targetPosition, float duration)
    {
        float timeMoving = 0;
        Vector3 startPosition = beak.transform.position;

        while (timeMoving < duration)
        {
            float t = timeMoving / duration;
            t = Mathf.SmoothStep(0f, 1f, t);


            beak.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            timeMoving += Time.deltaTime;

            yield return null;
        }

        // Ensure the object reaches the exact final position
        transform.position = targetPosition;
        returnToStart();
    }

    private void returnToStart()
    {
        if (attackStage == 1)
        {
            attackStage = 2;
            StartCoroutine(moveBeak(homePos, attackDuration * 3));
        }
        else
        {
            attackStage = 0;
        }
    }
}
