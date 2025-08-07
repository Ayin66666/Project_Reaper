using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Animation : MonoBehaviour
{
    public Animator anim;
    [SerializeField] Player_Attack playerAttack;
    [SerializeField] Player_Skill playerSkill;
    [SerializeField] Player_Move playerMove;
    void Awake()
    {
        anim = GetComponent<Animator>();

        playerAttack = GetComponentInParent<Player_Attack>();
        playerSkill = GetComponentInParent<Player_Skill>();
        playerMove = GetComponentInParent<Player_Move>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("isDash", Player_Status.instance.isDash);
    }

    public void FinishedAttack()
    {
        Debug.Log("공격 종료");
        Player_Status.instance.isAirAttack = false;
        Player_Status.instance.isGroundAttack = false;
        Player_Status.instance.isSkill = false;
        Player_Status.instance.canMove = true;
        Player_Status.instance.canJump = true;
        Player_Status.instance.canAttack = true;

        if(Player_Status.instance.isGround)
        {
            playerAttack.curComboCount = 0;
        }
        anim.SetBool("isAttack", false);
        anim.SetBool("isSkill", false);
        anim.SetBool("isChase", false);
        anim.SetInteger("ComboCount", playerAttack.curComboCount);
        Player_Status.instance.canNextAttack = true;
        Player_Status.instance.GravityCheck(false);
    }


    public void CanNextAttack()
    {
        Player_Status.instance.canNextAttack = true;
    }
    #region 콜라이더
    public void OnOff_AttackCollider()
    {
        playerAttack.attackCollider[playerAttack.curComboCount - 1].SetActive(playerAttack.attackCollider[playerAttack.curComboCount - 1].activeSelf ? false : true);
    }

    public void OnOff_AirborneAttack_Collider()
    {
        playerAttack.attackCollider[3].SetActive(playerAttack.attackCollider[3].activeSelf ? false : true);
    }

    public void OnOff_DownAttack_Colllider()
    {
        playerAttack.attackCollider[4].SetActive(playerAttack.attackCollider[4].activeSelf ? false : true);
    }

    public void OnOff_ChaseAttack_Collider()
    {
        playerAttack.attackCollider[5].SetActive(playerAttack.attackCollider[5].activeSelf ? false : true);
    }
    public void OnOff_Blue_Skill1_Collider()
    {
        playerSkill.blue_Skill1_Collider.SetActive(playerSkill.blue_Skill1_Collider.activeSelf ? false : true);
    }


    public void OnOff_Blue_Skill2_1Collider()
    {
        playerSkill.blue_Skill2_Collider[0].SetActive(playerSkill.blue_Skill2_Collider[0].activeSelf ? false : true);
    }
    public void OnOff_Blue_Skill2_2Collider()
    {
        playerSkill.blue_Skill2_Collider[1].SetActive(playerSkill.blue_Skill2_Collider[1].activeSelf ? false : true);
    }

    public void OnOff_Red_Skill1_Collider()
    {
        playerSkill.red_Skill1_Collider.SetActive(playerSkill.red_Skill1_Collider.activeSelf ? false : true);
    }

    public void OnOff_Red_Skill2_1Collider()
    {
        playerSkill.red_Skill2_Collider[0].SetActive(playerSkill.red_Skill2_Collider[0].activeSelf ? false : true);
    }

    public void OnOff_Red_Skill2_2Collider()
    {
        playerSkill.red_Skill2_Collider[1].SetActive(playerSkill.red_Skill2_Collider[1].activeSelf ? false : true);
    }

    public void AllOff_Attack_Collider()
    {
        for (int i = 0; i < 4; i++)
        {
            playerAttack.attackCollider[i].SetActive(false);
        }

        for (int i = 0;i < 2;i++)
        {
            playerSkill.blue_Skill2_Collider[i].SetActive(false);
        }

        for (int i = 0; i < 2; i++)
        {
            playerSkill.red_Skill2_Collider[i].SetActive(false);
        }
    }
    #endregion

    public void OnOff_Skill_Ready()
    {
        anim.SetBool("Skill_Ready", anim.GetBool("Skill_Ready") == true ? false : true);
    }

    public void OnOff_Holding()
    {
        anim.SetBool("Holding", anim.GetBool("Holding") == true ? false : true);
    }

    public void Body_On()
    {
        Player_Status.instance.sprite_Red.color = new Color(1, 1, 1, 1);
        Player_Status.instance.sprite_Blue.color = new Color(1, 1, 1, 1);
    }

    public void Body_Off()
    {
        Debug.Log("투명");
        Player_Status.instance.sprite_Red.color = new Color(1, 1, 1, 0);
        Player_Status.instance.sprite_Blue.color = new Color(1, 1, 1, 0);
    }

    public void Red_Skill1_Sound()
    {
        Player_Sound.instance.SFXPlay(Player_Sound.instance.red_Skill_Sound[0]);
    }

    public void Red_Skill2_Loop_Sound1()
    {
        Player_Sound.instance.SFXPlay(Player_Sound.instance.red_Skill_Sound[0]);
    }

    public void Red_Skill2_Loop_Sound2()
    {
        Player_Sound.instance.SFXPlay(Player_Sound.instance.red_Skill_Sound[1]);
    }

    public void Red_Skill2_Loop_Sound3()
    {
        Player_Sound.instance.SFXPlay(Player_Sound.instance.red_Skill_Sound[2]);
    }

    public void AttackMoveMent()
    {
        playerMove.Call_AttackMovement();
    }
}
