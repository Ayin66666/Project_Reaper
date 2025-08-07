using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Stage5_AirOneSlashPos : MonoBehaviour
{
    [SerializeField] private LayerMask layer;
    [SerializeField] private float rayRange;
    public Transform movePosL;
    public Transform movePosR;
    public bool isLeft;
    public bool isRight;
    private GameObject target;

    private void FixedUpdate()
    {
        if(target != null)
        {
            transform.position = target.transform.position;
            isLeft = Physics2D.Raycast(transform.position, -transform.right, rayRange, layer);
            isRight = Physics2D.Raycast(transform.position, transform.right, rayRange, layer);

            Debug.DrawRay(transform.position, -transform.right, Color.red, rayRange);
            Debug.DrawRay(transform.position, transform.right, Color.red, rayRange);

        }
    }

    public void CheckCall(GameObject followTarget)
    {
        target = followTarget;
    }
}
