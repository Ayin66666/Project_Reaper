using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_GroundCheck : MonoBehaviour
{
    public bool isGround;
    [SerializeField] private Transform trans;
    private Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rigid.velocity = Vector3.zero;
        transform.position = trans.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ground"))
        {
            isGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGround = false;
        }
    }
}
