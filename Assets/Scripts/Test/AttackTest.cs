using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTest : MonoBehaviour
{
    [SerializeField] private Transform pos;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy_Base>().TakeDamage(this.gameObject, 50, 1, false, Enemy_Base.HitType.None, 2f, pos.position);
        }
    }
}
