using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Normal_Range_AnimationEvent : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void NormalShotOver()
    {
        anim.SetBool("isNormalShot", false);
    }

    public void HowitzerShotOver()
    {
        anim.SetBool("isHowitzerShot", false);
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
