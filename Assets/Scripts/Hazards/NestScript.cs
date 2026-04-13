using System.Collections;
using UnityEngine;

public class NestScript : MonoBehaviour
{
    [SerializeField] GameObject door1;
    [SerializeField] GameObject door2;
    [SerializeField] GameObject nestTrigger;

    [SerializeField] int timeIncreaseAmount = 30;
    [SerializeField] float waitTime = 5.0f;
    [SerializeField] private ParticleSystem healParticle1;

    private static bool nest1 = true;

    private bool hasTriggered = false;

    private void OnEnable()
    {
        GameplayManager.Instance.resetEvent += resetNest;
    }

    private void OnDisable()
    {
        if (GameplayManager.Instance != null)
        {
            GameplayManager.Instance.resetEvent -= resetNest;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ReferenceEquals(GameplayManager.Instance.Player, other.gameObject) && !hasTriggered)
        {
            hasTriggered = true;
            if (nest1)
            {
                AudioManager.Instance.playTrack(2, 0);
                nest1 = false;
            }
            else
            {
                AudioManager.Instance.playTrack(3, 0);
            }
            StartCoroutine(NestStuff());
        }
    }

    IEnumerator NestStuff()
    {
        yield return StartCoroutine(MoveDoor(door1, new Vector3(0, 90, 90), 0.5f));
        yield return StartCoroutine(HealingWait(waitTime));
        yield return StartCoroutine(MoveDoor(door2, new Vector3(0, 90, 0), 0.5f));
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
    }

    IEnumerator HealingWait(float duration)
    {

        float elapsedTime = 0f;

        int playerStartHP = (int)GameplayManager.Instance.PlayerBody.getHP();
        int playerHealAmount = (int)GameplayManager.Instance.PlayerBody.getMaxHP() - playerStartHP;
        healParticle1.Play();

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            GameplayManager.Instance.IncreaseTimer(Time.deltaTime * timeIncreaseAmount / duration);
            GameplayManager.Instance.PlayerBody.setHP((int)(playerStartHP + (playerHealAmount * t)));

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        GameplayManager.Instance.PlayerBody.setHP((int)GameplayManager.Instance.PlayerBody.getMaxHP());
    }

    private void resetNest()
    {
        StopAllCoroutines();
        hasTriggered = false;
    }
}
