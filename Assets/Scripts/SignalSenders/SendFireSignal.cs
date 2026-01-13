using UnityEngine;

/// <summary>
/// Listends for a trigger pull event from the animator and sends it to the parent body
/// </summary>
public class SendFireSignal : MonoBehaviour
{
    [HideInInspector]
    public IFireAnimation body;

    public void PullTrigger()
    {
        if (body != null)
        {
            body.FireProjectile();
        }
    }

    public void FireComplete()
    {
        if (body != null)
        {
            body.FireComplete();
        }
    }
}
