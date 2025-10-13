using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_AttackCollider : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private bool isFlames;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isFlames)
            {
                Debug.Log("Hit");
                // �÷��̾� ��� ���� ����
            }
            else
            {
                Debug.Log("Hit");
                collision.GetComponent<Player_Status>().TakeDamage(damage, 1, false, Player_Status.HitColor.None, Player_Status.HitType.Stagger);
            }
        }
    }
}
