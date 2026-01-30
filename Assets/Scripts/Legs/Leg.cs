using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Leg : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private GameObject foot;
    [SerializeField] private Transform outPoint;
    [SerializeField] private float legLength;

    public Vector3 TargetPos = Vector3.zero;
    public Vector3 FootPos = Vector3.zero;

    private void Update()
    {
        Vector3 down = GameplayManager.Instance.GetGravity(outPoint.position).normalized;

        Debug.DrawRay(outPoint.position, down * legLength, Color.red);

        RaycastHit hit;


        if (Physics.Raycast(outPoint.position, down, out hit, legLength))
        {
            TargetPos = hit.point;
        }
        else
        {
            TargetPos = outPoint.position + down * legLength;
        }


      
       

            FootPos = foot.transform.position;
    }
        
    

    public void Step()
    {
        foot.transform.position = TargetPos;
    }
}
