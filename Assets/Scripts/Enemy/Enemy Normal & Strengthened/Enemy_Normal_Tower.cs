using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing;

public class Enemy_Normal_Tower : Enemy_Base
{
    [Header("--- Normal Tower Setting ---")]
    [SerializeField] private Enemy_Tower_TargetCheck attackRange;
    [SerializeField] private LineRenderer line;
    [SerializeField] private Transform shotPos;
    [SerializeField] private float bulletSpeed;
    private int attackCount;

    [Header("--- Prefabs ---")]
    [SerializeField] private GameObject spawnEffect;
    [SerializeField] private GameObject dieEffect;
    [SerializeField] private GameObject shot360Effect;
    [SerializeField] private GameObject bullet;

    // Sound Index
    // 0 : Normal Shot
    // 1 : Circle Shot

    private void Start()
    {
        Status_Setting();
        Target_Setting();
        Spawn();
    }

    private void Update()
    {
        if (state == State.Spawn || state == State.Die)
        {
            return;
        }

        // Find Target & Reset Enemy
        if(attackRange.haveTarget)
        {
            curTarget = attackRange.targets[Random.Range(0, attackRange.targets.Count)];
            haveTarget = true;
            CurTarget_Check();
        }
        else
        {
            haveTarget = false;
        }

        // Test
        if (Input.GetKeyDown(KeyCode.K))
        {
            // StartCoroutine(DashAttack());
        }

        // Think
        if (state == State.Idle && !isAttack && !isDie)
        {
            Think();
        }
        else
        {
            return;
        }
    }

    private void Think()
    {
        state = State.Think;

        if (attackCount >= 5)
        {
            StartCoroutine(CircleShot());
        }
        else
        {
            StartCoroutine(NormalShot());
        }
    }

    private IEnumerator NormalShot()
    {
        state = State.Attack;
        isAttack = true;
        attackCount++;

        // Aiming
        line.enabled = true;
        line.SetPosition(0, shotPos.position);
        float timer = 0.5f;
        while(timer > 0)
        {
            CurTarget_Check();
            line.SetPosition(1, curTarget.transform.position);
            timer -= Time.deltaTime;
            yield return null;
        }
        line.enabled = false;

        // Delay F
        yield return new WaitForSeconds(0.25f);

        // Sound
        sound.SoundPlay_Other(0);

        // Attack
        GameObject obj = Instantiate(bullet, shotPos.position, Quaternion.identity);
        Vector2 shotDir = (curTarget.transform.position - shotPos.position).normalized;
        obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.None, shotDir, bulletSpeed, bulletSpeed * 2f, 15f);

        // Attack Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
    }

    private IEnumerator HowitzerShot()
    {
        state = State.Attack;
        isAttack = true;
        attackCount++;

        // Aiming
        line.enabled = true;
        line.SetPosition(0, shotPos.position);
        float timer = 0.5f;
        while (timer > 0)
        {
            CurTarget_Check();
            line.SetPosition(1, curTarget.transform.position);
            timer -= Time.deltaTime;
            yield return null;
        }
        line.enabled = false;

        // Attack Setting
        Vector2 startPos = shotPos.position;
        Vector2 endPos = curTarget.transform.position;
        GameObject obj = Instantiate(bullet, shotPos.position, Quaternion.identity);

        // Delay F
        yield return new WaitForSeconds(0.25f);

        // Attack
        timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            obj.transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.OutQuart(timer));
            yield return null;
        }

        // Attack Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
    }

    private IEnumerator CircleShot()
    {
        state = State.Attack;
        isAttack = true;
        attackCount = 0;

        // Effect On
        shot360Effect.SetActive(true);
        float timer = 1f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        // Sound
        sound.SoundPlay_Other(1);

        // Attack
        BulletSpawn360(15);

        // Attack Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
    }

    private void BulletSpawn360(int count)
    {
        float angleStep = 360f / count; // count = ¼ÒÈ¯ÇÒ ÅºÀÇ °¹¼ö
        float anlge = 0;
        float radius = 1f;

        Transform startPos = shotPos;
        for (int i = 0; i < count; i++)
        {
            float bulletX = startPos.position.x + Mathf.Sin((anlge * Mathf.PI) / 180) * radius;
            float bulletY = startPos.position.y + Mathf.Cos((anlge * Mathf.PI) / 180) * radius;

            Vector3 projectVec = new Vector3(bulletX, bulletY, 0);
            Vector3 projectDir = (projectVec - startPos.position).normalized;

            // Bullet Spawn
            GameObject obj = Instantiate(bullet, shotPos.position, Quaternion.identity);
            obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.None, projectDir, 5, 30, 10);


            anlge += angleStep;
        }
    }

    protected override void Spawn()
    {
        StartCoroutine(SpawnCall());
    }

    private IEnumerator SpawnCall()
    {
        state = State.Spawn;
        line.enabled = false;
        statusUI_Normal.Status_Setting();

        // Effect On
        spawnEffect.SetActive(true);
        while(spawnEffect.activeSelf)
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.25f);

        state = State.Idle;
    }

    protected override void Stagger()
    {
        throw new System.NotImplementedException();
    }

    public override void Die()
    {
        StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        state = State.Die;
        isDie = true;

        // Sound
        sound.SoundPlay_public(Enemy_Sound.PublicSound.Die);

        // Effect On
        dieEffect.SetActive(true);
        float timer = 1;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(1f);

        // Destory
        Destroy(gameObject);
    }
}
