using System.Collections;
using UnityEngine;
using Easing;


public class Enemy_Strengthened_Melee : Enemy_Base
{
    [Header("--- Strengthened Melee Setting ---")]
    [SerializeField] private float chaseRange;
    [SerializeField] private float dashPower;
    [SerializeField] private bool isWall;
    [SerializeField] private bool isJump;
    [SerializeField] private Enemy_GroundCheck groundCheck;
    [SerializeField] private LayerMask jumpWallCheck;
    [SerializeField] private AnimationCurve wallJumpCurve;

    [Header("--- Attack Collider ---")]
    [SerializeField] private GameObject normalAttackCollider;
    [SerializeField] private GameObject dashAttackCollider;
    [SerializeField] private GameObject[] bullet;

    [Header("--- Pos Setting ---")]
    [SerializeField] private Transform dashPos;
    [SerializeField] private Transform backDashPos;
    [SerializeField] private Transform shotPos;

    // 점프 기능 관련 고민 결과
    // 점프를 할 부분에 빈 오브젝트 + 스크립트 (콜라이더 + 점프 위치) 배치
    // 몬스터가 이동하는 동안 이동방향으로 레이를 발사하고, 해당 오브젝트가 검출되면 이동 종료 및 점프

    // Sound Index
    // 0 : Normal Attack
    // 1 : Dash
    // 2 : Backstep

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
        Debug.DrawRay(transform.position, new Vector2(transform.localScale.x, 0), Color.red);

        // Find Target & Reset Enemy
        if (!haveTarget)
        {
            Target_Setting();
            if (!haveTarget && state != State.Await)
            {
                state = State.Await;
            }
        }

        WallCheck();
        TimerCheck();
        GroundCheck();

