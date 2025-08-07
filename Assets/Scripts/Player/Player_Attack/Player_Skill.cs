using Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Skill : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private BoxCollider2D boxCollider;

    [SerializeField] private Player_Move playerMove;
    [SerializeField] private Player_Animation playerAnim;
    [SerializeField] private Player_AttackCollision playerAttackCollision;

    [Header("---Collider---")]
    public GameObject blue_Skill1_Collider;
    public GameObject[] blue_Skill2_Collider;
    public GameObject red_Skill1_Collider;
    public GameObject[] red_Skill2_Collider;

    [Header("---Effect---")]
    [SerializeField] private GameObject blue_Skill1_Effect;
    [SerializeField] private GameObject blue_Skill2_1_Effect;
    [SerializeField] private GameObject blue_Skill2_2_Effect;
    [SerializeField] private GameObject blue_Skill2_Loop_Effect;

    [Header("---Pos---")]
    [SerializeField] private Transform blue_Skill2_2_StartPos;
    [SerializeField] private Transform blue_Skill2_2_EndPos;
    [SerializeField] private Transform blue_Skill2_1_EffectPos;
    [SerializeField] private Transform blue_Skill2_2_EffectPos;
    [SerializeField] private Transform blue_Skill2_Loop_EffectPos;

    [Header("---Skill Count---")]
    public int red_Skill1_HitCount;
    public float holdTimer;

    [SerializeField] private float skillDashPower;
    private RaycastHit2D blue_Skill2_2_WallCheck;

    private Coroutine myCoroutine;

    void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        red_Skill1_HitCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        UseSkill();

    }

    public void StopAllCoroutine()
    {
        StopAllCoroutines();
    }

    void UseSkill()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if ((Player_Status.instance.myColor == Player_Status.Mycolor.Blue) && Player_Status.instance.can_Blue_Skill1 && !Player_Status.instance.isGround)
            {
                if (myCoroutine != null)
                {
                    StopCoroutine(myCoroutine);
                }
                myCoroutine = StartCoroutine(Blue_Skill1());
            }
            else if ((Player_Status.instance.myColor == Player_Status.Mycolor.Blue) && !Player_Status.instance.can_Blue_Skill1 && !Player_Status.instance.isGround)
            {
                Debug.Log("쿨타임!");
            }
            else if ((Player_Status.instance.myColor == Player_Status.Mycolor.Red) && Player_Status.instance.can_Red_Skill1 && Player_Status.instance.isGround)
            {
                Debug.Log("블랙 스킬1");
                if (myCoroutine != null)
                {
                    StopCoroutine(myCoroutine);
                }
                StartCoroutine(nameof(Red_Skill1));
            }
            else if ((Player_Status.instance.myColor == Player_Status.Mycolor.Red) && !Player_Status.instance.can_Red_Skill1 && Player_Status.instance.isGround)
            {
                Debug.Log("쿨타임!");
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if ((Player_Status.instance.myColor == Player_Status.Mycolor.Blue) && Player_Status.instance.can_Blue_Skill2 && !Player_Status.instance.isGround)
            {
                Debug.Log("화이트 스킬2");
                if(myCoroutine != null)
                {
                    StopCoroutine(myCoroutine);
                }
                myCoroutine = StartCoroutine(Blue_Skill2());
            }
            else if ((Player_Status.instance.myColor == Player_Status.Mycolor.Blue) && !Player_Status.instance.can_Blue_Skill2 && !Player_Status.instance.isGround)
            {
                Debug.Log("쿨타임!");
            }
            else if ((Player_Status.instance.myColor == Player_Status.Mycolor.Red) && Player_Status.instance.can_Red_Skill2 && Player_Status.instance.isGround)
            {
                Debug.Log("블랙 스킬2");
                if (myCoroutine != null)
                {
                    StopCoroutine(myCoroutine);
                }
                StartCoroutine(nameof(Red_Skill2));
            }
            else if ((Player_Status.instance.myColor == Player_Status.Mycolor.Red) && !Player_Status.instance.can_Red_Skill2 && Player_Status.instance.isGround)
            {
                Debug.Log("쿨타임!");
            }
        }
    }

    IEnumerator Blue_Skill1()
    {
        Player_Status.instance.isInvincible = true;
        Player_Status.instance.canMove = false;
        Player_Status.instance.canDash = false;
        Player_Status.instance.canJump = false;
        Player_Status.instance.canAttack = false;
        Player_Status.instance.isDash = false;
        Player_Status.instance.isSkill = true;
        Player_Status.instance.isGroundAttack = false;
        Player_Status.instance.isAirAttack = false;

        Player_Status.instance.blue_Skill1_CoolTime = 3f;
        rigidBody2D.velocity = Vector2.zero;
        Player_Status.instance.GravityCheck(true);

        //애니메이션
        playerAnim.anim.SetTrigger("Blue_Skill1");
        playerAnim.anim.SetBool("isSkill", true);
        playerAnim.anim.SetBool("Skill_Ready", true);

        float timer = 0.5f;
        skillDashPower = 0;

        Vector3 moveDir = transform.localScale.x == 1 ? new Vector3(6f, 0) : new Vector3(-6f, 0);
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + moveDir;

        while (playerAnim.anim.GetBool("Skill_Ready"))
        {
            yield return null;
        }

        blue_Skill1_Effect.transform.position = blue_Skill2_1_EffectPos.position;
        blue_Skill1_Effect.SetActive(true);
        Player_Sound.instance.SFXPlay(Player_Sound.instance.blue_Skill_Sound[0]);

        while (timer > 0 && !Player_Status.instance.isWall)
        {
            timer -= Time.deltaTime;
            skillDashPower += 2f * Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(skillDashPower));

            yield return null;
        }

        Player_Status.instance.isInvincible = false;

        StartCoroutine(nameof(AirHolding));
    }


    IEnumerator Blue_Skill2()
    {
        Player_Status.instance.isInvincible = true;
        Player_Status.instance.canMove = false;
        Player_Status.instance.canDash = false;
        Player_Status.instance.canJump = false;
        Player_Status.instance.canAttack = false;

        Player_Status.instance.canChase = false;

        Player_Status.instance.isDash = false;
        Player_Status.instance.isSkill = true;
        Player_Status.instance.isGroundAttack = false;
        Player_Status.instance.isAirAttack = false;

        Player_Status.instance.blue_Skill2_CoolTime = 5f;
        
        rigidBody2D.velocity = Vector2.zero;
        Player_Status.instance.GravityCheck(true);

        playerAnim.anim.SetTrigger("Blue_Skill2-1");
        playerAnim.anim.SetBool("isSkill", true);
        playerAnim.anim.SetBool("Skill_Ready", true);

        float timer = 0.5f;
        skillDashPower = 0;

        Vector3 moveDir = transform.localScale.x == 1 ? new Vector3(7.5f, 0) : new Vector3(-7.5f, 0);
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + moveDir;
        while (playerAnim.anim.GetBool("Skill_Ready"))
        {
            yield return null;
        }

        blue_Skill2_1_Effect.transform.position = blue_Skill2_1_EffectPos.position;
        blue_Skill2_1_Effect.SetActive(true);
        Player_Sound.instance.SFXPlay(Player_Sound.instance.blue_Skill_Sound[0]);

        while (timer > 0 && !Player_Status.instance.isWall)
        {
            timer -= Time.deltaTime;
            skillDashPower += 2f * Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(skillDashPower));

            yield return null;
        }

        holdTimer = 1f;
        playerAttackCollision.hitCount = 3;

        playerAnim.anim.SetBool("Holding", true);
        //playerAnim.Body_Off(); // 컬러 적용 안됨 이슈
        blue_Skill2_1_Effect.SetActive(false);
        blue_Skill2_Loop_Effect.transform.position = blue_Skill2_Loop_EffectPos.position;
        blue_Skill2_Loop_Effect.SetActive(true);

        while (holdTimer > 0 && Input.GetKey(KeyCode.S))
        {
            holdTimer -= Time.deltaTime;
            yield return null;
        }

        playerAnim.anim.SetBool("Holding", false);
        playerAttackCollision.hitCount = 1;

        //콜라이더 및 이펙트 생성
        blue_Skill2_Loop_Effect.SetActive(false);

        //애니메이션
        playerAnim.anim.SetTrigger("Blue_Skill2-2");
        blue_Skill2_2_Effect.transform.position = blue_Skill2_2_EffectPos.position;
        transform.localScale = new Vector3((transform.localScale.x * -1), 1, 1);
        playerAnim.anim.SetBool("Skill_Ready", true);

        while (playerAnim.anim.GetBool("Skill_Ready"))
        {
            yield return null;
        }

        //blue_Skill2_2_Effect.transform.rotation = Quaternion.Euler(0, 0, -90f);
        blue_Skill2_2_Effect.SetActive(true);

        timer = 0.3f;
        skillDashPower = 0;
        blue_Skill2_2_WallCheck = Physics2D.Raycast(transform.position, Vector2.down, 100f, Player_Status.instance.groundCheck);

        startPos = transform.position;
        //Vector3 endPos2 = blue_Skill2_2_WallCheck.point; //현재는 바닥에 꽂히게 해놓은 상태
        Vector3 endPos2 = transform.position - moveDir;
        Player_Sound.instance.SFXPlay(Player_Sound.instance.blue_Skill_Sound[0]);

        while (timer > 0 && !Player_Status.instance.isWall)
        {
            timer -= Time.deltaTime;
            skillDashPower += 3f * Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos2, EasingFunctions.OutExpo(skillDashPower));

            yield return null;
        }
        Player_Status.instance.isInvincible = false;

        StartCoroutine(nameof(AirHolding));

        if (holdTimer < 0.1f)
        {
            Player_Status.instance.canChase = true;
        }
    }

    IEnumerator Red_Skill1()
    {
        Player_Status.instance.canMove = false;
        Player_Status.instance.canDash = false;
        Player_Status.instance.canJump = false;
        Player_Status.instance.canAttack = false;
        Player_Status.instance.isDash = false;
        Player_Status.instance.isSkill = true;
        Player_Status.instance.isGroundAttack = false;
        Player_Status.instance.isAirAttack = false;

        Player_Status.instance.red_Skill1_CoolTime = 3f;
        rigidBody2D.velocity = Vector2.zero;

        if (red_Skill1_HitCount == 3)
        {
            playerAnim.anim.SetTrigger("Red_Skill1_Enforced");
        }
        else
        {
            playerAnim.anim.SetTrigger("Red_Skill1");
        }

        playerAnim.anim.SetBool("isSkill", true);

        yield return null;
    }

    IEnumerator Red_Skill2()
    {
        Player_Status.instance.canMove = false;
        Player_Status.instance.canDash = false;
        Player_Status.instance.canJump = false;
        Player_Status.instance.canAttack = false;

        Player_Status.instance.canChase = false;

        Player_Status.instance.isDash = false;
        Player_Status.instance.isSkill = true;
        Player_Status.instance.isGroundAttack = false;
        Player_Status.instance.isAirAttack = false;

        Player_Status.instance.red_Skill2_CoolTime = 5f;
        rigidBody2D.velocity = Vector2.zero;

        playerAnim.anim.SetTrigger("Red_Skill2-1");
        playerAnim.anim.SetBool("isSkill", true);

        holdTimer = 2f;

        playerAnim.anim.SetBool("Holding", true);

        while (holdTimer > 0 && Input.GetKey(KeyCode.S))
        {
            holdTimer -= Time.deltaTime;
            yield return null;
        }

        playerAnim.anim.SetBool("Holding", false);
        playerAnim.anim.SetTrigger("Red_Skill2-2");
    }

    IEnumerator AirHolding()
    {
        float inputTimer = 0.5f;
        while (inputTimer > 0 && !Input.anyKey)
        {
            inputTimer -= Time.deltaTime;

            yield return null;
        }
        rigidBody2D.velocity = new Vector2(0, rigidBody2D.velocity.y);

        Player_Status.instance.canDash = true;
        Player_Status.instance.canAttack = true;

        playerAnim.anim.SetBool("isSkill", false);
        playerAnim.FinishedAttack();
    }
}
