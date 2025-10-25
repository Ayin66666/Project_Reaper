using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Tutorial_AnimationEvent : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
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
