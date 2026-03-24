using System.Collections;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [SerializeField] ParticleSystem explosion;
    [SerializeField] GameObject wall;
    [SerializeField] private AudioClip breakWall;
    [SerializeField, Range(0, 1)] private float breakWallVolume = 0.5f;

    public float delay = 5f;

    public void Break()
    {
        Debug.Log("wall break");
        explosion.Play();
        AudioManager.Instance.PlaySound(breakWall, breakWallVolume, transform.position);
        wall.SetActive(false);
    }
}
