using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing;

public class Trap_Spike : Trap_Base
{
    [Header("--- Spike Setting ---")]
    [SerializeField] private SpikeType type;
    [SerializeField] private int damage;
    private enum SpikeType { None, Move }

    [Header("---Object---")]
    [SerializeField] private GameObject spikeObject;

    [Header("--- Pos Setting ---")]
    [SerializeField] private Transform resetPos;
    [SerializeField] private Transform[] spawnMovePos;

    public override void TrapActivate(bool activate)
    {
        isActivate = activate;
        if(isActivate)
        {
            switch (type)
            {
                case SpikeType.None:
                    spikeObject.SetActive(true);
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
        float timer = 0;

        if (isOn)
        {
            // Spike On
            spikeObject.SetActive(true);
            Vector2 startPos = spawnMovePos[0].position;
            Vector2 endPos = spawnMovePos[1].position;
            while(timer < 1)
            {
                timer += Time.deltaTime;
                spikeObject.transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
                yield return null;
            }
            spikeObject.transform.position = endPos;
        }
        else
        {
            // Spike Off
            Vector2 startPos = spawnMovePos[1].position;
            Vector2 endPos = spawnMovePos[0].position;
            while(timer < 1)
            {
                timer += Time.deltaTime;
                spikeObject.transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
                yield return null;
            }
            spikeObject.transform.position = endPos;
            spikeObject.SetActive(false);
        }
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
