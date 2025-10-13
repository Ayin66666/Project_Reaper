using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AttackCollider : MonoBehaviour
{
    [Header("---Attack Status---")]
    public Enemy_Base enemy;
    [SerializeField] private bool isStagger;
    [SerializeField] private float motionVelue;
    [SerializeField] private AttackColor attackColor;
    private enum AttackColor { None, Red, Blue }
    private int damage;
    private float criticalChance;
    private float criticalMultiplier;

    private void Awake()
    {
        damage = enemy.damage;
        criticalChance = enemy.criticalChance;
        criticalMultiplier = enemy.criticalMultiplier;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            // Critical Cal
            int ran = Random.Range(0, 100);
            if(ran <= criticalChance)
            {
                damage = (int)(damage * motionVelue * criticalMultiplier);
                switch (attackColor)
                {
                    case AttackColor.None:
                        collision.GetComponent<Player_Status>().TakeDamage(damage, 1, true, Player_Status.HitColor.None, (isStagger ? Player_Status.HitType.Stagger : Player_Status.HitType.None));
                        break;
                    case AttackColor.Red:
                        collision.GetComponent<Player_Status>().TakeDamage(damage, 1, true, Player_Status.HitColor.Red, (isStagger ? Player_Status.HitType.Stagger : Player_Status.HitType.None));
                        break;
                    case AttackColor.Blue:
                        collision.GetComponent<Player_Status>().TakeDamage(damage, 1, true, Player_Status.HitColor.Blue, (isStagger ? Player_Status.HitType.Stagger : Player_Status.HitType.None));
                        break;
                }
            }
            else
            {
                // Normal Cal
                damage = (int)(damage * motionVelue);
                switch (attackColor)
                {
                    case AttackColor.None:
                        collision.GetComponent<Player_Status>().TakeDamage(damage, 1, false, Player_Status.HitColor.None, (isStagger ? Player_Status.HitType.Stagger : Player_Status.HitType.None));
                        break;
                    case AttackColor.Red:
                        collision.GetComponent<Player_Status>().TakeDamage(damage, 1, false, Player_Status.HitColor.Red, (isStagger ? Player_Status.HitType.Stagger : Player_Status.HitType.None));
                        break;
                    case AttackColor.Blue:
                        collision.GetComponent<Player_Status>().TakeDamage(damage, 1, false, Player_Status.HitColor.Blue, (isStagger ? Player_Status.HitType.Stagger : Player_Status.HitType.None));
                        break;
                }
            }
        }
    }
}
