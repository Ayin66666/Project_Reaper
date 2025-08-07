using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Stage1_AnimationEvent : MonoBehaviour
{
    [SerializeField] private Enemy_Boss_Stage1 boss;
    [SerializeField] private Animator anim;

    // 0 : Combo A, Combo B, Combo C, Rolling Slash
    // 1 : Backstep Air Slash, Teleport Slash Attack A, Teleport Slash Attack B
    // 2 : Teleport Slash Move
    // 3 : Bullet Hell Slash A - 시작 사운드
    // 4 : Bullet Hell Slash B - 불릿 발사 사운드

    // Sound
    public void SoundCall(int index)
    {
        boss.sound.SoundPlay_Other(index);
    }

    // Spawn
    public void SpawnAnim()
    {
        anim.SetBool("isSpawn", false);
    }

    // Die
    public void DieAnim()
    {
        anim.SetBool("isDie", false);
    }

    // Teleport Slash
    public void TeleportMoveAnim()
    {
        anim.SetBool("isTeleport", false);
    }

    public void TeleportAttackCollider1()
    {
        boss.TeleportSlashColliderA();
    }

    public void TeleportAttackCollider2()
    {
        boss.TeleportSlashColliderB();
    }

    public void TeleportShotCall()
    {
        boss.TeleportShotCall();
    }

    public void TeleportAttackAnim()
    {
        anim.SetBool("isTeleportSlash", false);
    }

    // Rolling Slash
    public void RollingSlashCollider()
    {
        boss.RollingAttackCollider();
    }

    public void RollingMoveAnim()
    {
        anim.SetBool("isRolling", false);
    }

    public void RollingAttackAnim()
    {
        anim.SetBool("isRollingSlash", false);
        boss.RollingAOECall();
    }

    // Combo Slash
    public void ComboAttackCollider1()
    {
        boss.ComboA();
    }

    public void ComboAttackCollider21()
    {
        boss.ComboB1();
    }

    public void ComboAttackCollider22()
    {
        boss.ComboB2();
    }

    public void ComboAttackCollider3()
    {
        boss.ComboC();
    }

    public void ComboShot()
    {
        boss.ComboShotCall();
    }

    public void ComboAttackAnim()
    {
        anim.SetBool("isComboSlash", false);
    }

    // Backstep Air Slash
    public void BackStepAirSlashCollider()
    {
        boss.BackstepAirSlashCollider();
    }

    public void BackstepAirslashBulletSpawn()
    {
        boss.BackstepShotCall();
    }

    public void BackstepAirSlashAnim()
    {
        anim.SetBool("isBackstepAirSlash", false);
    }

    // BulletHell
    public void BulletHellMoveAnim()
    {
        anim.SetBool("isBulletHellMove", false);
    }

    public void BulletHellChargeAnim()
    {
        anim.SetBool("isBulletHellCharge", false);
    }

    public void BulletHellAttackAnim()
    {
        anim.SetBool("isBulletHellSlash", false);
    }
}
