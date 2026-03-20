using UnityEngine;

public class SendAnimationCompleteSignal : MonoBehaviour
{
    [SerializeField] private BigT bigT;

    public void SummonComplete()
    {
        Debug.Log("Boss summon script ");
        bigT.SummonComplete();
    }
}
