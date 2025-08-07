using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_Enemy_Spawner : MonoBehaviour
{
    [Header("--- Spawner Setting ---")]
    [SerializeField] private SpawnType spawnType;
    [SerializeField] private bool isSpawn;
    [SerializeField] private float spawnerCheckDelay;
    [SerializeField] private List<GameObject> enemys;
    private int enemyCount;
    private enum SpawnType { Normal, Phase, Object, Boss }


    [Header("---Phase Spawn Setting---")]
    [SerializeField] private int[] spawnCount;


    [Header("---Object Spawn Setting---")]
    [SerializeField] private List<GameObject> enemy_Object;
    [SerializeField] private GameObject[] objectSpawnEnemys;
    [SerializeField] private Transform[] spawnPos;
    [SerializeField] private float spawnTimer;
    private float timer;


    [Header("---Boss Spawn SEtting---")]
    [SerializeField] private GameObject boss;

    private Coroutine curCorouttine;
    private Coroutine curCheckCoroutine;

    public void Spawn()
    {
        curCorouttine = StartCoroutine(SpawnCall());
    }

    private IEnumerator SpawnCall()
    {
        // Player Check + Door Active
        isSpawn = true;

        // Delay
        yield return new WaitForSeconds(1.5f);

        // Spawn & Spawner Check
        switch (spawnType)
        {
            case SpawnType.Normal:
                curCorouttine = StartCoroutine(Spawn_Normal());
                break;

            case SpawnType.Phase:
                curCorouttine = StartCoroutine(Spawn_Phase());
                break;

            case SpawnType.Object:
                curCorouttine = StartCoroutine(Spawn_Object());
                break;

            case SpawnType.Boss:
                curCorouttine = StartCoroutine(Spawn_Boss());
                break;
        }
    }

    private IEnumerator End_Spawn()
    {
        // Stop Spawn Systeam
        StopCoroutine(curCorouttine);
        isSpawn = false;

        if(spawnType == SpawnType.Object)
        {
            Object_EnemyDestory();
        }

        // Delay
        yield return new WaitForSeconds(0.2f);

        // Destroy
        Destroy(gameObject);
    }

    private IEnumerator Spawn_Check()
    {
        while(enemys.Count > 0)
        {
            for (int i = 0; i < enemys.Count; i++)
            {
                if (enemys[i] == null)
                {
                    enemys.RemoveAt(i);
                }
            }

            enemyCount = enemys.Count;

            // Check Delay
            yield return new WaitForSeconds(spawnerCheckDelay);
        }
    }

    private IEnumerator Spawn_Normal()
    {
        // Spawn
        enemyCount = enemys.Count;
        for (int i = 0; i < enemys.Count; i++)
        {
            enemys[i].SetActive(true);
        }

        // Wait
        curCheckCoroutine = StartCoroutine(Spawn_Check());
        while (enemyCount > 0)
        {
            yield return null;
        }

        // Spawn End
        StartCoroutine(End_Spawn());
    }

    private IEnumerator Spawn_Phase()
    {
        // Spawn -> 몇 번에 걸쳐 나눠 소환되는가 ?
        for (int i = 0; i < spawnCount.Length; i++)
        {
            // Spawn -> 몇 마리 소환할건가 ?
            for (int i2 = 0; i2 < spawnCount[i]; i2++)
            {
                GameObject obj = Instantiate(objectSpawnEnemys[Random.Range(0, objectSpawnEnemys.Length)], spawnPos[Random.Range(0,spawnPos.Length)].position, Quaternion.identity);
                enemys.Add(obj);
                enemyCount++;
            }

            // Enemy Check
            curCheckCoroutine = StartCoroutine(Spawn_Check());
            while (enemyCount > 0)
            {
                yield return null;
            }

            // Next Phase & Spawn End
            if (i < spawnCount.Length)
            {
                // Delay
                yield return new WaitForSeconds(0.25f);
            }
        }

        StartCoroutine(End_Spawn());
    }

    private IEnumerator Spawn_Object()
    {
        isSpawn = true;
        timer = 0;
        curCheckCoroutine = StartCoroutine(Object_Check());

        while (isSpawn)
        {
            // Timer Check
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }

            // Enemy Spawn
            if (timer <= 0)
            {
                timer = spawnTimer;
                for (int i2 = 0; i2 < spawnPos.Length; i2++)
                {
                    GameObject obj = Instantiate(objectSpawnEnemys[Random.Range(0, objectSpawnEnemys.Length)], spawnPos[Random.Range(0, spawnPos.Length)].position, Quaternion.identity);
                    enemys.Add(obj);
                }
            }

            yield return null;
        }
    }

    private IEnumerator Object_Check()
    {
        float count = enemy_Object.Count;
        while(count > 0)
        {
            // Object Check
            for (int i = 0; i < enemy_Object.Count; i++)
            {
                // Object Spawn
                enemy_Object[0].SetActive(true);
                while (enemy_Object[0] != null)
                {
                    yield return null;
                }

                // Spawn Speed Up
                spawnTimer *= 0.9f;

                // List Update
                enemy_Object.RemoveAt(0);
                count--;
            }

            // Check Delay
            yield return new WaitForSeconds(0.5f);
        }

        StartCoroutine(End_Spawn());
    }

    private void Object_EnemyDestory()
    {
        Debug.Log("Call A" + enemys.Count);
        if (enemys.Count > 0)
        {
            Debug.Log("Call B");
            // Object Check
            for (int i = 0; i < enemys.Count; i++)
            {
                Debug.Log("Call C");
                if (enemys[i] != null)
                {
                    enemys[i].GetComponent<Enemy_Base>().Die();
                }
                //enemys[i].GetComponent<Enemy_Base>().TakeDamage(gameObject, 99999, 1, false, Enemy_Base.HitType.None, 1, transform.position); ;
            }
        }

        Debug.Log("Call D");

    }

    private IEnumerator Spawn_Boss() // 보스 관련 추가 시스템을 대비해서 제작 -> 기본 기능은 Normal과 동일함
    {
        // Spawn
        enemyCount = 1;
        enemys.Add(boss);
        boss.SetActive(true);

        // Wait
        curCheckCoroutine = StartCoroutine(Spawn_Check());
        while (enemyCount > 0)
        {
            yield return null;
        }

        // Spawn End
        StartCoroutine(End_Spawn());
    }
}
