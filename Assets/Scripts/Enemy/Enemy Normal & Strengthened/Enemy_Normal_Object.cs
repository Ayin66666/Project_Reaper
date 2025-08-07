using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Normal_Object : Enemy_Base
{
    [Header("---Normal VFX---")]
    [SerializeField] private GameObject spawnVFX;
    [SerializeField] private GameObject dieVFX;

    [Header("---Boss VFX---")]
    [SerializeField] private GameObject spawnBossVFX;
    [SerializeField] private GameObject dieBossVFX;
    private void Start()
    {
        Status_Setting();
        Spawn();
        if(enemyType == EnemyType.Object)
        {
            statusUI_Normal.Status_Setting();
        }
    }

    protected override void Spawn()
    {
        hitStopCoroutine = StartCoroutine(SpawnCall());
    }

    private IEnumerator SpawnCall()
    {
        state = State.Spawn;

        // Object Type Check
        if (enemyType == EnemyType.Object)
        {
            // Normal Obejct
            spawnVFX.SetActive(true);
            while (spawnVFX.activeSelf)
            {
                yield return null;
            }
        }
        else
        {
            // Stage 4 Object
            statusUI_Boss.StartNameFadeCall();
            spawnBossVFX.SetActive(true);
            while (spawnBossVFX.activeSelf)
            {
                yield return null;
            }
        }

        state = State.Idle;
    }

    public override void Die()
    {
        hitStopCoroutine = StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        state = State.Die;

        if(enemyType == EnemyType.Object)
        {
            dieVFX.SetActive(true);
            while (dieVFX.activeSelf)
            {
                yield return null;
            }
        }
        else
        {
            dieBossVFX.SetActive(true);
            while (dieBossVFX.activeSelf)
            {
                yield return null;
            }
        }

        Destroy(gameObject);
    }

    protected override void Stagger()
    {
        Debug.Log("Call");
    }
}
