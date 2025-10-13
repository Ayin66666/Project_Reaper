using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Stage2_AnimationEvent : MonoBehaviour
{
    [SerializeField] private Enemy_Boss_Stage2 boss;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Spawn & Die
    public void SpawnAnim()
    {
        anim.SetBool("isSpawn", false);
    }

    public void DieAnim()
    {
        anim.SetBool("isDie", false);
    }
    public void PhaseAnim()
    {
        anim.SetBool("isPhase2", false);
    }

    // Guard
    public void GuardAnim()
    {
        anim.SetBool("isGuard", false);
    }
    public void GuardWait()
    {
        anim.SetBool("isCount", false);
    }

    // Counter Attack
    public void CounterAnim()
    {
        anim.SetBool("isCountSlash", false);
    }

    public void CounterSworadAura()
    {
        boss.CountShotCall();
    }

    public void CounterAttack()
    {
        boss.CoounterAttackCollider();
    }


    // Combo
    public void ComboAnim()
    {
        anim.SetBool("isComboSlash", false);
    }

    public void ComboShotCall()
    {
        boss.ComboShotCall();
    }

    public void ComboMoveCall()
    {
        boss.ComboMoveCall();
    }

    public void ComboAttack1()
    {
        boss.ComboColliderA();
    }

    public void ComboAttack2()
    {
        boss.ComboColliderB();
    }

    public void ComboAttack3()
    {
        boss.ComboColliderC();
    }


    // Ground Rush
    public void GroundRushMove()
    {
        boss.GroundRushMoveCall();
    }

    public void GroundRushAnim()
    {
        anim.SetBool("isGroundRush", false);
    }


    // Air Rush
    public void AirRushAnim()
    {
        anim.SetBool("isAirRush", false);
    }
    public void AirShot1()
    {
        boss.AirShotCall1();
    }
    public void AirShot2()
    {
        boss.AirShotCall2();
    }

    public void AirRushLandingAnim()
    {
        anim.SetBool("isAirRushLanding", false);
    }

    // Backstep
    public void BackstepAnim()
    {
        anim.SetBool("isBackstep", false);
    }

    public void BackstepAttack()
    {
        boss.BackstepCollider();
    }

    public void BackstepExplosion()
    {
        boss.BackstepExplosionCall();
    }

    public void BackstepSlashAnim()
    {
        anim.SetBool("isBackstepSlash", false);
    }

    // Super
    public void SuperAttack()
    {
        boss.SuperColliderCall();
    }

    public void SuperAnim()
    {
        anim.SetBool("isSuperSlash", false);
    }
}
