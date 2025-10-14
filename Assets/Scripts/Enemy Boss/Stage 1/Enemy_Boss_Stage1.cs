using Easing;
using System.Collections;
using UnityEngine;


public class Enemy_Boss_Stage1 : Enemy_Base
{
    [Header("---Boss Stage 1 Setting---")]
    [SerializeField] private Stage_Manager stage_Manager;
    [SerializeField] private GameObject container;
    [SerializeField] private CurAttack curAttack;
    private int attackCount;
    private enum ExtraAttackType { None, White, Black }
    private enum CurAttack { None, ComboSlash, RollingSlash, BackstepAirSlash, TeleportSlash, BulletHellSlash }

    [Header("---Move Position---")]
    [SerializeField] private Transform centerPos;
    [SerializeField] private Transform backDashPos;

    [Header("---Attack Collider---")]
    [SerializeField] private GameObject[] bullet;
    [SerializeField] private GameObject[] waveBullet;
    [SerializeField] private GameObject[] explosion;

    public GameObject rollingSlashCollider;
    public GameObject backstepAirSlashCollider;
    public GameObject[] comboAttackCollider;
    public GameObject[] teleportSlashCollider;

    [Header("---Attack Pos---")]
    [SerializeField] private Transform shotPos;
    [SerializeField] private Transform[] rollingExplosionPos;
    [SerializeField] private Transform[] backstepExplosionPos;
    [SerializeField] private Transform[] backstepShotPos;
    [SerializeField] private Transform[] teleportShotPos;

    // Sound Index
    // 0 : Combo A, Combo B
    // 1 : Combo C, Rolling Slash, Backstep Air Slash, Teleport Slash Attack B

    // 2 : Teleport Slash Move
    // 3 : Teleport Slash Attack A

    // 4 : Bullet Hell Slash A
    // 5 : Bullet Hell Slash B

    private void Start()
    {
        Status_Setting();
        Spawn();
    }

    private void Update()
    {
        // Spawn & Die Check
        if (state == State.Spawn || state == State.Die)
        {
            return;
        }

        GroundCheck();

        // Find Target & Reset Enemy
        if (!haveTarget)
        {
            Target_Setting();
            if (!haveTarget && state != State.Await)
            {
                state = State.Await;
            }
        }

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

        // Find Target
        Target_Setting();
        LookAt();

        if (curTarget != null)
        {
            CurTarget_Check();
            if (hitStopCoroutine != null) StopCoroutine(hitStopCoroutine);
            if (attackCount >= 5)
            {
                // 필살기
                hitStopCoroutine = StartCoroutine(BulletHellSlash());
                return;
            }

            if (targetDir <= 5)
            {
                // 근거리 공격
                hitStopCoroutine = StartCoroutine(RollingSlash());
            }
            else
            {
                // 원거리 공격
                int ran = Random.Range(0, 100);
                hitStopCoroutine = StartCoroutine(ran <= 50 ? TeleportSlash() : RollingSlash());
            }
        }
    }

