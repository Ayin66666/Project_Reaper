using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
    [SerializeField] private int maxSpawnCount;
    [SerializeField] private float spawnTimer;
    [SerializeField] private GameObject spawnVFX;
    [SerializeField] private float timer;


    [Header("---Boss Spawn Setting---")]
    [SerializeField] private GameObject boss;
    private Coroutine curCorouttine;
    private Coroutine curCheckCoroutine;


    [Header("---EnemyCount---")]
    [SerializeField] protected GameObject enemyCountSet;
    [SerializeField] protected Text enemyCountText;
    [SerializeField] protected bool haveEnemyCount;


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
        enemyCountSet.SetActive(false);

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
            if(enemyCountSet.activeSelf)
                enemyCountText.text = $"남은 몬스터 : {enemyCount}";

            // Check Delay
            yield return new WaitForSeconds(spawnerCheckDelay);
        }
    }

    private IEnumerator Spawn_Normal()
    {
        // Spawn
        enemyCount = enemys.Count;
        enemyCountSet.SetActive(true);
        enemyCountText.text = $"남은 몬스터 : {enemyCount}";

        for (int i = 0; i < enemys.Count; i++)
        {
            // 이펙트
            Instantiate(spawnVFX, enemys[i].transform.position, Quaternion.identity);

            // 몬스터
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
        enemyCountSet.SetActive(true);

        for (int i = 0; i < spawnCount.Length; i++)
        {
            enemyCountText.text = $"남은 페이즈 : {spawnCount.Length - i}";
            // Spawn -> 몇 마리 소환할건가 ?
            for (int i2 = 0; i2 < spawnCount[i]; i2++)
            {
                GameObject obj = Instantiate(objectSpawnEnemys[Random.Range(0, objectSpawnEnemys.Length)], spawnPos[Random.Range(0,spawnPos.Length)].position, Quaternion.identity);

                // 이펙트
                Instantiate(spawnVFX, obj.transform.position, Quaternion.identity);

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
        maxSpawnCount = spawnPos.Length;
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
                enemys.RemoveAll(e => e == null);
                if (enemys.Count < maxSpawnCount)
                {
                    int spawnCount = maxSpawnCount - enemys.Count;
                    for (int i2 = 0; i2 < spawnCount; i2++)
                    {
                        GameObject obj = Instantiate(objectSpawnEnemys[Random.Range(0, objectSpawnEnemys.Length)], spawnPos[Random.Range(0, spawnPos.Length)].position, Quaternion.identity);
                        enemys.Add(obj);

                        // 이펙트
                        Instantiate(spawnVFX, obj.transform.position, Quaternion.identity);
                    }
                }
            }

            yield return null;
        }
    }

    private IEnumerator Object_Check()
    {
        float count = enemy_Object.Count;

        enemyCountSet.SetActive(true);
        enemyCountText.text = $"남은 오브젝트 수 : {count}";

        while (count > 0)
        {
            // Object Check
            for (int i = 0; i < enemy_Object.Count; i++)
            {
                // 오브젝트 소환
                enemy_Object[i].SetActive(true);

                // 파괴 대기
                while (enemy_Object[i] != null)
                {
                    yield return null;
                }

                // 최신화
                count--;
                enemyCountText.text = $"남은 오브젝트 수 : {count}";
            }

            // Check Delay
            yield return new WaitForSeconds(0.5f);
        }

        StartCoroutine(End_Spawn());
    }

    private void Object_EnemyDestory()
    {
        if (enemys.Count > 0)
        {
            // Object Check
            for (int i = 0; i < enemys.Count; i++)
            {
                if (enemys[i] != null)
                {
                    enemys[i].GetComponent<Enemy_Base>().Die();
                }
            }
        }

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
