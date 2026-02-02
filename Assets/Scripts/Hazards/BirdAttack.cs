using System.Collections;
using UnityEngine;

public class BirdAttack : MonoBehaviour
{
    [SerializeField] GameObject beak;
    [SerializeField] GameObject upperBeak;
    [SerializeField] GameObject lowerBeak;
    Vector3 targetPosition;
    PlayerBody body;
    [SerializeField] float attackDuration = 0.25f;
    [SerializeField] float attackForce = 30f;
    [SerializeField] int attackDamage = -30;
    //number indicates what the bird should be doing
    //0 = Idle, 1 = poking forward, 2 = retreating
    private int attackStage = 0;
    private Vector3 homePos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        homePos = beak.transform.position;
        body = GameplayManager.Instance.Player.GetComponentInChildren<PlayerBody>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        Vector3 upperBeakOpen = new Vector3(-20f, 0f, 0f);
        Vector3 lowerBeakOpen = new Vector3(20f, 0f, 0f);
        if (ReferenceEquals(GameplayManager.Instance.Player, collision.gameObject))
        {
            if (attackStage == 0)
            {
                targetPosition = collision.transform.position;
                attackStage = 1;
                beak.transform.LookAt(collision.transform.position);
                beak.transform.Rotate(90f, 0f, 0f);
                upperBeak.transform.Rotate(upperBeakOpen);
                lowerBeak.transform.Rotate(lowerBeakOpen);
                StartCoroutine(moveBeak(collision.transform.position, attackDuration, -upperBeakOpen, -lowerBeakOpen));
            }
        }
    }

    IEnumerator moveBeak(Vector3 targetPosition, float duration, Vector3 upperBeakAngle, Vector3 lowerBeakAngle)
    {
        float timeMoving = 0;
        Vector3 startPosition = beak.transform.position;
        Quaternion upperStartRotation = upperBeak.transform.localRotation;
        Quaternion upperEndRotation = upperBeak.transform.localRotation * Quaternion.Euler(upperBeakAngle);
        Quaternion lowerStartRotation = lowerBeak.transform.localRotation;
        Quaternion lowerEndRotation = lowerBeak.transform.localRotation * Quaternion.Euler(lowerBeakAngle);
        Debug.Log("corutine");

        while (timeMoving < duration)
        {
            float t = timeMoving / duration;
            t = Mathf.SmoothStep(0f, 1f, t);


            beak.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            upperBeak.transform.localRotation = Quaternion.Slerp(upperStartRotation, upperEndRotation, timeMoving / duration);
            lowerBeak.transform.localRotation = Quaternion.Slerp(lowerStartRotation, lowerEndRotation, timeMoving / duration);

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

            Vector3 attackDirection = targetPosition - homePos;
            attackDirection = attackDirection.normalized;
            attackDirection = attackDirection * attackForce;
            body.ApplyForce(attackDirection, ForceMode.Impulse);

            body.modifyHP(attackDamage);

            StartCoroutine(moveBeak(homePos, attackDuration * 3, Vector3.zero, Vector3.zero));
        }
        else
        {
            attackStage = 0;
            beak.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
