using System.Collections;
using UnityEngine;

public class NestScript : MonoBehaviour
{
    [SerializeField] GameObject door1;
    [SerializeField] GameObject door2;
    [SerializeField] GameObject nestTrigger;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (ReferenceEquals(GameplayManager.Instance.Player, other.gameObject) && !hasTriggered)
        {
            GameplayManager.Instance.PauseTimer();
            StartCoroutine(MoveDoor(door1, new Vector3(0, 90, 90), 0.5f));
        }
    }

    IEnumerator MoveDoor(GameObject door, Vector3 newRot, float moveTime)
    {
        Quaternion startRotation = door.transform.localRotation;

        Quaternion targetRotation = Quaternion.Euler(newRot);
        float elapsedTime = 0f;

        while (elapsedTime < moveTime)
        {
            // Calculate the interpolation factor (0 to 1)
            float t = elapsedTime / moveTime;

            door.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        door.transform.localRotation = targetRotation;

        if (ReferenceEquals(door, door1))
        {
            StartCoroutine(HealingWait(10));
        }
        else if (ReferenceEquals(door, door2))
        {
            // GameplayManager.Instance.PauseTimer();
        }
    }

    IEnumerator HealingWait(float duration)
    {
        yield return new WaitForSeconds(duration);

        StartCoroutine(MoveDoor(door2, new Vector3(0, 90, 0), 0.5f));
    }
}
