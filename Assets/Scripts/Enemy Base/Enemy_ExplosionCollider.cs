using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ExplosionCollider : MonoBehaviour
{
    [Header("---Attack Setting---")]
    [SerializeField] private int damage;
    public AttackType attackType;

    public enum AttackType { Normal, Red, Blue }

    public void Setting(int damage)
    {
        this.damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            switch (attackType)
            {
                case AttackType.Normal:
                    other.GetComponent<Player_Status>().TakeDamage(damage, 1, false, Player_Status.HitColor.None, Player_Status.HitType.None);
                    break;

                case AttackType.Red:
                    other.GetComponent<Player_Status>().TakeDamage(damage, 1, false, Player_Status.HitColor.Red, Player_Status.HitType.None);
                    break;

                case AttackType.Blue:
                    other.GetComponent<Player_Status>().TakeDamage(damage, 1, false, Player_Status.HitColor.Blue, Player_Status.HitType.None);
                    break;
            }
        }
    }
}
