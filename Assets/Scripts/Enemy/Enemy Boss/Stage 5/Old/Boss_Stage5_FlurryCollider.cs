using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Stage5_FlurryCollider : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float attackTimer;
    [SerializeField] private bool canAttack;
    private float curTimer;

    private void Update()
    {
        if(curTimer > 0)
        {
            curTimer -= Time.deltaTime;
        }

        if(curTimer <= 0 && !canAttack)
        {
            canAttack = true;
            curTimer = 0;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canAttack)
        {
            canAttack = false;
            curTimer = attackTimer;
            collision.GetComponent<Player_Status>().TakeDamage(damage, 1, false, Player_Status.HitColor.None, Player_Status.HitType.None);
        }
    }
}
