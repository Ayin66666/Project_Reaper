using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_AttackCollision : MonoBehaviour
{
    public enum attackType { Normal, Airborne, Down, KnockBack, Stagger, Chase } // 플레이어 일반 공격 타입
    public attackType current_AttackType;

    public enum SkillType { None, Blue1, Blue2, Red1, Red2 } // 플레이어 스킬 타입
    public SkillType current_SkillType;

    [SerializeField] Player_Skill playerSkill;
    [SerializeField] Player_Chase playerChase;

    [SerializeField] private float damageValue;
    [SerializeField] private float hitTimer;
    public int hitCount;
    [SerializeField] Transform airbornePos;
    [SerializeField] Transform downAttackPos;
    [SerializeField] Transform knockBackPos;

    //[SerializeField] GameObject skill_Collider;

    [SerializeField] Collider2D[] enemy;
    [SerializeField] LayerMask enemyCheck;

    Coroutine myCoroutine;

    void Awake()
    {
        hitTimer = -1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (hitTimer > 0)
        {
            hitTimer -= Time.deltaTime;
        }

        if (current_SkillType == SkillType.Blue2)
        {
            CheckEnemy();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) //단타용
    {
        if (collision.CompareTag("Enemy"))
        {

            if(current_SkillType == SkillType.Blue2)
            {
                return;
            }
            else
            {
                if(current_SkillType == SkillType.Red1)
                {
                    Debug.Log("히트");
                    if (playerSkill.red_Skill1_HitCount < 3)
                    {
                        playerSkill.red_Skill1_HitCount++;
                    }
                    else
                    {
                        if (myCoroutine != null)
                        {
                            StopCoroutine(myCoroutine);
                        }
                        myCoroutine = StartCoroutine(Player_Status.instance.ChaseTimer());
                    }
                }
                Debug.Log("테스트");

                switch (current_AttackType)
                {
                    case attackType.Normal:
                        collision.GetComponent<Enemy_Base>().TakeDamage(Player_Status.instance.gameObject, (int)(Player_Status.instance.damage * damageValue), hitCount, false, Enemy_Base.HitType.None, 0.3f, airbornePos.position);

                        break;
                    case attackType.Airborne:
                        collision.GetComponent<Enemy_Base>().TakeDamage(Player_Status.instance.gameObject, (int)(Player_Status.instance.damage * damageValue), hitCount, false, Enemy_Base.HitType.AirBorne, 2f, airbornePos.position);
                        //Player_Status.instance.canChase = true; // 테스트용
                        playerChase.enemy = collision.gameObject;
                        Debug.Log("에어본");
                        break;
                    case attackType.Down:
                        collision.GetComponent<Enemy_Base>().TakeDamage(Player_Status.instance.gameObject, (int)(Player_Status.instance.damage * damageValue), hitCount, false, Enemy_Base.HitType.DownAttack, 15f, downAttackPos.position);

                        break;
                    case attackType.KnockBack:
                        collision.GetComponent<Enemy_Base>().TakeDamage(Player_Status.instance.gameObject, (int)(Player_Status.instance.damage * damageValue), hitCount, false, Enemy_Base.HitType.KnockBack, 10f, knockBackPos.position);

                        break;
                    case attackType.Stagger:
                        collision.GetComponent<Enemy_Base>().TakeDamage(Player_Status.instance.gameObject, (int)(Player_Status.instance.damage * damageValue), hitCount, false, Enemy_Base.HitType.Stagger, 2f, knockBackPos.position);

                        break;
                    case attackType.Chase:
                        collision.GetComponent<Enemy_Base>().TakeDamage(Player_Status.instance.gameObject, (int)(Player_Status.instance.damage * damageValue), hitCount, false, Enemy_Base.HitType.Stagger, 2f, knockBackPos.position);
                        break;
                }
            }
        }
    }


    public void CheckEnemy()
    {
        enemy = Physics2D.OverlapBoxAll(this.transform.position, new Vector2(10, 6), 0, enemyCheck);

        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].GetComponent<Enemy_Base>().TakeDamage(Player_Status.instance.gameObject, (int)(Player_Status.instance.damage * damageValue), hitCount, false, Enemy_Base.HitType.Stagger, 2f, knockBackPos.position);
        }
        Set_HitTimer();
    }

    private void Set_HitTimer()
    {
        hitTimer = 0.2f;
    }
}
