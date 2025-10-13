using System.Collections;
using UnityEngine;


public class Enemy_Strengthened_Range : Enemy_Base
{
    [Header("--- Strengthened Range Setting ---")]
    [SerializeField] private float chaseRange;
    [SerializeField] private float dashPower;
    [SerializeField] private bool isWall;
    [SerializeField] private bool isJump;
    [SerializeField] private Enemy_GroundCheck groundCheck;
    [SerializeField] private LayerMask jumpWallCheck;
    [SerializeField] private AnimationCurve wallJumpCurve;
    [SerializeField] private LineRenderer line;

    [Header("--- Prefabs ---")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject spawnVFX;

    [Header("--- Pos Setting ---")]
    [SerializeField] private Transform shotPos;

    [Header("--- Attack Collider ---")]
    public GameObject normalAttackCollider;

    // Sound Index
    // 0 : Normal Attack
    // 1 : ContinuousShot

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
        if (!haveTarget)
        {
            Target_Setting();
            if (!haveTarget && state != State.Await)
            {
                state = State.Await;
            }
        }

        // LR Wall Check
        WallCheck();
        TimerCheck();
        GroundCheck();

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

        // Target Check & Attack Setting
        Target_Setting();
        CurTarget_Check();
        LookAt();

        if (targetDir < chaseRange)
        {
            // Attack Setting
            int ran = Random.Range(0, 100);
            if (targetDir <= 1.5f)
            {
                // Melee Attack
                hitStopCoroutine = StartCoroutine(NormalAttack());
            }
            else
            {
                // Ranage Attack
                if (ran <= 50)
                {
                    hitStopCoroutine = StartCoroutine(NormalShot());
                }
                else
                {
                    hitStopCoroutine = StartCoroutine(ContinuousShot());
                }
            }
        }
        else
        {
            // Ground Check + wall Check
            LookAt();
            if (groundCheck.isGround)
            {
                hitStopCoroutine = StartCoroutine(Chase());
            }
            else
            {
                Target_Setting();
                Think();
            }
        }
    }

    private IEnumerator Chase()
    {
        state = State.Move;

        // Chase
        anim.SetFloat("Move", 1);
        while (targetDir >= chaseRange && !isWall)
        {
            CurTarget_Check();
            LookAt();
            if(isGround)
            {
                rigid.velocity = new Vector2(targetVector.normalized.x * moveSpeed, rigid.velocity.y);

            }
            else
            {
                rigid.velocity = new Vector2(targetVector.normalized.x * moveSpeed * 0.15f, rigid.velocity.y);

            }
            yield return null;
        }
        anim.SetFloat("Move", 0);

        // Think
        rigid.velocity = Vector2.zero;
        state = State.Idle;
    }

    private IEnumerator Jump(Vector2 end)
    {
        state = State.Move;
        rigid.velocity = Vector2.zero;
        rigid.gravityScale = 0f;
        isJump = true;
        anim.SetFloat("Move", 0);

        Vector2 startPos = transform.position;
        Vector2 endPos = end;
        //float height = end.y + 0.1f;
        float height = Mathf.Max(1.0f, Mathf.Abs(endPos.y - startPos.y) + 2.0f);

        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 1.25f;
            Vector2 currentPos = CalculateParabolicPosition(startPos, endPos, height, timer);
            transform.position = currentPos;
            yield return null;
        }

        rigid.gravityScale = 1f;
        isJump = false;
        isGround = true;

        // Delay
        yield return new WaitForSeconds(0.5f);

