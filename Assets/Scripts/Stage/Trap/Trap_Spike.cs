using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing;

public class Trap_Spike : Trap_Base
{
    [Header("--- Spike Setting ---")]
    [SerializeField] private SpikeType type;
    [SerializeField] private int damage;
    [SerializeField] private Animator[] anim;
    [SerializeField] private Transform resetPos;
    private enum SpikeType { None, Move }

    [Header("---Object---")]
    [SerializeField] private GameObject spikeObject;

    public override void TrapActivate(bool activate)
    {
        isActivate = activate;
        if(isActivate)
        {
            switch (type)
            {
                case SpikeType.None:
                    for (int i = 0; i < anim.Length; i++)
                    {
                        anim[i].SetBool("isAttack", isActivate);
                    }
                    break;

                case SpikeType.Move:
                    StartCoroutine(SpawnMove(isActivate));
                    break;
            }
        }
        else
        {
            StartCoroutine(SpawnMove(false));
        }
    }

    private IEnumerator SpawnMove(bool isOn)
    {
        // Spike On
        for (int i = 0; i < anim.Length; i++)
        {
            anim[i].SetBool("isAttack", isOn);

        }
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && isActivate)
        {
            collision.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            collision.gameObject.transform.position = resetPos.position;
            collision.gameObject.GetComponent<Player_Status>().TakeDamage(damage, 1, false, Player_Status.HitColor.None, Player_Status.HitType.None);
        }
    }
}
