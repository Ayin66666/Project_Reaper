using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss_PhaseCange_AnimationEvent : MonoBehaviour
{
    [SerializeField] private Enemy_Boss_PhaseChange boss;
    private Animator anim;


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }



    // BulletHell -> Phase 1
    public void BulletHellChargeAnim()
    {
        anim.SetBool("isAttackReady", false);
    }

    public void BulletHellAttackAnim()
    {
        anim.SetBool("isAOE", false);
    }


    // Air Rush -> Phase 2
    public void AirRushAnim()
    {
        anim.SetBool("isAirRush", false);
    }
    public void AirShot1()
    {
        boss.AirShotCall1();
    }

    public void AirRushLandingAnim()
    {
        anim.SetBool("isAirRushLanding", false);
    }
}
