using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Strengthened_Range_AnimtionEvent : MonoBehaviour
{
    [SerializeField] private Enemy_Strengthened_Range enemy;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void NormalAttackCollider()
    {
        enemy.normalAttackCollider.SetActive(enemy.normalAttackCollider.activeSelf ? false : true);
    }

    public void NormalAttackOver()
    {
        anim.SetBool("isNormalAttack", false);
    }

    public void NormalShotCall()
    {
        enemy.NormalShotCall();
    }

    public void HormalShotOver()
    {
        anim.SetBool("isNormalShotAttack", false);
    }

    public void ContinuousShotOver()
    {
        anim.SetBool("isContinuousShot", false);
    }

    public void HitOver()
    {
        anim.SetBool("isHit", false);
    }

    public void DieOver()
    {
        anim.SetBool("isDie", false);
    }
}