        // Chase
        hitStopCoroutine = StartCoroutine(Chase());
    }

    Vector2 CalculateParabolicPosition(Vector2 start, Vector2 end, float height, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        // 두 점 사이의 중간점을 계산하고 높이를 추가
        Vector2 midpoint = (start + end) / 2;
        midpoint += Vector2.up * height;

        // 베지어 곡선의 계산
        Vector2 position = uu * start;
        position += 2 * u * t * midpoint;
        position += tt * end;

        return position;
    }

    private IEnumerator NormalAttack()
    {
        state = State.Attack;
        isAttack = true;
        LookAt();

        // Animation
        anim.SetTrigger("Attack");
        anim.SetBool("isNormalAttack", true);
        while (anim.GetBool("isNoramlAttack"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
    }

    private IEnumerator NormalShot()
    {
        state = State.Attack;
        isAttack = true;
        LookAt();
        line.enabled = true;
        line.SetPosition(0, shotPos.position);

        // Animation + Aiming
        anim.SetTrigger("Attack");
        anim.SetBool("isShotReady", true);
        anim.SetBool("isNormalShotAttack", true);

        float timer = 0.3f;
        while (timer > 0)
        {
            LookAt();
            line.SetPosition(1, curTarget.transform.position);
            timer -= Time.deltaTime;
            yield return null;
        }
        anim.SetBool("isShotReady", false);
        line.enabled = false;

        // Animation Wait
        while (anim.GetBool("isNormalShotAttack"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
    }

    public void NormalShotCall()
    {
        // Sound
        sound.SoundPlay_Other(0);

        // Attack
        Vector2 shotDir = (curTarget.transform.position - shotPos.position).normalized;
        GameObject obj = Instantiate(bullet, shotPos.position, Quaternion.identity);
        obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.None, shotDir, 10f, 20f, 10f);
    }

    private IEnumerator ContinuousShot()
    {
        state = State.Attack;
        isAttack = true;

        CurTarget_Check();
        LookAt();

        // Animation
        anim.SetTrigger("Attack");
        anim.SetBool("ContinuousReady", true);
        anim.SetBool("isContinuousShot", true);

        // Aiming
        line.enabled = true;
        line.SetPosition(0, shotPos.position);
        float timer = 1f;
        while (timer > 0)
        {
            CurTarget_Check();
            LookAt();
            line.SetPosition(1, curTarget.transform.position);
            timer -= Time.deltaTime;
            yield return null;
        }
        Vector2 shotDir = (curTarget.transform.position - transform.position).normalized;
        line.enabled = false;
        anim.SetBool("ContinuousReady", false);

        // Sound
        sound.SoundPlay_Other(1);

        // Attack
        for (int i = 0; i < 5; i++)
        {
            GameObject obj = Instantiate(bullet, shotPos.position, Quaternion.identity);
            obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.None, shotDir, 10f, 20f, 10f);
            yield return new WaitForSeconds(0.15f);
        }
       
        // Animation Wait
        while(anim.GetBool("isContinuousShot"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
    }

    private void WallCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(transform.localScale.x, 0), aiCollider.bounds.size.x + 0.5f, jumpWallCheck);
        if (hit.collider != null)
        {
            isWall = true;
            if (state == State.Move && isWall && !isJump)
            {
                Vector2 endPos = hit.collider.gameObject.GetComponent<Stage_WallSetting>().jumpPos.position;
                hitStopCoroutine = StartCoroutine(Jump(endPos));
            }
        }
        else
        {
            isWall = false;
        }
    }

    protected override void Spawn()
    {
        StartCoroutine(SpawnCall());
    }

    private IEnumerator SpawnCall()
    {
        state = State.Spawn;
        spawnVFX.SetActive(true);
        statusUI_Normal.Status_Setting();
        while (spawnVFX.activeSelf)
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.15f);

        state = State.Idle;
    }

    protected override void Stagger()
    {
        anim.SetFloat("Move", 0);
        for (int i = 0; i < animationTrigger.Length; i++)
        {
            anim.ResetTrigger(animationTrigger[i]);
        }
        for (int i = 0; i < animationbool.Length; i++)
        {
            anim.SetBool(animationbool[i], false);
        }
        normalAttackCollider.SetActive(false);
    }

    public override void Die()
    {
        StopAllCoroutines();
        StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        state = State.Die;
        isDie = true;

        // UI 초기화
        statusUI_Normal.Die();

        // Sound
        sound.SoundPlay_public(Enemy_Sound.PublicSound.Die);

        // Animation
        anim.SetTrigger("Die");
        anim.SetBool("isDie", true);
        while (anim.GetBool("isDie"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(1f);

        // Destroy
        Destroy(gameObject);
    }
}
