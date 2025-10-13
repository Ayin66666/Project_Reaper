using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Strengthened_Melee_AnimtionEvent : MonoBehaviour
{
    [SerializeField] private Enemy_Strengthened_Melee enemy;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void NormalOver()
    {
        anim.SetBool("isNormalAttack", false);
    }

    public void NormalAttackCollider()
    {
        enemy.NormalAttackCollider();
    }

    public void BackDashShooting()
    {
        enemy.BackDashShooting();
    }

    public void BackstepShotOver()
    {
        anim.SetBool("isBackDashShot", false);
    }

    public void SpawnOver()
    {
        anim.SetBool("isSpawn", false);
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
