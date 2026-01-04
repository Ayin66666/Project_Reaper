using Easing;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Boss_Stage5 : Enemy_Base
{
    [Header("---Stage 5 Boss Setting---")]
    [SerializeField] private Stage_Manager stage_Manager;
    [SerializeField] private GameObject container;
    [SerializeField] private Boss_Stage5_AirOneSlashPos oneSlashPos;
    [SerializeField] private CycleCount cycleCount;
    [SerializeField] private int attackCount;
    private bool isPhase2;
    private bool isCycle;
    private bool isWall;
    private enum CycleCount { Cycle1, Cycle2, Cycle3 }


    [Header("---Effect---")]
    [SerializeField] private GameObject teleportVFX;
    [SerializeField] private GameObject teleportChargeVFX;
    [SerializeField] private GameObject flurryChargeVFX;
    [SerializeField] private GameObject HalfMoonChargeVFX;
    [SerializeField] private GameObject[] horizontalWarringVFX;
    [SerializeField] private GameObject sweepingChargeVFX;
    [SerializeField] private GameObject CenterSlashChargeVFX;
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject swordAuraVFX;

    [Header("---Prefabs---")]
    [SerializeField] private GameObject[] bullet;
    [SerializeField] private Boss_Stage5_CenterSlashA[] centerSlashA;
    [SerializeField] private Boss_Stage5_CenterSlashA[] centerSlashB;
    [SerializeField] private Boss_Stage5_CenterSlashA[] centerSlashC;
    [SerializeField] private GameObject centerExplosion;

    [Header("---Phase 1 Attack Collider---")]
    public GameObject upwardCollider;
    public GameObject[] groundOneCollider;
    public GameObject[] groundFlurryCollider;
    public GameObject[] airOneCollider;
    public GameObject[] airFlurryCollider;
    public GameObject[] airFlurryCollider2;
    public GameObject halfMoonCollider;
    public GameObject[] comboCollider;


    [Header("---Phase 2 Attack Collider---")]
    [SerializeField] private GameObject[] HorizontalCollider;
    [SerializeField] private GameObject[] sweepingCollder;
    [SerializeField] private GameObject centerBCollider;


    [Header("---Move Pos---")]
    [SerializeField] private Transform upwardMovePos;
    [SerializeField] private Transform groundOneMovePos;
    [SerializeField] private Transform groundFlurryMovePos;
    [SerializeField] private Transform[] airFlurryMovePos;
    [SerializeField] private Transform comboMovePos;
    [SerializeField] private Transform returnPos;
    [SerializeField] private Transform[] sweepingMovePos;
    [SerializeField] private Transform auraShootPos;
    [SerializeField] private Collider2D centerMoveCollider;


    [Header("---Explosion Pos---")]
    [SerializeField] private Transform[] centerExplosionPosL;
    [SerializeField] private Transform[] centerExplosionPosR;

    // 1 페이즈 패턴
    // Teleport -> 타켓 플레이어 위치로 순간이동

    // Upward Slash -> 지상 1번 스킬 / 올려베기 / 피격 시 대상 에어본

    // Ground One Slash -> 공중 2번 스킬 재활용 / 지상일섬 / 피격 시 효과 x

    // Ground Flurry -> 지상 2번 스킬 / 지상난무 / 피격 시 경직

    // Air One Slash -> 공중 1번 스킬 / 공중일섬 / 피격 시 경직

    // Air Flurry -> 공중 2번 스킬 / 공중난무 / 피격 시 경직

    // Combo Slash -> 일반 공격 / 콤보 /  피격 시 효과 x

    // Half Moon Slash -> 신규 모션? / 반원베기 / 피격 시 경직


    // 2 페이즈 패턴
    // Horizontal Slash -> 플레이어 암전 / 가로줄 베기 / 피격 시 효과 x

    // Sweeping Slash -> 공중 1번 스킬 재활용 / 범위일섬 / 피격 시 경직

    // Center Slash ->  신규 모션 or 플레이어 암전 / 중앙베기 / 피격 시 효과 x


    // 1 페이즈 사이클
    // 사이클 A -> 순간이동 -> 올려베기 -> 공중일섬 (전 - 순간이동) -> 공중난무

    // 사이클 B -> 지상일섬 -> 올려베기 -> 지상난무

    // 사이클 C -> 순간이동 -> 콤보 -> 순간이동 -> 지상일섬

    // 사이클 D -> 순간이동 -> 올려베기 -> 공중일섬 (전 - 순간이동) -> 반원베기

    // 사이클 E -> 콤보 -> 순간이동 -> 지상일섬 -> 지상난무


    // 페이즈 변환 사이클
    // 스테이지 1 보스 ? -> 스테이지 2 보스 SuperSlash -> 스테이지 3 보스 AOE -> 스테이지 4 보스 ? -> Center Slash / Half Moon Slash


    // 2페이즈 사이클
    // 사이클 A-2 -> 순간이동 -> 올려베기 -> 지상난무 (전 - 순간이동) -> 범위돌진 -> 가로줄 베기

    // 사이클 B-2 -> 순간이동 -> 콤보 -> 지상일섬 -> 순간이동 -> 중앙베기

    // 사이클 C-2 -> 콤보 -> 범위돌진 -> 순간이동 -> 올려베기 -> 가로줄 베기

    // 사이클 D-2 -> 범위돌진 -> 순간이동 -> 올려베기 -> 가로줄 베기 -> 반원베기


    // 공격 카운트가 10회 쌓이면 사이클 종료 후 암전 + 추가 공격
    // 1 페이즈 -> 범위일섬 + 반원베기
    // 2 페이즈 ->  범위일섬 + 중앙베기


    // Sound Index
    // 0 : Tepeport
    // 1 : Uward Slash
    // 2 : Ground One Slash, Air One Slash, Sweeping Slash, Horizontal Slash A, Center Slash A
    // 3 : Ground Flurry A, Air Flurry A
    // 4 : Ground Flurry B
    // 5 : Air Flurry B
    // 6 : Combo Slash A, Combo Slash B
    // 7 : Combo Slash C
    // 8 : Half Moon Slash A
    // 9 : Horizontal Slash B, Center Slash B
    // 10 : Phase 2

    private void Start()
    {
        Status_Setting();
        Target_Setting();
        Spawn();
    }

    private void Update()
    {
        // Spawn & Die Check
        if (state == State.Spawn || state == State.Die)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            StartCoroutine(DieCall());
        }

        WallCheck();
        GroundCheck();

        // Phase 2 Check
        if (!isPhase2 && hp <= 500)
        {
            StartCoroutine(Phase2On());
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
        if(Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(CenterSlash());
        }


        if (state == State.Idle && !isAttack && !isDie && !isCycle)
        {
            if(isPhase2)
            {
                Phase1_Think();
            }
            else
            {
                Phase2_Think();
            }
        }
        else
        {
            return;
        }
    }

    private void Phase1_Think()
    {
        state = State.Think;
        int ran = Random.Range(0, 100);
        switch (cycleCount)
        {
            case CycleCount.Cycle1:
                if (ran <= 33)
                {
                    hitStopCoroutine = StartCoroutine(CycleA());
                }
                else if (ran <= 66)
                {
                    hitStopCoroutine = StartCoroutine(CycleB());
                }
                else
                {
                    hitStopCoroutine = StartCoroutine(CycleC());
                }
                break;

            case CycleCount.Cycle2:
                if (ran <= 33)
                {
                    hitStopCoroutine = StartCoroutine(CycleD());
                }
                else if (ran <= 66)
                {
                    hitStopCoroutine = StartCoroutine(CycleE());
                }
                else
                {
                    hitStopCoroutine = StartCoroutine(CycleA());
                }
                break;

            case CycleCount.Cycle3:
                if (ran <= 33)
                {
                    hitStopCoroutine = StartCoroutine(CycleB());
                }
                else if (ran <= 66)
                {
                    hitStopCoroutine = StartCoroutine(CycleC());
                }
                else
                {
                    hitStopCoroutine = StartCoroutine(CycleD());
                }
                break;
        }

    }

    private void Phase2_Think()
    {
        state = State.Think;
        int ran = Random.Range(0, 100);
        switch (cycleCount)
        {
            case CycleCount.Cycle1:
                if (ran <= 33)
                {
                    hitStopCoroutine = StartCoroutine(CycleA());
                }
                else if (ran <= 66)
                {
                    hitStopCoroutine = StartCoroutine(CycleE());
                }
                else
                {
                    hitStopCoroutine = StartCoroutine(CycleC2());
                }
                break;

            case CycleCount.Cycle2:
                if (ran <= 33)
                {
                    hitStopCoroutine = StartCoroutine(CycleD());
                }
                else if (ran <= 66)
                {
                    hitStopCoroutine = StartCoroutine(CycleB());
                }
                else
                {
                    hitStopCoroutine = StartCoroutine(CycleA2());
                }
                break;

            case CycleCount.Cycle3:
                if (ran <= 33)
                {
                    hitStopCoroutine = StartCoroutine(CycleB2());
                }
                else if (ran <= 66)
                {
                    hitStopCoroutine = StartCoroutine(CycleC());
                }
                else
                {
                    hitStopCoroutine = StartCoroutine(CycleD2());
                }
                break;
        }   
    }

    #region Phase 1 Cycle

    // 사이클 A -> 순간이동 -> 올려베기 -> 공중일섬 (전 - 순간이동) -> 공중난무
    private IEnumerator CycleA()
    {
        isCycle = true;

        StartCoroutine(Teleport());
        while(isAttack)
        {
            yield return null;
        }

        StartCoroutine(UpwardSlash());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(AirOneSlash());
        while (isAttack)
        {
            yield return null;
        }

        hitStopCoroutine = StartCoroutine(AirFlurry());
        while (isAttack)
        {
            yield return null;
        }

        // Additional Attack
        if (attackCount >= 10)
        {
            StartCoroutine(AttackCountSkill());
        }
        else
        {
            // Delay
            yield return new WaitForSeconds(delay);

            state = State.Idle;
            isCycle = false;
        }
    }

    // 사이클 B -> 지상일섬 -> 올려베기 -> 지상난무
    private IEnumerator CycleB()
    {
        isCycle = true;

        StartCoroutine(GroundOneSlash());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(UpwardSlash());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(GroundFlurry());
        while (isAttack)
        {
            yield return null;
        }

        // Additional Attack
        if (attackCount >= 10)
        {
            StartCoroutine(AttackCountSkill());
        }
        else
        {
            // Delay
            yield return new WaitForSeconds(delay);

            state = State.Idle;
            isCycle = false;
        }
    }

    // 사이클 C -> 순간이동 -> 콤보 -> 순간이동 -> 지상일섬
    private IEnumerator CycleC()
    {
        isCycle = true;

        StartCoroutine(Teleport());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(ComboSlash());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(Teleport());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(GroundOneSlash());
        while (isAttack)
        {
            yield return null;
        }

        // Additional Attack
        if (attackCount >= 10)
        {
            StartCoroutine(AttackCountSkill());
        }
        else
        {
            // Delay
            yield return new WaitForSeconds(delay);

            state = State.Idle;
            isCycle = false;
        }
    }

    // 사이클 D -> 순간이동 -> 올려베기 -> 공중일섬 (전 - 순간이동) -> 반원베기
    private IEnumerator CycleD()
    {
        isCycle = true;

        StartCoroutine(Teleport());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(UpwardSlash());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(AirOneSlash());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(HalfMoonSlash());
        while (isAttack)
        {
            yield return null;
        }

        // Additional Attack
        if (attackCount >= 10)
        {
            StartCoroutine(AttackCountSkill());
        }
        else
        {
            // Delay
            yield return new WaitForSeconds(delay);

            state = State.Idle;
            isCycle = false;
        }
    }

    // 사이클 E -> 콤보 -> 순간이동 -> 지상일섬 -> 지상난무
    private IEnumerator CycleE()
    {
        isCycle = true;

        StartCoroutine(ComboSlash());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(Teleport());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(GroundOneSlash());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(GroundFlurry());
        while (isAttack)
        {
            yield return null;
        }

        // Additional Attack
        if (attackCount >= 10)
        {
            StartCoroutine(AttackCountSkill());
        }
        else
        {
            // Delay
            yield return new WaitForSeconds(delay);

            state = State.Idle;
            isCycle = false;
        }
    }
    #endregion


    #region Phase 2 Cycle

    // 사이클 A-2 -> 순간이동 -> 올려베기 -> 지상난무 (전 - 순간이동) -> 범위돌진 -> 가로줄 베기
    private IEnumerator CycleA2()
    {
        isCycle = true;
        StartCoroutine(Teleport());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(UpwardSlash());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(GroundFlurry());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(SweepingSlash());
        while (isAttack)
        {
            yield return null;
        }

        hitStopCoroutine = StartCoroutine(HorizontalSlash());
        while (isAttack)
        {
            yield return null;
        }

        // Additional Attack
        if (attackCount >= 10)
        {
            StartCoroutine(AttackCountSkill());
        }
        else
        {
            // Delay
            yield return new WaitForSeconds(delay);

            state = State.Idle;
            isCycle = false;
        }
    }

    // 사이클 B-2 -> 순간이동 -> 콤보 -> 지상일섬 -> 순간이동 -> 중앙베기
    private IEnumerator CycleB2()
    {
        isCycle = true;

        StartCoroutine(Teleport());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(ComboSlash());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(GroundOneSlash());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(Teleport());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(CenterSlash());
        while (isAttack)
        {
            yield return null;
        }

        // Additional Attack
        if (attackCount >= 10)
        {
            StartCoroutine(AttackCountSkill());
        }
        else
        {
            // Delay
            yield return new WaitForSeconds(delay);

            state = State.Idle;
            isCycle = false;
        }
    }

    // 사이클 C-2 -> 콤보 -> 범위돌진 -> 순간이동 -> 올려베기 -> 가로줄 베기
    private IEnumerator CycleC2()
    {
        isCycle = true;

        StartCoroutine(ComboSlash());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(SweepingSlash());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(Teleport());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(UpwardSlash());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(HorizontalSlash());
        while (isAttack)
        {
            yield return null;
        }

        // Additional Attack
        if (attackCount >= 10)
        {
            StartCoroutine(AttackCountSkill());
        }
        else
        {
            // Delay
            yield return new WaitForSeconds(delay);

            state = State.Idle;
            isCycle = false;
        }
    }

    // 사이클 D-2 -> 범위돌진 -> 순간이동 -> 올려베기 -> 가로줄 베기 -> 반원베기
    private IEnumerator CycleD2()
    {
        isCycle = true;

        StartCoroutine(SweepingSlash());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(Teleport());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(UpwardSlash());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(HorizontalSlash());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(HalfMoonSlash());
        while (isAttack)
        {
            yield return null;
        }

        // Additional Attack
        if (attackCount >= 10)
        {
            StartCoroutine(AttackCountSkill());
        }
        else
        {
            // Delay
            yield return new WaitForSeconds(delay);

            state = State.Idle;
            isCycle = false;
        }
    }
    #endregion


    #region Phase 1 Skill
    private IEnumerator Teleport()
    {
        state = State.Attack;
        isAttack = true;
        attackCount++;

        CurTarget_Check();
        LookAt();

        // Charge Effect + Move Pos Check
        teleportChargeVFX.SetActive(true);
        oneSlashPos.gameObject.SetActive(true);
        while (teleportChargeVFX.activeSelf)
        {
            CurTarget_Check();
            LookAt();
            yield return null;
        }

        TargetLRCheck(curTarget);
        float timer = 0;
        while(timer < (isPhase2 ? 0.25f : 0.5f))
        {
            CurTarget_Check();
            LookAt();
            timer += Time.deltaTime;
            yield return null;
        }
        teleportChargeVFX.SetActive(false);

        // Sound
        sound.SoundPlay_Other(0);

        // Pos Setting
        Vector2 movePos = oneSlashPos.isLeft ? oneSlashPos.movePosR.position : oneSlashPos.movePosL.position;
        movePos.y = 0;
        transform.position = movePos;
        oneSlashPos.gameObject.SetActive(false);
        LookAt();

        // Move Effect
        teleportVFX.SetActive(true);

        // Animaton
        anim.SetTrigger("Attack");
        anim.SetBool("isTeleport", true);
        while(anim.GetBool("isTeleport"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay/2);

        isAttack = false;
    } // End

    private IEnumerator UpwardSlash()
    {
        state = State.Attack;
        isAttack = true;
        attackCount++;

        CurTarget_Check();
        LookAt();

        // Animation
        anim.SetTrigger("Attack");
        anim.SetBool("isUpawrdSlash", true);
        while(anim.GetBool("isUpawrdSlash"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        isAttack = false;
    } // End

    public void UpwardCollider()
    {
        upwardCollider.SetActive(upwardCollider.activeSelf ? false : true);

        // Sound
        if (!upwardCollider.activeSelf)
        {
            sound.SoundPlay_Other(1);
        }
    } // End

    public void UpwardMoveCall()
    {
        StartCoroutine(UpwardMove());
    } // End

    private IEnumerator UpwardMove()
    {
        Vector2 startPos = transform.position;
        Vector2 endPos = upwardMovePos.position;
        float timer = 0;
        while(timer < 1 && !isWall)
        {
            timer += Time.deltaTime * (isPhase2 ? 2.5f : 2f);
            transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
    } // End

    private IEnumerator GroundOneSlash() 
    {
        state = State.Attack;
        isAttack = true;
        attackCount++;

        anim.SetTrigger("Attack");
        anim.SetBool("isGroundOneReady", true);
        anim.SetBool("isGroundOneSlash", true);

        // Charge
        float timer = 0;
        while (timer < 1f)
        {
            LookAt();
            timer += Time.deltaTime;
            yield return null;
        }
        anim.SetBool("isGroundOneReady", false);

        // Sound
        sound.SoundPlay_Other(2);

        // Move + Attack
        groundOneCollider[0].SetActive(true);
        Vector2 startPos = transform.position;
        Vector2 endPos = groundOneMovePos.position;
        timer = 0;
        while (timer < 1f && !isWall)
        {
            timer += Time.deltaTime * (isPhase2 ? 3f : 2f);
            transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        groundOneCollider[0].SetActive(false);

        // Attack Collider 2
        groundOneCollider[1].SetActive(true);
        while (groundOneCollider[1].activeSelf)
        {
            yield return null;
        }

        // Animation Wait
        while(anim.GetBool("isGroundOneSlash"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        isAttack = false;
    } // End

    private IEnumerator GroundFlurry()
    {
        state = State.Attack;
        isAttack = true;
        attackCount++;

        CurTarget_Check();
        LookAt();

        // Animation
        anim.SetTrigger("Attack");
        anim.SetBool("isGroundFlurryReady", true);
        anim.SetBool("isGroundFlurryM", true);
        anim.SetBool("isGroundFlurry", true);

        // Sound
        sound.SoundPlay_Other(3);

        // Attack Delay F
        flurryChargeVFX.SetActive(true);    
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * (isPhase2 ? 2f : 1.25f);
            yield return null;
        }
        flurryChargeVFX.SetActive(false);

        // Move -> Phase 2 Only
        if (isPhase2)
        {
            Vector2 startPos = transform.position;
            Vector2 endPos = groundFlurryMovePos.position;
            timer = 0;
            while(timer < 1)
            {
                timer += Time.deltaTime * 3.5f;
                transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
                yield return null;
            }
        }
        anim.SetBool("isGroundFlurryReady", false);

        // Attack Loop
        sound.SoundPlay_Other(4);
        groundFlurryCollider[0].SetActive(true);
        yield return new WaitForSeconds(isPhase2 ? 1.7f : 1.5f);
        groundFlurryCollider[0].SetActive(false);
        anim.SetBool("isGroundFlurryM", false);

        // Animation Wait
        while (anim.GetBool("isGroundFlurry"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        isAttack = false;
    } // End

    public void groundFlurryColliderB1Call()
    {
        groundFlurryCollider[1].SetActive(groundFlurryCollider[1].activeSelf ? false : true);
    } // End

    private IEnumerator AirOneSlash()
    {
        state = State.Attack;
        isAttack = true;
        attackCount++;

        GravitySetting(true);
        CurTarget_Check();
        LookAt();

        // Move Up
        transform.position = new Vector3(transform.position.x, curTarget.transform.position.y, 0);
        teleportVFX.SetActive(true);

        // Aniamtion
        anim.SetTrigger("Attack");
        anim.SetBool("isAirOneSlashReady", true);
        anim.SetBool("isAirOneSlash", true);
        anim.SetBool("isFalling", true);
        anim.SetBool("isFallingEndAnim", true);

        // Charge
        float timer = 0;
        while (timer < 1)
        {
            CurTarget_Check();
            LookAt();
            timer += Time.deltaTime * (isPhase2 ? 4f : 3f);
            yield return null;
        }
        anim.SetBool("isAirOneSlashReady", false);

        // MovePos Setting
        Vector2 startPos = transform.position;
        Vector2 endPos = groundOneMovePos.position;
        endPos.y = transform.position.y;

        // Sound On
        sound.SoundPlay_Other(2);

        // Move + Attack 1
        airOneCollider[0].SetActive(true);
        timer = 0;
        while(timer < 1 && !isWall)
        {
            timer += Time.deltaTime;
            transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        airOneCollider[0].SetActive(false);

        // Attack 2
        airOneCollider[1].SetActive(true);
        while(airOneCollider[1].activeSelf)
        {
            yield return null;
        }
        anim.SetBool("isAirOneSlash", false);

        // Ground Wait
        GravitySetting(false);
        while(!isGround)
        {
            yield return null;
        }
        anim.SetBool("isFalling", false);

        // Animamton Wait
        while (anim.GetBool("isFallingEndAnim"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        isAttack = false;
    } // End

    private IEnumerator AirFlurry()
    {
        state = State.Attack;
        isAttack = true;
        attackCount++;

        GravitySetting(true);
        CurTarget_Check();
        LookAt();

        // Move Up
        transform.position = new Vector3(transform.position.x, curTarget.transform.position.y, 0);
        teleportVFX.SetActive(true);

        // Animation
        anim.SetTrigger("Attack");
        anim.SetBool("isAirFlurryReady", true);
        anim.SetBool("isAirFlurryM", true);
        anim.SetBool("isAirFlurry", true);
        anim.SetBool("isFalling", true);
        anim.SetBool("isFallingEndAnim", true);

        // Sound On
        sound.SoundPlay_Other(3);

        // Charge
        flurryChargeVFX.SetActive(true);
        float timer = 0;
        while(timer < (isPhase2 ? 0.35f : 0.5f))
        {
            timer += Time.deltaTime;
            yield return null;
        }
        flurryChargeVFX.SetActive(false);

        // Move -> Phase 2 Only
        Vector2 startPos;
        Vector2 endPos;
        if (isPhase2)
        {
            startPos = transform.position;
            endPos = groundFlurryMovePos.position;
            timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime * 3.5f;
                transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
                yield return null;
            }
        }
        anim.SetBool("isAirFlurryReady", false);

        // Frist Attack -> Rush
        airFlurryCollider[0].SetActive(true);
        startPos = transform.position;
        endPos = airFlurryMovePos[0].position;
        timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * (isPhase2 ? 1.25f : 1f);
            transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        airFlurryCollider[0].SetActive(false);

        // Attack -> Add
        airFlurryCollider2[0].SetActive(true);
        while(airFlurryCollider2[0].activeSelf)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.15f);

        // Second Attack
        anim.SetTrigger("Attack");
        isInvincibility = true;
        spriteRenderer.enabled = false;
        airFlurryCollider[1].SetActive(true);
        yield return new WaitForSeconds(isPhase2 ? 2.25f : 1.75f);
        anim.SetBool("isAirFlurryM", false);
        airFlurryCollider[1].SetActive(false);
        spriteRenderer.enabled = true;
        isInvincibility = false;

        // Last Attack -> Rush
        startPos = transform.position;
        endPos = airFlurryMovePos[1].position;
        timer = 0;
        airFlurryCollider[2].SetActive(true);
        while(timer < 1)
        {
            timer += Time.deltaTime * (isPhase2 ? 2f : 1.5f);
            transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        airFlurryCollider[2].SetActive(false);

        // Attack -> Add
        airFlurryCollider2[1].SetActive(true);
        while (airFlurryCollider2[1].activeSelf)
        {
            yield return null;
        }
        anim.SetBool("isAirFlurry", false);

        // Ground Wait
        GravitySetting(false);
        while (!isGround)
        {
            yield return null;
        }
        anim.SetBool("isFalling", false);

        // Animamton Wait
        while (anim.GetBool("isFallingEndAnim"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        GravitySetting(false);
        isAttack = false;
    } // End

    private IEnumerator ComboSlash()
    {
        state = State.Attack;
        isAttack = true;
        attackCount++;

        CurTarget_Check();
        LookAt();

        anim.SetTrigger("Attack");
        anim.SetBool("isComboSlash", true);
        while(anim.GetBool("isComboSlash"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        isAttack = false;
    }  // End

    public void ComboACollider()
    {
        comboCollider[0].SetActive(comboCollider[0].activeSelf ? false : true);

        // Sound
        if (!comboCollider[0].activeSelf)
        {
            sound.SoundPlay_Other(6);
        }
    } // End

    public void ComboBCollider()
    {
        comboCollider[1].SetActive(comboCollider[1].activeSelf ? false : true);

        // Sound
        if (!comboCollider[1].activeSelf)
        {
            sound.SoundPlay_Other(6);
        }
    } // End

    public void ComboC1Collider()
    {
        comboCollider[2].SetActive(comboCollider[2].activeSelf ? false : true);

        // Sound
        if (!comboCollider[2].activeSelf)
        {
            sound.SoundPlay_Other(7);
        }
    } // End

    public void ComboC2Collider()
    {
        comboCollider[3].SetActive(comboCollider[3].activeSelf ? false : true);

        // Sound
        if (!comboCollider[3].activeSelf)
        {
            sound.SoundPlay_Other(7);
        }
    } // End

    public void ComboMoveCall()
    {
        StartCoroutine(ComboMove());
    } // End

    public void ComboShootCall()
    {
        Vector2 shotDir = (auraShootPos.position - transform.position).normalized;
        GameObject obj = Instantiate(swordAuraVFX, transform.position, Quaternion.identity);
        obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.None, shotDir, 15, 30, 15);
    } // End

    private IEnumerator ComboMove()
    {
        // Target Look
        CurTarget_Check();
        LookAt();

        // Move
        Vector2 startPos = transform.position;
        Vector2 endPos = comboMovePos.position;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * (isPhase2 ? 3.5f : 2.8f);
            transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
    } // End

    private IEnumerator HalfMoonSlash()
    {
        state = State.Attack;
        isAttack = true;
        attackCount++;

        GravitySetting(true);
        CurTarget_Check();
        LookAt();

        // Animation
        anim.SetTrigger("Attack");
        anim.SetBool("isHalfMoonReady", true);
        anim.SetBool("isHalfMoonSlash", true);

        anim.SetBool("isFalling", true);
        anim.SetBool("isFallingEndAnim", true);

        // Sound
        sound.SoundPlay_Other(8);

        // Fade
        fadeImage.gameObject.SetActive(true);
        float a = 0;
        while (a < 1)
        {
            a += Time.deltaTime * 2f;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, a);
            yield return null;
        }

        // Move Up
        Vector2 movePos = new Vector2(curTarget.transform.position.x, curTarget.transform.position.y + 3f);
        transform.position = movePos;
        teleportVFX.SetActive(true);
        while(teleportVFX.activeSelf)
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.25f);

        // Effect Wait
        while (halfMoonCollider.activeSelf)
        {
            yield return null;
        }

        // Attack
        anim.SetBool("isHalfMoonReady", false);
        while(anim.GetBool("isHalfMoonSlash"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.15f);

        // Fade Out
        a = 1;
        while (a > 0)
        {
            a -= Time.deltaTime * 5f;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, a);
            yield return null;
        }
        fadeImage.gameObject.SetActive(false);

        // Ground Wait
        GravitySetting(false);
        while (!isGround)
        {
            yield return null;
        }
        anim.SetBool("isFalling", false);

        // Animamton Wait
        while (anim.GetBool("isFallingEndAnim"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        isAttack = false;
    } // End

    public void HaifMoonCollider()
    {
        halfMoonCollider.SetActive(true);

        // Sound
        if (!halfMoonCollider.activeSelf)
        {
            sound.SoundPlay_Other(9);
        }
    }
    #endregion


    #region Phase 2 Skill
    private IEnumerator HorizontalSlash()
    {
        state = State.Attack;
        isAttack = true;
        isInvincibility = true;
        attackCount++;

        // Sound
        sound.SoundPlay_Other(0);

        // Teleport + Hide + Effect
        teleportVFX.SetActive(true);
        spriteRenderer.enabled = false;
        transform.position = returnPos.position;
        while(teleportVFX.activeSelf)
        {
            yield return null;
        }

        // Delay F
        yield return new WaitForSeconds(0.25f);

        // Warring
        for (int i = 0; i < horizontalWarringVFX.Length; i++)
        {
            horizontalWarringVFX[i].SetActive(true);
            yield return new WaitForSeconds(0.15f);
            horizontalWarringVFX[i].SetActive(false);
            yield return new WaitForSeconds(0.15f);
        }

        // Delay F
        yield return new WaitForSeconds(0.25f);

        // Attack
        for (int i = 0; i < horizontalWarringVFX.Length; i++)
        {
            // Sound
            sound.SoundPlay_Other(2);

            // Attack
            HorizontalCollider[i].SetActive(true);
            yield return new WaitForSeconds(0.3f);
            HorizontalCollider[i].SetActive(false);

            // Delay
            if (i < horizontalWarringVFX.Length - 1)
            {
                yield return new WaitForSeconds(0.25f);
            }
        }

        // Sound
        sound.SoundPlay_Other(0);

        // Hide Off
        transform.position = returnPos.position;
        teleportVFX.SetActive(true);
        spriteRenderer.enabled = true;

        // Delay
        yield return new WaitForSeconds(delay);

        isAttack = false;
    } // 사운드 미묘 -> 추후 체크 필요! End

    private IEnumerator SweepingSlash()
    {
        state = State.Attack;
        isAttack = true;
        attackCount++;

        // Sound
        sound.SoundPlay_Other(0);

        // Teleport + Effect
        int ran = Random.Range(0, sweepingMovePos.Length);
        transform.position = sweepingMovePos[ran].position;
        teleportVFX.SetActive(true);
        CurTarget_Check();
        LookAt();

        // Effect Wait
        while(teleportVFX.activeSelf)
        {
            yield return null;
        }

        // Animation
        anim.SetTrigger("Attack");
        anim.SetBool("isSweepingReady", true);
        anim.SetBool("isSweepingSlash", true);

        // Sound
        sound.SoundPlay_Other(3);

        // Charge
        sweepingChargeVFX.SetActive(true);
        float timer = 0;
        while(timer < 1f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        sweepingChargeVFX.SetActive(false);
        anim.SetBool("isSweepingReady", false);

        // Sound
        sound.SoundPlay_Other(2);

        // Move
        Vector2 startPos = ran == 0 ? sweepingMovePos[0].position : sweepingMovePos[1].position;
        Vector2 endPos = ran == 0 ? sweepingMovePos[1].position : sweepingMovePos[0].position;
        timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        // Sound
        sound.SoundPlay_Other(9);

        // Attack 1
        sweepingCollder[0].SetActive(true);
        while(sweepingCollder[0].activeSelf)
        {
            yield return null;
        }

        // Attack 2
        sweepingCollder[1].SetActive(true);

        // Animation Wait
        while (anim.GetBool("isSweepingSlash"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);
        isAttack = false;
    } // End

    private IEnumerator CenterSlash()
    {
        state = State.Attack;
        isAttack = true;
        attackCount++;

        // Sound
        sound.SoundPlay_Other(0);

        // Teleport + Effect
        transform.position = returnPos.position;
        teleportVFX.SetActive(true);
        while(teleportVFX.activeSelf)
        {
            yield return null;
        }

        CurTarget_Check();
        LookAt();

        // Animation
        anim.SetTrigger("Attack");
        anim.SetBool("CenterCharge", true);
        anim.SetBool("isCenterSlash", true);

        // Fade
        StartCoroutine(Fade(true, 2f));

        // Charge + Attack A
        CenterSlashChargeVFX.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            if(i == 0)
            {
                // Sound
                sound.SoundPlay_Other(2);

                // Frist Attack
                for (int i2 = 0; i2 < centerSlashA.Length; i2++)
                {
                    centerSlashA[i2].gameObject.SetActive(true);
                    Target_Setting();
                    centerSlashA[i2].Setting(curTarget, CenterMovePos(), 0.75f, 0.5f);
                }

                // Wait
                while (centerSlashA[0].gameObject.activeSelf)
                {
                    yield return null;
                }
            }
            else if(i == 1)
            {
                // Sound
                sound.SoundPlay_Other(2);

                // second Attack
                for (int i2 = 0; i2 < centerSlashB.Length; i2++)
                {
                    centerSlashB[i2].gameObject.SetActive(true);
                    Target_Setting();
                    centerSlashB[i2].Setting(curTarget, CenterMovePos(), 0.85f, 0.5f);
                }

                // Wait
                while (centerSlashB[0].gameObject.activeSelf)
                {
                    yield return null;
                }
            }
            else
            {
                // Sound
                sound.SoundPlay_Other(2);

                // Third Attack
                for (int i2 = 0; i2 < centerSlashC.Length; i2++)
                {
                    centerSlashC[i2].gameObject.SetActive(true);
                    Target_Setting();
                    centerSlashC[i2].Setting(curTarget, CenterMovePos(), 0.95f, 0.5f);
                }

                // Wait
                while (centerSlashC[0].gameObject.activeSelf)
                {
                    yield return null;
                }
            }

            // Delay
            yield return new WaitForSeconds(0.35f);
        }
        CenterSlashChargeVFX.SetActive(false);

        // Delay F
        yield return new WaitForSeconds(0.5f);

        // Attack B - Center Slash
        anim.SetBool("CenterCharge", false);
        while (anim.GetBool("isCenterSlash"))
        {
            yield return null;
        }
        while(centerBCollider.activeSelf)
        {
            yield return null;
        }

        // Fade Out
        float a = 1;
        while (a > 0)
        {
            a -= Time.deltaTime * 5f;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, a);
            yield return null;
        }
        fadeImage.gameObject.SetActive(false);

        // Delay
        yield return new WaitForSeconds(delay);

        isAttack = false;
    }

    public void CenterSlashCollider()
    {
        centerBCollider.SetActive(true);
        Debug.Log("Call Cen On");
        // Sound
        if(!centerBCollider.activeSelf)
        {
            sound.SoundPlay_Other(9);
        }
    }

    public void CenterExplosionCall()
    {
        StartCoroutine(CenterExplosion());
    }

    private IEnumerator CenterExplosion()
    {
        for (int i = 0; i < centerExplosionPosL.Length; i++)
        {
            Instantiate(centerExplosion, centerExplosionPosL[i].position, Quaternion.identity);
            Instantiate(centerExplosion, centerExplosionPosR[i].position, Quaternion.identity);
            yield return new WaitForSeconds(0.15f);
        }
    }

    private Vector2 CenterMovePos()
    {
        float x = centerMoveCollider.bounds.size.x;
        float y = centerMoveCollider.bounds.size.y;

        x = Random.Range((x / 2) * -1, x / 2);
        y = Random.Range((y / 2) * -1, y / 2);
        Debug.Log("x : " + x + "y : " + y);

        Vector2 moveDir = new(x + centerMoveCollider.transform.position.x, y + centerMoveCollider.transform.position.y);
        Debug.Log(moveDir);
        return moveDir;
    }

    private IEnumerator Fade(bool isOn, float time)
    {
        float timer = isOn ? 0 : 1;
        if(isOn)
        {
            // Fade In
            fadeImage.gameObject.SetActive(true);
            while (timer < 1)
            {
                timer += Time.deltaTime * time;
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, timer);
                yield return null;
            }
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);
        }
        else
        {
            // Fade Out
            while (timer > 0)
            {
                timer -= Time.deltaTime * time;
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, timer);
                yield return null;
            }
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
            fadeImage.gameObject.SetActive(false);
        }
    }

    #endregion

    private IEnumerator AttackCountSkill()
    {
        state = State.Attack;
        isAttack = true;
        attackCount = 0;

        // Fade On
        StartCoroutine(Fade(true, 2f));

        // Attack
        if(isPhase2)
        {
            // Phase 2 Attack
            StartCoroutine(SweepingSlash());
            while (isAttack)
            {
                yield return null;
            }

            StartCoroutine(HalfMoonSlash());
            while (isAttack)
            {
                yield return null;
            }
        }
        else
        {
            // Phase 1 Attack
            StartCoroutine(SweepingSlash());
            while (isAttack)
            {
                yield return null;
            }

            StartCoroutine(SweepingSlash());
            while (isAttack)
            {
                yield return null;
            }
        }

        // Fade Off
        StartCoroutine(Fade(false, 1f));

        // Delay
        yield return new WaitForSeconds(delay);
        state = State.Idle;
        isAttack = false;
    }

    private void TargetLRCheck(GameObject followTarget)
    {
        oneSlashPos.CheckCall(followTarget);
    }

    private void GravitySetting(bool isOff)
    {
        if (isOff)
        {
            rigid.velocity = Vector2.zero;
            rigid.gravityScale = 0;
        }
        else
        {
            rigid.velocity = Vector2.zero;
            rigid.gravityScale = 1;
        }
    }

    private void WallCheck()
    {
        isWall = Physics2D.Raycast(transform.position, new Vector2(transform.localScale.x, 0), 1f, groundLayer);
    }

    private IEnumerator Phase2On()
    {
        state = State.Think;
        isInvincibility = true;
        isPhase2 = true;

        // Sound
        sound.SoundPlay_Other(10);

        // Phase Attack
        StartCoroutine(CenterSlash());
        while(isAttack)
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.5f);

        state = State.Idle;
        isInvincibility = false;
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

        Stage_Manager.instance.BGM_Setting(1);

        // Sound
        sound.SoundPlay_public(Enemy_Sound.PublicSound.Spawn);

        // Spawn UI
        statusUI_Boss.StartFadeCall();

        /*
        // Animation
        anim.SetTrigger("Spawn");
        anim.SetBool("isSpawn", true);
        while(anim.GetBool("isSpawn"))
        {
            yield return null;
        }
        */

        // Delay
        yield return new WaitForSeconds(1f);

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
        if(hitStopCoroutine != null)
        {
            StopCoroutine(hitStopCoroutine);
        }
        StopAllCoroutines();

        StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        state = State.Die;
        isDie = true;
        isAttack = false;

        // Animation Reset
        for (int i = 0; i < animationbool.Length; i++)
        {
            anim.SetBool(animationbool[i], false);
        }
        for (int i = 0; i < animationTrigger.Length; i++)
        {
            anim.ResetTrigger(animationTrigger[i]);
        }
        anim.SetFloat("AttackSpeed", 0);

        // Effect Reset
        fadeImage.gameObject.SetActive(false);
        CenterSlashChargeVFX.SetActive(false);
        flurryChargeVFX.SetActive(false);
        HalfMoonChargeVFX.SetActive(false);
        sweepingChargeVFX.SetActive(false);
        teleportChargeVFX.SetActive(false);
        teleportVFX.SetActive(false);
        for (int i = 0; i < horizontalWarringVFX.Length; i++)
        {
            horizontalWarringVFX[i].SetActive(false);
        }

        // Collider Reset
        upwardCollider.SetActive(false);
        halfMoonCollider.SetActive(false);
        centerBCollider.SetActive(false);

        for (int i = 0; i < sweepingCollder.Length; i++)
        {
            sweepingCollder[i].SetActive(false);
        }
        for (int i = 0; i < airOneCollider.Length; i++)
        {
            airOneCollider[i].SetActive(false);
        }
        for (int i = 0; i < groundOneCollider.Length; i++)
        {
            groundOneCollider[i].SetActive(false);
        }
        for (int i = 0; i < groundFlurryCollider.Length; i++)
        {
            groundFlurryCollider[i].SetActive(false);
        }
        for (int i = 0; i < airFlurryCollider.Length; i++)
        {
            airFlurryCollider[i].SetActive(false);
        }
        for (int i = 0; i < HorizontalCollider.Length; i++)
        {
            HorizontalCollider[i].SetActive(false);
        }
        for (int i = 0; i < comboCollider.Length; i++)
        {
            comboCollider[i].SetActive(false);
        }

        Stage_Manager.instance.BGM_Setting(0);

        // Sound
        sound.SoundPlay_public(Enemy_Sound.PublicSound.Die);

        // Animation
        anim.SetTrigger("Die");
        anim.SetBool("isDie", true);
        while(anim.GetBool("isDie"))
        {
            yield return null;
        }

        /*
        // UI Call
        stage_Manager.Stage_Clear();
        while (stage_Manager.isUI)
        {
            yield return null;
        }
        */
        
        // Delay
        yield return new WaitForSeconds(1f);

        // Destroy
        Destroy(container);
    }
}
