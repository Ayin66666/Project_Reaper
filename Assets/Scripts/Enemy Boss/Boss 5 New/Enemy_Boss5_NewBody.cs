using System;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_Boss5_NewBody : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private Enemy_Boss5_New boss;
    [SerializeField] private List<Attack_Base> attack;
    private Animator anim;


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    public void LookAt()
    {
        boss.LookAt();
    }


    #region Combo
    public void Combo_Attack(int index)
    {
        ((Attack_Combo)attack[0]).Combo_Attack(index);
    }

    public void Combo_Movement()
    {
        LookAt();
        ((Attack_Combo)attack[0]).ComboMove();
    }

    public void Combo_Aura()
    {
        ((Attack_Combo)attack[0]).SwordAura();
    }

    public void ComboOver()
    {
        anim.SetBool("isCombo", false);
    }
    #endregion


    #region Ground Rush
    public void GroundRush_Attack()
    {
        ((Attack_GroundRush)attack[1]).GroundRush_Attack();
    }

    public void GroundRush_SwordAura()
    {
        ((Attack_GroundRush)attack[1]).SwordAura();
    }

    public void GroundRushOver()
    {
        anim.SetBool("isGroundRush", false);
    }
    #endregion
}
