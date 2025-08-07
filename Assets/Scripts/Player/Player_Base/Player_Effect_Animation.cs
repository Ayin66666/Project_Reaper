using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Effect_Animation : MonoBehaviour
{
    public Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OffEffect()
    {
        this.gameObject.SetActive(false);
    }

    public void WhiteSkill2_Loop_Sound()
    {
        Player_Sound.instance.SFXPlay(Player_Sound.instance.blue_Skill_Sound[1]);
    }
}
