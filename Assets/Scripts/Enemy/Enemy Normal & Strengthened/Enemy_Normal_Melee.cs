using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Easing;
using UnityEngine.UI;

public class Enemy_Normal_Melee : Enemy_Base
{
    [Header("--- Normal Melee Setting ---")]
    [SerializeField] private float chaseRange;
    [SerializeField] private float dashPower;
    [SerializeField] private bool isWall;
    [SerializeField] private Enemy_GroundCheck groundCheck;

    [Header("--- Attack Collider ---")]
    [SerializeField] private GameObject normalAttackCollider;
    [SerializeField] private GameObject dashAttackCollider;

    [Header("--- Pos Setting ---")]
    [SerializeField] private Transform dashPos;

    // Sound Index
    // 0 : 근접공격
    // 1 : 대쉬 공격

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

        WallCheck();
        TimerCheck();
        GroundCheck();

        // Test
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(DashAttack());
        }

        // Think
        if (state == State.Idle && !isAttack && !isDie && !isAirBorne)
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
            int ran = Random.Range(0, 100);
            if(ran <= 65)
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
        Debug.Log("Call chase");
        state = State.Move;
 
        // Chase
        anim.SetFloat("Move", 1);
        while(targetDir >= chaseRange && groundCheck.isGround && !isWall)
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

    private IEnumerator NormalAttack()
    {
        state = State.Attack;
        isAttack = true;
        LookAt();

        // Animation
        anim.SetTrigger("Attack");
        anim.SetBool("isNormalAttack", true);
        while(anim.GetBool("isNormalAttack"))
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

        // Attack Sound
        if(!normalAttackCollider.activeSelf)
        {
            sound.SoundPlay_Other(0);
        }
    }

    private IEnumerator DashAttack()
    {
        Debug.Log("Rush");

        state = State.Attack;
        isAttack = true;

        // Animation
        anim.SetTrigger("Attack");
        anim.SetBool("isDashReady", true);
        anim.SetBool("isDashAttack", true);

        // Animation Wait
        float timer = 0.5f;
        while(timer > 0)
        {
            LookAt();
            timer -= Time.deltaTime;
            yield return null;
        }
        anim.SetBool("isDashReady", false);

        // Dash Setting
        Vector3 startPos = transform.position;
        Vector3 endPos = dashPos.position;
        endPos.y = transform.position.y;

        // Attack Sound
        sound.SoundPlay_Other(1);

        // Dash
        timer = 0;
        while(timer < 1 && !isWall)
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

    private void WallCheck()
    {
        isWall = Physics2D.Raycast(transform.position, new Vector2(transform.localScale.x, 0), 1f, groundLayer);
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
        while(anim.GetBool("isSpawn"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.25f);

        state = State.Idle;
    }

    public override void Die()
    {
        StopAllCoroutines();
        StartCoroutine(DieCall());
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

    private IEnumerator DieCall()
    {
        state = State.Die;
        isDie = true;

        // Sound
        sound.SoundPlay_public(Enemy_Sound.PublicSound.Die);

        // Animation
        anim.SetTrigger("Die");
        anim.SetBool("isDie", true);

        // Animation Wait
        while(anim.GetBool("isDie"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.25f);

        // Destroy
        Destroy(gameObject);
    }
}
