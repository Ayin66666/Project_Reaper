using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss_Stage5_AnimationEventNew : MonoBehaviour
{
    [SerializeField] private Enemy_Boss_Stage5New boss;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    public void AttackOver()
    {
        anim.SetBool("isAttack", false);
    }

    #region
    public void Combo_Collider(int index)
    {
        //boss.comboCollider[index].SetActive(!boss.comboCollider[index].activeSelf);
    }

    public void ComboOver()
    {
        anim.SetBool("isCombo", false);
    }

    public void RushOver()
    {
        anim.SetBool("isRush", false);
    }


    public void GroundSlash_Collider(int index)
    {
        //boss.groundSlashCollider[index].SetActive(!boss.groundSlashCollider[index].activeSelf);
    }

    public void GroundSlashOver()
    {
        anim.SetBool("isGroundSlash", false);
    }


    public void Backstep_Collider()
    {
        //boss.backstepSlashCollider.SetActive(!boss.backstepSlashCollider.activeSelf);
    }

    public void BackstepSlashOver()
    {
        anim.SetBool("isBackstepSlash", false);
    }


    public void Backstep_Collider(int index)
    {
        //boss.upperComboCollider[index].SetActive(!boss.upperComboCollider[index].activeSelf);
    }

    public void UpperOver(int index)
    {
        anim.SetBool(index == 0 ? "isUpper" : index == 1 ? "isUpperAir" : "isUpperStrike", false);
    }


    public void SweepingSlash_Collider()
    {
        //boss.sweepingCollider.SetActive(!boss.sweepingCollider.activeSelf);
    }

    public void SweepingSlashOver()
    {
        anim.SetBool("isSweepingAttack", false);
    }


    public void HalfmoonSlash_Collider()
    {
        //boss.halfmoonSlashCollider.SetActive(!boss.halfmoonSlashCollider.activeSelf);
    }

    public void HalfmoonSlashOver(int index)
    {
        anim.SetBool(index == 0 ? "isHaifmoonSlash" : "isHaifmoonStrike", false);
    }


    public void SuperHalfmooonSlash_Collider(int index)
    {
        //boss.superHalfmoonCollider[index].SetActive(!boss.superHalfmoonCollider[index].activeSelf);
    }

    public void SuperHalfmoonSlashOver()
    {
        anim.SetBool("isSuperHaifmoonSlash", false);
    }
    #endregion
}
