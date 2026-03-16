using System.Collections;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [SerializeField] ParticleSystem explosion;
    [SerializeField] GameObject wall;

    public float delay = 5f;

    public void Break()
    {
        Debug.Log("wall break");
        explosion.Play();
        wall.SetActive(false);
    }
}
