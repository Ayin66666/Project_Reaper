using Easing;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Status : MonoBehaviour
{
    public static Player_Status instance;

    public Rigidbody2D rigidBody2D;
    public LayerMask groundCheck;

    [SerializeField] private Player_Animation playerAnim;
    [SerializeField] private Player_Skill playerSkill;
    [SerializeField] private Player_UI playerUI;
    [SerializeField] private Transform knockBack_Pos;
    [SerializeField] private GameObject playerContainer;

    public enum Mycolor { Blue, Red } //플레이어 색깔
    public Mycolor myColor;
    public enum HitColor { None, Blue, Red } //플레이어 피격 여부를 결정할 적 공격의 색깔
    public enum HitType { None, Stagger } // 플레이어 피격 타입

    public GameObject body_Red;
    public GameObject body_Blue;
    public GameObject change_Effect_Red;
    public GameObject change_Effect_Blue;
    public GameObject portrait_Red;
    public GameObject portrait_Blue;
    public GameObject hp_Background_Red;
    public GameObject hp_Background_Blue;
    public SpriteRenderer sprite_Red;
    public SpriteRenderer sprite_Blue;

    public int curHp , maxHp, damage;
    public float getDamaged;
    public float speed, attackSpeed, criticalMultiply;

    public int jumpCount;
    public float jumpPower, dashPower, dashDistance, dashCoolTime, color_Change_Cooltime, attackMovePower;
    public float blue_Skill1_CoolTime, blue_Skill2_CoolTime;
    public float red_Skill1_CoolTime, red_Skill2_CoolTime;
    public float blue_Skill1_CoolTime_Max, blue_Skill2_CoolTime_Max;
    public float red_Skill1_CoolTime_Max, red_Skill2_CoolTime_Max;

    public bool canMove, canJump, canDash, can_ColorChange;
    public bool isJump, isDash, isSkill, isWall, isGround, isSide, isHit, isDie;
    public bool isGroundAttack, isAirAttack;
    public bool canAttack, canNextAttack;
    public bool can_Blue_Skill1, can_Blue_Skill2;
    public bool can_Red_Skill1, can_Red_Skill2;
    public bool canChase;
    public bool isInvincible;

    private Coroutine hitEffectCoroutine;
    private Coroutine hitMoveCoroutine;
    private Coroutine hitCalcCoroutine;
    private Coroutine myCoroutine;

    public Action action;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        rigidBody2D = GetComponent<Rigidbody2D>();
        playerSkill = GetComponent<Player_Skill>();
        playerAnim.anim = GetComponentInChildren<Animator>();
        sprite_Red = body_Red.GetComponent<SpriteRenderer>();
        sprite_Blue = body_Blue.GetComponent<SpriteRenderer>();

        myColor = Mycolor.Red;

        maxHp = 1000;
        curHp = 1000;
        getDamaged = curHp;

        canMove = true;
        canDash = true;
        canJump = true;
        can_ColorChange = true;
        canAttack = true;
        canNextAttack = true;
        can_Blue_Skill1 = true;
        can_Blue_Skill2 = true;
        can_Red_Skill1 = true;
        can_Red_Skill2 = true;
        canChase = false;

        //스킬 쿨타임
        blue_Skill1_CoolTime_Max = 3f;
        blue_Skill2_CoolTime_Max = 5f;
        red_Skill1_CoolTime_Max = 3f;
        red_Skill2_CoolTime_Max = 5f;
    }

    void Update()
    {
        StatRecovery();
        ChangeColor();
        CoolTime();

        if(isSkill)
        {
            canMove = false;
        }

        if(isDie)
        {
            canMove = false;
            canDash = false;
            canJump = false;
            canAttack = false;
            canNextAttack = false;
            isInvincible = true;
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            TakeDamage(50, 1, false, HitColor.None, HitType.None);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(200, 1, false, HitColor.None, HitType.Stagger);
        }
        if (Input.GetKey(KeyCode.M))
        {
            if (myCoroutine != null)
            {
                StopCoroutine(myCoroutine);
            }
            myCoroutine = StartCoroutine(ChaseTimer());
        }
    }
    public void StatRecovery()
    {
        if (curHp > maxHp) //현재 체력이 최대 체력 초과시 최대 체력으로 고정
        {
            curHp = maxHp;
            getDamaged = (float)maxHp;
        }
    }

    public void ChangeColor()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) && !isSkill && !isHit && can_ColorChange)
        {
            myColor = myColor == Mycolor.Blue ? Mycolor.Red : Mycolor.Blue;
            Player_Sound.instance.SFXPlay(Player_Sound.instance.utility_Sound[3]);
            color_Change_Cooltime = 1f;

            if (myColor == Mycolor.Blue)
            {
                change_Effect_Blue.transform.position = transform.position;
                change_Effect_Blue.SetActive(true);
                body_Red.SetActive(false);
                body_Blue.SetActive(true);
                hp_Background_Blue.SetActive(true);
                hp_Background_Red.SetActive(false);

                portrait_Red.SetActive(false);
                portrait_Blue.SetActive(true);
                playerAnim.anim = GetComponentInChildren<Animator>();
            }
            else
            {
                change_Effect_Red.transform.position = transform.position;
                change_Effect_Red.SetActive(true);
                body_Red.SetActive(true);
                body_Blue.SetActive(false);
                hp_Background_Blue.SetActive(false);
                hp_Background_Red.SetActive(true);

                portrait_Red.SetActive(true);
                portrait_Blue.SetActive(false);
                playerAnim.anim = GetComponentInChildren<Animator>();
            }
        }
    }
    public void GravityCheck(bool On)
    {
        rigidBody2D.gravityScale = On ? 0 : 4;
    }

    public void TakeDamage(int damage, int hitCount, bool isCritical, HitColor hitColor, HitType hitType)
    {
        if (isInvincible)
        {
            return;
        }

        isHit = true;
        Player_Sound.instance.SFXPlay(Player_Sound.instance.utility_Sound[0]);

        switch (hitColor)
        {
            case HitColor.None:
                break;
            case HitColor.Blue:
                if(myColor == Mycolor.Blue)
                    return;
                break;
            case HitColor.Red:
                if (myColor == Mycolor.Red)
                    return;
                break;
        }

        switch (hitType)
        {
            case HitType.None:
                //플레이어 잠시 무적 및 반짝거림, 행동 안 끊김, 슬로우 X, 흔들림 X, 화면 이펙트만
                Debug.Log("흔들림");
                Effect_Manager.instance.VirtualCamera_Shake(0.2f);
                break;
            case HitType.Stagger:
                //플레이어 잠시 무적 및 반짝거림, 행동 끊김, 슬로우 O, 흔들림 O, 화면 이펙트 및 피격 이펙트
                //StopAllCoroutines();
                //playerSkill.StopAllCoroutine();
                Debug.Log("흔들림");

                Effect_Manager.instance.VirtualCamera_Shake(1f);

                playerAnim.anim.SetBool("isHit", true);
                playerAnim.AllOff_Attack_Collider();

                hitMoveCoroutine = StartCoroutine(StaggerMove());
                break;
        }

        if(hitCalcCoroutine != null)
        {
            StopCoroutine(hitCalcCoroutine);
        }
        hitCalcCoroutine = StartCoroutine(HitCalculate(damage, hitCount, isCritical));
    }

    void Die()
    {
        Debug.Log("사망");
        isDie = true;
        canMove = false;
        canDash = false;
        canJump = false;
        canAttack = false;
        canNextAttack = false;
        isInvincible = true;

        //애니메이션
        playerUI.DieCall();
    }

    IEnumerator StaggerMove()
    {
        float timer = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = knockBack_Pos.position;

        canMove = false;
        canDash = false;
        canJump = false;
        canAttack = false;

        while (timer < 1)
        {
            timer += Time.deltaTime * 2f;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));

            yield return null;
        }
        Debug.Log("경직풀림");

        playerAnim.anim.SetBool("isHit", false);
        isHit = false;

        if (curHp > 0)
        {
            canMove = true;
            canDash = true;
            canJump = true;
            canAttack = true;
        }
    }

    IEnumerator HitCalculate(int damage, int hitCount, bool isCritical)
    {
        if (hitEffectCoroutine != null)
        {
            StopCoroutine(hitEffectCoroutine);
        }
        hitEffectCoroutine = StartCoroutine(HitEffect());

        for (int i = 0; i < hitCount; i++)
        {
            curHp -= (int)(damage / hitCount);
            StartCoroutine(SetGetdamaged());

            if (curHp <= 0)
            {
                //애니메이션

                Die();
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator HitEffect()
    {
        Debug.Log("확인");
        isInvincible = true;

        sprite_Red.color = new Color(1, 1, 1, 0.5f);
        sprite_Blue.color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(0.1f);
        sprite_Red.color = new Color(1, 1, 1, 1);
        sprite_Blue.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(0.1f);
        sprite_Red.color = new Color(1, 1, 1, 0.5f);
        sprite_Blue.color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(0.1f);
        sprite_Red.color = new Color(1, 1, 1, 1);
        sprite_Blue.color = new Color(1, 1, 1, 1);

        if (curHp > 0)
        {
            isInvincible = false;
            isHit = false;
        }
    }

    public void CoolTime()
    {
        //대쉬 쿨타임
        dashCoolTime -= (dashCoolTime > 0) ? Time.deltaTime : 0;
        color_Change_Cooltime -= (color_Change_Cooltime > 0) ? Time.deltaTime : 0;

        if (!isSkill)
        {
            canDash = (dashCoolTime > 0 || curHp <= 0) ? false : true;
        }

        can_ColorChange = (color_Change_Cooltime <= 0) ? true : false;

        //스킬 쿨타임
        blue_Skill1_CoolTime -= (blue_Skill1_CoolTime > 0) ? Time.deltaTime : 0;
        blue_Skill2_CoolTime -= (blue_Skill2_CoolTime > 0) ? Time.deltaTime : 0;

        red_Skill1_CoolTime -= (red_Skill1_CoolTime > 0) ? Time.deltaTime : 0;
        red_Skill2_CoolTime -= (red_Skill2_CoolTime > 0) ? Time.deltaTime : 0;

        can_Blue_Skill1 = (blue_Skill1_CoolTime <= 0) ? true : false;
        can_Blue_Skill2 = (blue_Skill2_CoolTime <= 0) ? true : false;
        can_Red_Skill1 = (red_Skill1_CoolTime <= 0) ? true : false;
        can_Red_Skill2 = (red_Skill2_CoolTime <= 0) ? true : false;
    }

    public IEnumerator ChaseTimer()
    {
        canChase = true;
        float timer = 1;
        Debug.Log("테스트1");
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            Debug.Log(timer);
            yield return null;
        }

        canChase = false;
        Debug.Log("테스트2");
    }

    public Vector2 BezierTesting(Vector2 P_1, Vector2 P_2, Vector2 P_3, float Value)
    {
        Vector2 A = Vector2.Lerp(P_1, P_2, Value);
        Vector2 B = Vector2.Lerp(P_2, P_3, Value);

        Vector2 C = Vector2.Lerp(A, B, Value);


        return C;
    }

    IEnumerator SetGetdamaged() // 닼소처럼 hp바 구현
    {
        yield return new WaitForSeconds(0.5f);
        getDamaged = curHp;
    }

    public void SetVelocity()
    {
        rigidBody2D.velocity = Vector2.zero;
    }

    public static void SetPos(Vector3 pos)
    {
        
    }

    public void CheckPlatform()
    {
        gameObject.transform.SetParent(playerContainer.transform);
    }
}
