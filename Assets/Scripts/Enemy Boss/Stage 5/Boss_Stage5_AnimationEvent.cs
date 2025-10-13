using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Stage5_AnimationEvent : MonoBehaviour
{
    [SerializeField] private Enemy_Boss_Stage5 boss;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Spawn
    public void SpawnOver()
    {
        anim.SetBool("isSpawn", false);
    }


    // Die
    public void DieOver()
    {
        anim.SetBool("isDie", false);
    }


    // Teleport
    public void TeleportOver()
    {
        anim.SetBool("isTeleport", false);
    }


    // Upward
    public void UpwardAttack()
    {
        boss.UpwardCollider();
    }
    public void UPwardMove()
    {
        boss.UpwardMoveCall();
    }
    public void UpwardOver()
    {
        anim.SetBool("isUpawrdSlash", false);
    }


    // Ground One Slash
    public void GroundOneSlashOver()
    {
        anim.SetBool("isGroundOneSlash", false);
    }


    // Ground Flurry
    public void GroundFlurryCollider1()
    {
        boss.groundFlurryColliderB1Call();
    }
    public void GroundFlurryOver()
    {
        anim.SetBool("isGroundFlurry", false);
    }


    // Combo
    public void ComboMove()
    {
        boss.ComboMoveCall();
    }
    public void ComboA()
    {
        boss.ComboACollider();
    }
    public void ComboB()
    {
        boss.ComboBCollider();
    }
    public void ComboC1()
    {
        boss.ComboC1Collider();
    }
    public void ComboC2()
    {
        boss.ComboC2Collider();
    }
    public void ComboShoot()
    {
        boss.ComboShootCall();
    }
    public void ComboOver()
    {
        anim.SetBool("isComboSlash", false);
    }


    // Half Moon Slash
    public void HalfMoonSlashCollider()
    {
        boss.HaifMoonCollider();
    }
    public void HalfMoonOver()
    {
        anim.SetBool("isHalfMoonSlash", false);
    }


    // Sweeping Slash
    public void SweepingSlashOver()
    {
        anim.SetBool("isSweepingSlash", false);
    }


    // Center Slash
    public void CenterSlashCollider()
    {
        Debug.Log("CenterSlashCollider");
        boss.CenterSlashCollider();
    }
    public void CenterOver()
    {
        Debug.Log("CenterOver");

        anim.SetBool("isCenterSlash", false);
        boss.CenterExplosionCall();
    }


    // Falling
    public void FallingOver()
    {
        anim.SetBool("isFallingEndAnim", false);
    }
}
