using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bullet : MonoBehaviour
{
    [Header("---Componemt---")]
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private Collider2D bulletCollider;
    [SerializeField] private GameObject hitEffect;

    [Header("---Status---")]
    [SerializeField] private int damage;
    [SerializeField] private float curSpeed;
    [SerializeField] private float maxSpeed;
    private Coroutine hitCoroutine;

    public enum BulletType { None, Blue, Red }
    public BulletType bulletType;
    public enum Bullet { Bullet, Wave }
    public Bullet bullet;

    public void Bullet_Setting(BulletType type, Vector2 moveDir, float moveSpeed, float maxSpeed, float lifeTimer)
    {
        bulletType = type;
        curSpeed = moveSpeed;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, moveDir);
        hitCoroutine = StartCoroutine(Bullet_Movement(lifeTimer, moveDir));
    }

    private IEnumerator Bullet_Movement(float lifeTimer, Vector2 moveDir)
    {
        // Rotation
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Timer
        float timer = lifeTimer;
        while(timer > 0)
        {
            // Add Speed
            if(curSpeed < maxSpeed)
                curSpeed += 15f * Time.deltaTime;

            // Move
            rigid.velocity = moveDir.normalized * curSpeed;
            timer -= Time.deltaTime;
            yield return null;
        }

        // End
        HitEffect();
    }

    private void HitEffect()
    {
        if(hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }

        rigid.velocity = Vector2.zero;
        bulletCollider.enabled = false;

        // Hit Efftect
        hitEffect.transform.parent = null;
        hitEffect.SetActive(true);

        // Destory Bullet
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player OR Player Attack Hit
        switch (bulletType)
        {
            case BulletType.Blue:
                if (collision.CompareTag("Player") && collision.GetComponent<Player_Status>().myColor == Player_Status.Mycolor.Red)
                {
                    collision.GetComponent<Player_Status>().TakeDamage(damage, 1, false, Player_Status.HitColor.Blue, Player_Status.HitType.None);
                    HitEffect();
                }
                break;

            case BulletType.Red:
                if (collision.CompareTag("Player") && collision.GetComponent<Player_Status>().myColor == Player_Status.Mycolor.Blue)
                {
                    collision.GetComponent<Player_Status>().TakeDamage(damage, 1, false, Player_Status.HitColor.Red, Player_Status.HitType.None);
                    HitEffect();
                }
                break;

            case BulletType.None:
                if (collision.CompareTag("Player"))
                {
                    collision.GetComponent<Player_Status>().TakeDamage(damage, 1, false, Player_Status.HitColor.None, Player_Status.HitType.None);
                    HitEffect();
                }
                break;
        }

        // Ground Hit
        if (collision.CompareTag("Ground"))
        {
            switch (bullet)
            {
                case Bullet.Bullet:
                    HitEffect();
                    break;

                case Bullet.Wave:
                    break;
            }
        }

        if(collision.CompareTag("Wall"))
        {
            HitEffect();
        }
    }
}
