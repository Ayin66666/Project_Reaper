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
    public void Combo_Movement()
    {
        ((Attack_Combo)attack[0]).ComboMove();
    }

    private void Combo_Aura()
    {
        ((Attack_Combo)attack[0]).SwordAura();
    }
    #endregion


    #region Ground Rush
    #endregion
}
