using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    Rigidbody2D rigid;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;
    private Vector3 moveDir;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f).normalized;
        if(moveDir.magnitude > 0.1f)
        {
            rigid.velocity = new Vector2(moveDir.x * moveSpeed, rigid.velocity.y);
        }
        else
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            rigid.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);
        }
    }
}