        // Think
        if (state == State.Idle && !isAttack && !isDie && !isAirBorne)
        {
            Think();
        }
    }

    private void Think()
    {
        // Target Check & Attack Setting
        state = State.Think;
        Target_Setting();
        CurTarget_Check();
        LookAt();

        if (targetDir < chaseRange)
        {
            int ran = Random.Range(0, 100);
            if(ran <= 50)
            {
                hitStopCoroutine = StartCoroutine(BackDashShot());
            }
            else if (ran <= 75)
            {
                hitStopCoroutine = StartCoroutine(NormalAttack());
            }
            else
            {
                hitStopCoroutine = StartCoroutine(DashAttack());
            }
        }
        else
        {
            hitStopCoroutine = StartCoroutine(Chase());
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
            rigid.velocity = new Vector2(targetVector.normalized.x * moveSpeed, rigid.velocity.y);
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

        state = State.Move;
        rigid.velocity = Vector2.zero;
        rigid.gravityScale = 0f;
        isJump = true;

        Vector2 startPos = transform.position;
        Vector2 endPos = end;
        //float height = end.y + 0.1f;
        float height = Mathf.Max(1.0f, Mathf.Abs(endPos.y - startPos.y) + 2.0f);
        Debug.Log("Jump StartPos : " + startPos + " / Jump EndPos : " + endPos);

        float timer = 0;
        while (timer < 1)
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

        /*
        // Jump
        Vector2 startPos = transform.position;
        float heightY = endPos.y; // 점프 올라갈 값
        float timer = 0f; // 타이머
        while(timer < 1)
        {
            timer += Time.deltaTime;
            float heightT = wallJumpCurve.Evaluate(timer); // 애니메이션 커브에서 가져올 값
            float height = Mathf.Lerp(0f, heightY, heightT); // 점프(y)에 대한 러프
            transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.InOutQuart(timer)) + new Vector2(0f, height); // 이동값 + 점프값
            yield return null;
        }

        anim.SetFloat("Move", 0);
        rigid.gravityScale = 1f;
        isJump = false;
        isGround = true;
        
        // Delay
        yield return new WaitForSeconds(1f);

        // Chase
        hitStopCoroutine = StartCoroutine(Chase());
        */
    }

    Vector2 CalculateParabolicPosition(Vector2 start, Vector2 end, float height, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        // 두 점 사이의 중간점을 계산하고 높이를 추가합니다.
        Vector2 midpoint = (start + end) / 2;
        midpoint += Vector2.up * height;

        // 베지어 곡선의 계산
        Vector2 position = uu * start;
        position += 2 * u * t * midpoint;
        position += tt * end;

        return position;

        /*
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        // 두 점 사이의 중간점을 계산하고 높이를 추가합니다.
        Vector2 midpoint = (start + end) / 2;
        midpoint += Vector2.up * height;

        // 베지어 곡선의 계산
        Vector2 position = uu * start;
        position += 2 * u * t * midpoint;
        position += tt * end;

        return position;
        */
    }

    private IEnumerator NormalAttack()
    {
        state = State.Attack;
        isAttack = true;

        CurTarget_Check();
        LookAt();

        // Animation
        anim.SetTrigger("Attack");
        anim.SetBool("isNormalAttack", true);
        while (anim.GetBool("isNormalAttack"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
    }

    public void NormalAttackCollider()
    {
        normalAttackCollider.SetActive(normalAttackCollider.activeSelf == true ? false : true);

        // Sound
        if (!normalAttackCollider.activeSelf)
        {
            sound.SoundPlay_Other(0);
        }
    }

    private IEnumerator DashAttack()
    {
        state = State.Attack;
        isAttack = true;

        CurTarget_Check();
        LookAt();

        // Animation
        anim.SetTrigger("Attack");
        anim.SetBool("isDashReady", true);
        anim.SetBool("isDashAttack", true);

        // Animation Wait
        float timer = 0.5f;
        while (timer > 0)
        {
            CurTarget_Check();
            LookAt();
            timer -= Time.deltaTime;
            yield return null;
        }
        anim.SetBool("isDashReady", false);

        // Dash Setting
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (dashPos.position - transform.position).normalized, (dashPos.position - transform.position).magnitude, groundLayer);
        Vector3 startPos = transform.position;
        Vector3 endPos = dashPos.position;
        endPos.y = transform.position.y;

        // Sound
        sound.SoundPlay_Other(1);

        // Dash
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * dashPower;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetBool("isDashAttack", false);
        rigid.velocity = Vector2.zero;

        // Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
    }

    private IEnumerator BackDashShot()
    {
        state = State.Attack;
        isAttack = true;

        CurTarget_Check();
        LookAt();

        // Animation
        anim.SetTrigger("Attack");
        anim.SetBool("isBackDashReady", true);
        anim.SetBool("isBackDashShot", true);

        // Animation wait
        float timer = 0;
        while(timer < 0.5f)
        {
            CurTarget_Check();
            LookAt();
            timer += Time.deltaTime;
            yield return null;
        }
        anim.SetBool("isBackDashReady", false);

        // Sound
        sound.SoundPlay_Other(1);

        // Move
        Vector3 startPos = transform.position;
        Vector3 endPos = backDashPos.position;
        endPos.y = transform.position.y;
        timer = 0;
        while(timer < 1 && !isWall && groundCheck.isGround)
        {
            timer += Time.deltaTime * dashPower;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }


        // Animation Wait
        while (anim.GetBool("isBackDashShot"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
    }

    public void BackDashShooting()
    {
        // Sound
        sound.SoundPlay_Other(2);

        // Shot Attack
        Vector2 shotDir = (curTarget.transform.position - transform.position).normalized;
        GameObject obj = Instantiate(bullet[Random.Range(0, bullet.Length)], shotPos.position, Quaternion.identity);
        obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.None, shotDir, 15, 30, 30);
    }

    private void WallCheck()
    {
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(transform.localScale.x, transform.position.y + 1f), aiCollider.bounds.size.x + 0.5f, jumpWallCheck);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(transform.localScale.x, 0), aiCollider.bounds.size.x + 0.5f, jumpWallCheck);
        if(hit.collider != null )
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
        StopAllCoroutines();
        StartCoroutine(SpawnCall());
    }

    private IEnumerator SpawnCall()
    {
        state = State.Spawn;
        statusUI_Normal.Status_Setting();

        // Animation
        anim.SetTrigger("Spawn");
        anim.SetBool("isSpawn", true);

        // Animation Wait
        while (anim.GetBool("isSpawn"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.25f);

        state = State.Idle;
    }

    protected override void Stagger()
    {
        // Animation Reset
        for (int i = 0; i < animationTrigger.Length; i++)
        {
            anim.ResetTrigger(animationTrigger[i]);
        }
        for (int i = 0; i < animationbool.Length; i++)
        {
            anim.SetBool(animationbool[i], false);
        }
        anim.SetFloat("Move", 0);
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

        // Animation Wait
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
