using System.Collections;
using UnityEngine;
using Easing;


public class Enemy_Normal_Range : Enemy_Base
{
    [Header("--- Normal Range Setting ---")]
    private Vector2 shotDir;
    [SerializeField] private float chaseRange;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float dashPower;
    [SerializeField] private bool isWall;
    [SerializeField] private Enemy_GroundCheck groundCheck;
    [SerializeField] private LineRenderer line;

    [Header("--- Prefabs ---")]
    [SerializeField] private GameObject bullet;

    [Header("--- Pos Setting ---")]
    [SerializeField] private Transform shotPos;
    [SerializeField] private Transform dashPos;

    [Header("--- Attack Collider ---")]
    [SerializeField] private GameObject dashAttackCollider;
    [SerializeField] private GameObject dashAttackEffect;

    // Sound Index
    // 0 : Dash
    // 1 : Shot

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

        if(state == State.Groggy && line.enabled == true)
        {
            line.enabled = false;
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
        LookAt();

        if (targetDir < chaseRange)
        {
            // Attack Setting
            if (hitStopCoroutine != null) StopCoroutine(hitStopCoroutine);
            hitStopCoroutine = StartCoroutine(targetDir <= 1.5f ? DashAttack() : NormalShot());
        }
        else
        {
            // Ground Check + wall Check
            if (hitStopCoroutine != null) StopCoroutine(hitStopCoroutine);
            hitStopCoroutine = StartCoroutine(Chase());
        }
    }

    private IEnumerator Chase()
    {
        Debug.Log("Call chase");
        state = State.Move;

        // Chase
        anim.SetFloat("Move", 1);
        while (targetDir >= chaseRange && groundCheck.isGround && !isWall)
        {
            CurTarget_Check();
            LookAt();
            rigid.velocity = targetVector.normalized * moveSpeed;
            yield return null;
        }
        anim.SetFloat("Move", 0);

        // Think
        rigid.velocity = Vector2.zero;
        state = State.Idle;
    }

    private IEnumerator DashAttack()
    {
        state = State.Attack;
        isAttack = true;
        LookAt();

        // Animation
        anim.SetTrigger("Attack");
        anim.SetBool("isDashReady", true);
        anim.SetBool("isDashAttack", true);

        // Animation Wait
        float timer = 0.5f;
        while (timer > 0)
        {
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

        // Look At
        if (targetVector.x > 0)
        {
            curLook = CurLook.Left;
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
        else if (targetVector.x < 0)
        {
            curLook = CurLook.Right;
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }

        // Sound
        sound.SoundPlay_Other(0);

        // Dash Move
        dashAttackCollider.SetActive(true);
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * dashPower;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetBool("isDashAttack", false);
        dashAttackCollider.SetActive(false);
        rigid.velocity = Vector2.zero;


        // Move Effect + Wait
        dashAttackEffect.SetActive(true);
        while (dashAttackEffect.activeSelf)
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

        // Animation
        anim.SetTrigger("Attack");
        anim.SetBool("isShotReady", true);
        anim.SetBool("isNormalShot", true);

        // Aiming
        line.enabled = true;
        line.SetPosition(0, shotPos.position);
        float timer = 0;
        while (timer < 0.5f)
        {
            LookAt();
            line.SetPosition(1, curTarget.transform.position);
            shotDir = (curTarget.transform.position - shotPos.position).normalized;
            timer += Time.deltaTime;
            yield return null;
        }
        line.enabled = false;
        anim.SetBool("isShotReady", false);

        // Shot Delay
        yield return new WaitForSeconds(0.1f);

        // Sound
        sound.SoundPlay_Other(1);

        // Attack
        GameObject obj = Instantiate(bullet, shotPos.position, Quaternion.identity);
        obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.None, shotDir, bulletSpeed, bulletSpeed * 2f, 15f);

        // Animation Wait
        while (anim.GetBool("isNormalShot"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
    }

    private IEnumerator HowitzerShot()
    {
        // 작업중
        state = State.Attack;
        isAttack = true;
        LookAt();

        // Animation
        anim.SetTrigger("Attack");
        anim.SetBool("isShotReady", true);
        anim.SetBool("isHowitzerShot", true);
        float timer = 0;
        while (timer < 0.5f)
        {
            LookAt();
            timer += Time.deltaTime;
            yield return null;
        }
        anim.SetBool("isShotReady", false);

        // Attack
        Vector2 startPos = shotPos.position;
        Vector2 endPos = curTarget.transform.position;
        GameObject obj = Instantiate(bullet, shotPos.position, Quaternion.identity);
        timer = 0;
        while (timer < 1 && obj != null)
        {
            timer += Time.deltaTime * 1.5f;
            obj.transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.InOutCubic(timer));
            yield return null;
        }

        // Animation Wait
        while (anim.GetBool("isHowitzerShot"))
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
        isWall = Physics2D.Raycast(transform.position, new Vector2(transform.localScale.x, 0), 1f, groundLayer);
    }

    protected override void Spawn()
    {
        StartCoroutine(SpawnCall());
    }

    private IEnumerator SpawnCall()
    {
        state = State.Spawn;
        statusUI_Normal.Status_Setting();

        // Animation
        anim.SetTrigger("Spawn");
        anim.SetBool("isSpawn", true);
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
        dashAttackEffect.SetActive(false);
        line.enabled = false;

        // Animation Reset
        anim.SetFloat("Move", 0);
        for (int i = 0; i < animationTrigger.Length; i++)
        {
            anim.ResetTrigger(animationTrigger[i]);
        }
        for (int i = 0; i < animationbool.Length; i++)
        {
            anim.SetBool(animationbool[i], false);
        }
    }

    public override void Die()
    {
        if (hitStopCoroutine != null) StopCoroutine(hitStopCoroutine);
        if (hitAirborneCoroutine != null) StopCoroutine(hitAirborneCoroutine);
        if (hitKnockbackCoroutine != null) StopCoroutine(hitKnockbackCoroutine);
        if (hitDownAttackCoroutine != null) StopCoroutine(hitDownAttackCoroutine);

        StopAllCoroutines();
        StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        state = State.Die;
        isDie = true;
        line.enabled = false;

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