    private IEnumerator RollingSlash()
    {
        curAttack = CurAttack.RollingSlash;
        state = State.Attack;
        isAttack = true;
        attackCount++;

        // LookAt
        CurTarget_Check();
        LookAt();

        // Animation
        anim.SetTrigger("Attack");
        anim.SetBool("isRolling", true);
        anim.SetBool("isRollingSlash", true);

        // 이동
        Vector2 startPos = transform.position;
        Vector2 endPos = curTarget.transform.position;
        endPos.y = transform.position.y;

        // 벽 체크
        Vector2 moveDir = endPos - startPos;
        RaycastHit2D hit = Physics2D.Raycast(startPos, moveDir.normalized, moveDir.magnitude, groundLayer);
        if (hit.collider != null) endPos = hit.point + hit.normal * 0.5f;

        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.InOutQuart(timer));
            yield return null;
        }
        anim.SetBool("isRolling", false);

        // 애니메이션 대기
        while (anim.GetBool("isRollingSlash"))
        {
            yield return null;
        }

        // 딜레이
        yield return new WaitForSeconds(delay);

        // 연계 공격 판단
        CurTarget_Check();
        LookAt();

        // Next Attack Think
        int ran = Random.Range(0, 100);
        StartCoroutine(ran < 50 ? ComboSlash() : BackstepAirSlash());
    } // End

    public void RollingAttackCollider()
    {
        rollingSlashCollider.SetActive(rollingSlashCollider.activeSelf ? false : true);

        // Sound
        if (!rollingSlashCollider.activeSelf)
        {
            sound.SoundPlay_Other(1);
        }
    }

    public void RollingAOECall()
    {
        StartCoroutine(ExtraAOEAttack(rollingExplosionPos, explosion[Random.Range(0, explosion.Length)], 0.07f));
    }

    private IEnumerator ComboSlash() // End
    {
        curAttack = CurAttack.ComboSlash;
        attackCount++;

        anim.SetTrigger("Attack");
        anim.SetBool("isComboSlash", true);

        while (anim.GetBool("isComboSlash"))
        {
            yield return null;
        }

        int ran = Random.Range(0, 100);
        if (ran <= 40)
        {
            StartCoroutine(BackstepAirSlash());
        }
        else
        {
            // 딜레이
            yield return new WaitForSeconds(delay);
            curAttack = CurAttack.None;
            state = State.Idle;
            isAttack = false;
        }
    }

    public void ComboA()
    {
        comboAttackCollider[0].SetActive(comboAttackCollider[0].activeSelf ? false : true);
        if (!comboAttackCollider[0].activeSelf)
        {
            sound.SoundPlay_Other(0);
        }
    }

    public void ComboB1()
    {
        comboAttackCollider[1].SetActive(comboAttackCollider[1].activeSelf ? false : true);
        if (!comboAttackCollider[1].activeSelf)
        {
            sound.SoundPlay_Other(0);
        }
    }

    public void ComboB2()
    {
        comboAttackCollider[2].SetActive(comboAttackCollider[2].activeSelf ? false : true);
        if (!comboAttackCollider[1].activeSelf)
        {
            sound.SoundPlay_Other(0);
        }
    }

    public void ComboC()
    {
        comboAttackCollider[3].SetActive(comboAttackCollider[3].activeSelf ? false : true);
        if (!comboAttackCollider[3].activeSelf)
        {
            sound.SoundPlay_Other(1);
        }
    }

    public void ComboShotCall()
    {
        BulletSpawnRL(1, transform.localScale.x > 0 ? false : true, true);
    }

    private IEnumerator BackstepAirSlash()
    {
        curAttack = CurAttack.BackstepAirSlash;
        isAttack = true;
        attackCount++;

        CurTarget_Check();
        LookAt();

        anim.SetTrigger("Attack");
        anim.SetBool("isBackstep", true);
        anim.SetBool("isBackstepAirSlash", true);

        // Explosion Attack
        StartCoroutine(ExtraAOEAttack(backstepExplosionPos, explosion[Random.Range(0, explosion.Length)], 0.12f));

        // 이동
        Vector2 startPos = transform.position;
        Vector2 endPos = backDashPos.position;

        // 벽 체크
        Vector2 moveDir = endPos - startPos;
        RaycastHit2D hit = Physics2D.Raycast(startPos, moveDir.normalized, moveDir.magnitude, groundLayer);
        if (hit.collider != null) endPos = hit.point + hit.normal * 0.5f;

        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / 0.35f;
            transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.InOutQuart(timer));
            yield return null;
        }
        anim.SetBool("isBackstep", false);

        // 대기
        while (anim.GetBool("isBackstepAirSlash"))
        {
            yield return null;
        }

        // 딜레이
        yield return new WaitForSeconds(delay);
        curAttack = CurAttack.BackstepAirSlash;
        state = State.Idle;
        isAttack = false;
    } // End 1

    public void BackstepAirSlashCollider()
    {
        backstepAirSlashCollider.SetActive(backstepAirSlashCollider.activeSelf ? false : true);
        if (!backstepAirSlashCollider.activeSelf)
        {
            sound.SoundPlay_Other(1);
        }
    }

    public void BackstepShotCall()
    {
        StartCoroutine(BulletShot(backstepShotPos, bullet[Random.Range(0, bullet.Length)], 0f));
    }

    private IEnumerator TeleportSlash()
    {
        curAttack = CurAttack.TeleportSlash;
        isAttack = true;
        attackCount++;

        anim.SetTrigger("Attack");
        anim.SetBool("isTeleport", true);
        anim.SetBool("isTeleportSlash", true);
        anim.SetBool("isTeleprotDelay", true);

        // Sound
        sound.SoundPlay_Other(2);

        // Animaton Wait
        while (anim.GetBool("isTeleport"))
        {
            yield return null;
        }
        spriteRenderer.enabled = false;

        // Move Delay
        yield return new WaitForSeconds(Random.Range(0.5f, 1f));

        // Move
        CurTarget_Check();
        if (targetDir > 0)
        {
            transform.position = new Vector3(curTarget.transform.position.x - 3f, transform.position.y, curTarget.transform.position.z);
        }
        else
        {
            transform.position = new Vector3(curTarget.transform.position.x + 3f, transform.position.y, curTarget.transform.position.z);
        }

        // Extra Attack
        Instantiate(explosion[Random.Range(0, explosion.Length)], transform.position, Quaternion.identity);

        // Extra Attack Delay
        yield return new WaitForSeconds(0.45f);
        anim.SetBool("isTeleprotDelay", false);
        spriteRenderer.enabled = true;

        // LookAt
        CurTarget_Check();
        LookAt();

        // Delay
        yield return new WaitForSeconds(Random.Range(0.15f, 0.25f));

        // Attack
        while (anim.GetBool("isTeleportSlash"))
        {
            yield return null;
        }

        // 딜레이
        yield return new WaitForSeconds(delay);
        curAttack = CurAttack.None;
        state = State.Idle;
        isAttack = false;
    } // End

    public void TeleportSlashColliderA()
    {
        teleportSlashCollider[0].SetActive(teleportSlashCollider[0].activeSelf ? false : true);

        // Sound
        if (!teleportSlashCollider[0].activeSelf)
        {
            sound.SoundPlay_Other(3);
        }
    }

    public void TeleportSlashColliderB()
    {
        teleportSlashCollider[0].SetActive(teleportSlashCollider[1].activeSelf ? false : true);

        // Sound
        if (!teleportSlashCollider[1].activeSelf)
        {
            sound.SoundPlay_Other(1);
        }
    }

    public void TeleportShotCall()
    {
        StartCoroutine(BulletShot(teleportShotPos, bullet[Random.Range(0, bullet.Length)], 0f));
    }

    private IEnumerator BulletHellSlash() // End
    {
        curAttack = CurAttack.BulletHellSlash;
        isAttack = true;
        attackCount = 0;

        anim.SetTrigger("Attack");
        anim.SetBool("isBulletHellMove", true);
        anim.SetBool("isBulletHellCharge", true);
        anim.SetBool("isBulletHellSlash", true);
        anim.SetBool("isBulletHellSlashReady", false);

        // Sound
        sound.SoundPlay_Other(2);

        // 순간이동
        while (anim.GetBool("isBulletHellMove"))
        {
            yield return null;
        }

        // 투명화 ON / Off
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        transform.position = centerPos.position;
        yield return new WaitForSeconds(1.5f);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);

        // 화면 암전? 효과 -> 만약 시간이 남거나 리소스가 있다면
        anim.SetBool("isBulletHellSlashReady", true);

        // Sound
        sound.SoundPlay_Other(4);

        // 차징
        while (anim.GetBool("isBulletHellCharge"))
        {
            yield return null;
        }

        // Sound
        sound.SoundPlay_Other(5);

        // 공격
        StartCoroutine(BulletHellSlashSpawnCall());
        while (anim.GetBool("isBulletHellSlash"))
        {
            yield return null;
        }

        // 대기
        yield return new WaitForSeconds(delay);
        curAttack = CurAttack.None;
        state = State.Idle;
        isAttack = false;
    }

    private IEnumerator BulletHellSlashSpawnCall()
    {
        for (int i = 0; i < 15; i++)
        {
            // Bullet Type & Movement Setting
            int ran = Random.Range(0, 2);
            switch (ran)
            {
                case 0:
                    BulletSpawn360(31, true);
                    break;

                case 1:
                    BulletSpawn360(31, false);
                    break;
            }

            // Big Wave Attack 
            if (i % 3 == 0)
            {
                ran = Random.Range(0, 2);
                switch (ran)
                {
                    case 0:
                        BulletSpawnRL(1, true, true);
                        BulletSpawnRL(1, false, true);
                        break;

                    case 1:
                        BulletSpawnRL(1, true, false);
                        BulletSpawnRL(1, false, false);
                        break;
                }
            }

            // Spawn Delay
            yield return new WaitForSeconds(Random.Range(0.7f, 1.2f));
        }

        anim.SetBool("isBulletHellSlash", false);
    }

    private void BulletSpawnRL(int count, bool isLeft, bool isWhite)
    {
        /*
        if (isLeft)
        {
            // Left Spawn
            for (int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(waveBullet[isWhite ? 0 : 1], shotPos.position, Quaternion.identity);
                obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.Red, -shotPos.right, 10, 25, 15);
            }
        }
        else
        {
            // Right Spawn
            for (int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(waveBullet[isWhite ? 0 : 1], shotPos.position, Quaternion.identity);
                obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.Blue, shotPos.right, 10, 25, 15);
            }
        }
        */
        for (int i = 0; i < count; i++)
        {
            Vector2 dir = isLeft ? -shotPos.right : shotPos.right;
            GameObject obj = Instantiate(waveBullet[isWhite ? 0 : 1], shotPos.position, Quaternion.LookRotation(Vector3.forward, dir));
            obj.GetComponent<Enemy_Bullet>().Bullet_Setting(isLeft ? Enemy_Bullet.BulletType.Red : Enemy_Bullet.BulletType.Blue, dir, 10, 25, 15);
        }
    }

    private void BulletSpawn360(int count, bool isWhite)
    {
        float angleStep = 360f / count; // count = 소환할 탄의 갯수
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
            if (isWhite)
            {
                GameObject obj = Instantiate(bullet[0], shotPos.position, Quaternion.identity);
                obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.Red, projectDir, 5, 30, 10);
            }
            else
            {
                GameObject obj = Instantiate(bullet[1], shotPos.position, Quaternion.identity);
                obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.Blue, projectDir, 5, 30, 10);
            }

            anlge += angleStep;
        }
    }

    private IEnumerator ExtraAOEAttack(Transform[] attackPos, GameObject attackObj, float attackDelay)
    {
        Vector3[] exPos = new Vector3[attackPos.Length];
        for (int i = 0; i < attackPos.Length; i++)
        {
            exPos[i] = attackPos[i].position;
        }

        for (int i = 0; i < attackPos.Length; i++)
        {
            Instantiate(attackObj, exPos[i], Quaternion.identity);
            yield return new WaitForSeconds(attackDelay);
        }
    }

    private IEnumerator BulletShot(Transform[] attackPos, GameObject attackObj, float attackDelay)
    {
        Transform[] exPos = attackPos;
        int ran = Random.Range(0, bullet.Length);
        for (int i = 0; i < attackPos.Length; i++)
        {
            GameObject obj = Instantiate(attackObj, shotPos.position, Quaternion.identity);
            if (ran == 0)
            {
                obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.Red, (exPos[i].position - transform.position).normalized, 15, 50, 15f);

            }
            else
            {
                obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.Red, (exPos[i].position - transform.position).normalized, 15, 50, 15f);
            }
            yield return new WaitForSeconds(attackDelay);
        }
    }

    protected override void Spawn()
    {
        StartCoroutine(SpawnCall());
    }

    private IEnumerator SpawnCall()
    {
        state = State.Spawn;
        isInvincibility = true;
        Player_Status.instance.canMove = false;

        // Spawn UI
        statusUI_Boss.StartFadeCall();

        // Sound
        sound.SoundPlay_public(Enemy_Sound.PublicSound.Spawn);

        // Animation
        anim.SetTrigger("Spawn");
        anim.SetBool("isSpawn", true);

        // Delay
        while (anim.GetBool("isSpawn"))
        {
            yield return null;
        }

        // UI Wait
        while (statusUI_Boss.isFade)
        {
            yield return null;
        }

        state = State.Idle;
        isInvincibility = false;
        Player_Status.instance.canMove = true;
    }

    protected override void Stagger()
    {
        Debug.Log("Call");
    }

    public override void Die()
    {
        StopAllCoroutines();
        StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        // UI Off
        statusUI_Boss.Die();

        // Sound
        sound.SoundPlay_public(Enemy_Sound.PublicSound.Die);

        // Collider Off
        rollingSlashCollider.SetActive(false);
        backstepAirSlashCollider.SetActive(false);
        foreach (GameObject obj in comboAttackCollider)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in teleportSlashCollider)
        {
            obj.SetActive(false);
        }

        // Animation
        anim.SetTrigger("Die");
        anim.SetBool("isDie", true);
        while (anim.GetBool("isDie"))
        {
            yield return null;
        }

        /*
        // UI Call
        stage_Manager.Stage_Clear();
        while(stage_Manager.isUI)
        {
            yield return null;
        }
        */

        // Delay
        yield return new WaitForSeconds(0.5f);

        // Destroy
        Destroy(container);
    }
}
