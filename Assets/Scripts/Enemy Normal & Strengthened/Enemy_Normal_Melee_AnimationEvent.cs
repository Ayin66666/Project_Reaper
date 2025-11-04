using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Normal_Melee_AnimationEvent : MonoBehaviour
{
    [SerializeField] private Enemy_Normal_Melee enemy;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void NormalAttackCollider()
    {
        enemy.NormalAttackCollider();
    }

    public void NormalAttackOver()
    {
        anim.SetBool("isNormalAttack", false);
    }

    public void SpawnOver()
    {
        anim.SetBool("isSpawn", false);
    }

    public void HitOver()
    {
        anim.SetBool("isHIt", false);
    }

    public void DieOver()
    {
        anim.SetBool("isDie", false);
    }
}
