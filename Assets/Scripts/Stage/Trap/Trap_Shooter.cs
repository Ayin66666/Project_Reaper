using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Trap_Shooter : Trap_Base
{
    [Header("--- Shooter Setting ---")]
    [SerializeField] private int damage;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float shootDelay;
    [SerializeField] private float firstShotDelay;
    [SerializeField] private BulletType bulletType;
    [SerializeField] private bool isChange;
    private float curDelay;
    private enum BulletType { None, White, Black }

    [Header("--- Prefabs ---")]
    [SerializeField] private GameObject[] bullets;

    [Header("--- Pos Setting ---")]
    [SerializeField] private Transform shotPos;

    public override void TrapActivate(bool activate)
    {
        isActivate = activate;
        curDelay = firstShotDelay;
    }

    private void Update()
    {
        if(isActivate)
        {
            // Attack
            if (curDelay <= 0)
            {
                Shoot();
            }

            // Cooldown
            if (shootDelay >= 0)
            {
                curDelay -= Time.deltaTime;
            }
        }
    }

    private void Shoot()
    {
        // Attack
        Vector2 shotDir = (shotPos.position - transform.position).normalized;
        switch (bulletType)
        {
            case BulletType.None:
                GameObject obj1 = Instantiate(bullets[0], shotPos.position, Quaternion.identity);
                obj1.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.None, shotDir, bulletSpeed, bulletSpeed * 2f, 15f);
                break;

            case BulletType.White:
                GameObject obj2 = Instantiate(bullets[1], shotPos.position, Quaternion.identity);
                obj2.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.Red, shotDir, bulletSpeed, bulletSpeed * 2f, 15f);
                break;

            case BulletType.Black:
                GameObject obj3 = Instantiate(bullets[2], shotPos.position, Quaternion.identity);
                obj3.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.Blue, shotDir, bulletSpeed, bulletSpeed * 2f, 15f);
                break;
        }

        // Delay Reset
        curDelay = shootDelay;

        // Bullet Change
        if(isChange)
        {
            if(bulletType == BulletType.White)
            {
                bulletType = BulletType.Black;
            }
            else if(bulletType == BulletType.Black)
            {
                bulletType = BulletType.White;
            }
        }
    }
}
