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

    // 경고 콜라이더 On/Off
    public void Attack_Warring(int index)
    {
        boss.attack_Warring[index].SetActive(!boss.attack_Warring[index].activeSelf);
    }

    // Sound Index
    // 0 : Counter
    // 1 : Combo A B C, Air Slash, backstep Slash
    // 2 : Ground Charge, Super Charge
    // 3 : Ground & Air Rush
    // 4 : Super B
    // 5 : Explosion A - Air Main
    // 6 : Explosion B - Air Sub, Backstep Explosion

    // Sound Play
    public void SoundCall(int index)
    {
        boss.sound.SoundPlay_Other(index);
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
        boss.attack_Warring[0].SetActive(false);
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

    public void ComboAttack(int index)
    {
        // Warring Off
        boss.attack_Warring[1].SetActive(false);

        // Attack Collider
        boss.ComboCollider(index);
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
        // boss.sound.SoundPlay_Other(1);
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
        //boss.sound.SoundPlay_Other(3);
    }

    public void SuperAnim()
    {
        anim.SetBool("isSuperSlash", false);
    }
}
