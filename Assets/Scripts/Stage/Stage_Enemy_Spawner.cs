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
                enemyCountText.text = $"���� ���� : {enemyCount}";

            // Check Delay
            yield return new WaitForSeconds(spawnerCheckDelay);
        }
    }

    private IEnumerator Spawn_Normal()
    {
        // Spawn
        enemyCount = enemys.Count;
        enemyCountSet.SetActive(true);
        enemyCountText.text = $"���� ���� : {enemyCount}";

        for (int i = 0; i < enemys.Count; i++)
        {
            // ����Ʈ
            Instantiate(spawnVFX, enemys[i].transform.position, Quaternion.identity);

            // ����
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
        // Spawn -> �� ���� ���� ���� ��ȯ�Ǵ°� ?
        enemyCountSet.SetActive(true);

        for (int i = 0; i < spawnCount.Length; i++)
        {
            enemyCountText.text = $"���� ������ : {spawnCount.Length - i}";
            // Spawn -> �� ���� ��ȯ�Ұǰ� ?
            for (int i2 = 0; i2 < spawnCount[i]; i2++)
            {
                GameObject obj = Instantiate(objectSpawnEnemys[Random.Range(0, objectSpawnEnemys.Length)], spawnPos[Random.Range(0,spawnPos.Length)].position, Quaternion.identity);

                // ����Ʈ
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

                        // ����Ʈ
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
        enemyCountText.text = $"���� ������Ʈ �� : {count}";

        while (count > 0)
        {
            // Object Check
            for (int i = 0; i < enemy_Object.Count; i++)
            {
                // ������Ʈ ��ȯ
                enemy_Object[i].SetActive(true);

                // �ı� ���
                while (enemy_Object[i] != null)
                {
                    yield return null;
                }

                // �ֽ�ȭ
                count--;
                enemyCountText.text = $"���� ������Ʈ �� : {count}";
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

    private IEnumerator Spawn_Boss() // ���� ���� �߰� �ý����� ����ؼ� ���� -> �⺻ ����� Normal�� ������
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
