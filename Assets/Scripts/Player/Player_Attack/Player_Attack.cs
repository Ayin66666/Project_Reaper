using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Attack : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private Player_Animation playerAnim;
    [SerializeField] private Player_Move playerMove;

    public GameObject[] attackCollider;

    public int curComboCount;
    public int maxComboCount;
    public enum States { Attack1, Attack2, Attack3, CommandAttack }
    public States currentState;

    private Coroutine myCoroutine;

    void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Player_Status.instance.action += CoroutineStop;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Z) && Player_Status.instance.canAttack && Player_Status.instance.canNextAttack && curComboCount < maxComboCount)
        {
            if(Player_Status.instance.isHit)
            {
                return;
            }
            ComboAttack();
        }

        if(Player_Status.instance.isGround && !Player_Status.instance.isGroundAttack && !Player_Status.instance.isAirAttack)
        {
            curComboCount = 0;
        }
    }

    void ComboAttack()
    {
        Debug.Log("공격");
        rigidBody2D.velocity = Vector2.zero;

        Player_Status.instance.isGroundAttack = Player_Status.instance.isGround ? true : false;
        Player_Status.instance.isAirAttack = Player_Status.instance.isGround ? false : true;
        Player_Status.instance.canMove = false;
        Player_Status.instance.canJump = false;
        Player_Status.instance.canNextAttack = false;

        Player_Status.instance.GravityCheck(true);

        curComboCount++;

        if (curComboCount == 3)
        {
            CommandAttack();
            return;
        }

        if (curComboCount > 3)
        {
            curComboCount = 1;
        }
            
        playerAnim.anim.SetBool("isAttack", true);
        playerAnim.anim.SetTrigger("ComboAttack");
        playerAnim.anim.SetInteger("ComboCount", curComboCount);
        if(curComboCount == 1)
        {
            Player_Sound.instance.SFXPlay(Player_Sound.instance.groundAttack_Sound[0]);
        }
        else if(curComboCount == 2)
        {
            Player_Sound.instance.SFXPlay(Player_Sound.instance.groundAttack_Sound[1]);
        }

        currentState = (States)(curComboCount - 1);
    }

    void CommandAttack()
    {
        playerAnim.anim.SetBool("isAttack", true);

        if (Input.GetKey(KeyCode.UpArrow) && Player_Status.instance.isGround)
        {
            playerAnim.anim.SetTrigger("UpSmash");
            Player_Sound.instance.SFXPlay(Player_Sound.instance.groundAttack_Sound[3]);
            Debug.Log("위로 날리기");
        }
        else if (Input.GetKey(KeyCode.DownArrow) && !Player_Status.instance.isGround)
        {
            playerAnim.anim.SetTrigger("DownSmash");
            Player_Sound.instance.SFXPlay(Player_Sound.instance.groundAttack_Sound[3]);
            Debug.Log("아래로 날리기");
        }
        else
        {
            playerAnim.anim.SetTrigger("ComboAttack");
            Player_Sound.instance.SFXPlay(Player_Sound.instance.groundAttack_Sound[2]);
        }

        playerAnim.anim.SetInteger("ComboCount", curComboCount);

        currentState = (States)(curComboCount);
    }

    void CoroutineStop()
    {
        if(myCoroutine != null)
        {
            StopCoroutine(myCoroutine);
        }
    }
}
