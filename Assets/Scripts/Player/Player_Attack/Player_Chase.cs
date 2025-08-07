using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Chase : MonoBehaviour
{
    [SerializeField] Player_Animation playerAnim;
    [SerializeField] Player_Move playerMove;
    [SerializeField] Player_Skill playerSkill;

    public GameObject enemy;
    [SerializeField] private RaycastHit2D chaseWallcheck_Left;
    [SerializeField] private RaycastHit2D chaseWallcheck_Right;

    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Chase();
    }

    void Chase()
    {
        if (Player_Status.instance.canChase && Input.GetKeyDown(KeyCode.V))
        {
            playerAnim.anim.SetBool("isChase", true);
            playerAnim.anim.SetTrigger("ChaseAttack");

            Player_Status.instance.canChase = false;
            Player_Status.instance.GravityCheck(true);
            Player_Status.instance.rigidBody2D.velocity = Vector2.zero;

            if(playerSkill.red_Skill1_HitCount == 3)
            {
                playerSkill.red_Skill1_HitCount = 0;
            }

            chaseWallcheck_Left = Physics2D.Raycast(enemy.transform.position, Vector2.left, 3f, Player_Status.instance.groundCheck);
            chaseWallcheck_Right = Physics2D.Raycast(enemy.transform.position, Vector2.right, 3f, Player_Status.instance.groundCheck);

            if(chaseWallcheck_Left)
            {
                playerMove.transform.position = enemy.transform.position + new Vector3(2f, 0, 0);
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                playerMove.transform.position = enemy.transform.position + new Vector3(-2f, 0, 0);
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
        }
    }
}
