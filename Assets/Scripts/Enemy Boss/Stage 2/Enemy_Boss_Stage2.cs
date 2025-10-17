using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing;
using Unity.VisualScripting;

public class Enemy_Boss_Stage2 : Enemy_Base
{
    private LineRenderer line;

    [Header("---Stage 2 Boss Setting---")]
    [SerializeField] private Stage_Manager stage_Manager;
    [SerializeField] private GameObject container;
    [SerializeField] private float hitTimer;
    [SerializeField] private bool isPhase2;
    private int attackCount;
    private bool isCombo;
    private bool isWall;
    private Vector3 endPos;

    [Header("---Move Pos---")]
    [SerializeField] private Transform comboMovePos;
    [SerializeField] private Transform backstepPos;
    [SerializeField] private Transform groundRushPos;
    [SerializeField] private Transform[] airRushPos1;
    [SerializeField] private Transform[] airRushPos2;

    [Header("---Exlposion Pos---")]
    [SerializeField] private Transform[] airRushShotPos;
    [SerializeField] private Transform[] backExplosionPos;
    [SerializeField] private Transform[] airExplosionPosL;
    [SerializeField] private Transform[] airExplosionPosR;
    [SerializeField] private Transform[] GroundRushExlposionPos;
    [SerializeField] private Transform[] countSwordAuraShotDir;


    [Header("---Attack Collider---")]
    public GameObject counterCollider;
    public GameObject[] comboCollider;
    public GameObject groundRushCollider;
    public GameObject airRushCollider;
    public GameObject backstepcollider;
    public GameObject superCollider;

    [Header("---Prefab---")]
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject[] bullet;
    [SerializeField] private GameObject countSwordAura;
    [SerializeField] private GameObject airRushAura;
    [SerializeField] private GameObject airRushEffect;

    // FSM
    // 가드 -> N초 이내에 피격 당했는가 ?
    // Y -> 카운터 슬래쉬
    // N -> ( 그라운드 & 에어 ) 러쉬
    

    // 카운터 슬래쉬 이후 연계
    // 1. 콤보 -> 백스탭
    // 2. 콤보 -> 그라운드 -> 백스탭


    // ( 그라운드 & 에어 ) 러쉬 이후 연계
    // 1. 콤보 -> 백스탭
    // 2. 백스탭 -> 에어 -> 그라운드
    // 3. 에어 -> 백스탭 -> 그라운드 -> 콤보


    // 어택 카운트 10 이상 콤보
    // 1. 그라운드 -> 콤보 -> 에어 -> 백스탭 -> 슈퍼
    // 2. 그라운드 -> 콤보 -> 백스탭 -> 그라운드 -> 슈퍼


    // 슈퍼 슬래쉬 작업해야함! -> 에셋 살거면 빨리 사서 이벤트 넣고 돌리면 제작 끝날듯?

    // Sound Index
    // 0 : Combo A B
    // 1 : Combo C, backstep Slash, Counter B
    // 2 : Counter A
    // 3 : Ground & Air Rush Charge
    // 4 : Ground & Air Rush
    // 5 : Explosion
    // 6 : Super A
    // 7 : Super B

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        Status_Setting();
        Spawn();
        hitTimer = 1.25f;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            StartCoroutine(BackstepSlash());
        }

        Vector3 rayDir = (backstepPos.position - transform.position).normalized;
        Vector3 rayStart = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Debug.DrawRay(rayStart, rayDir, Color.red, (backstepPos.position - transform.position).magnitude);
        
        // Spawn & Die Check
        if (state == State.Spawn || state == State.Die)
        {
            return;
        }

        WallCheck();
        GroundCheck();

        // Phase 2 Check
        if(!isPhase2 && hp <= 500)
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

        if (state == State.Idle && !isAttack && !isDie && !isCombo)
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

        if(attackCount >= 10)
        {
            int ran = Random.Range(0, 100);
            if(ran <= 50)
            {
                hitStopCoroutine = StartCoroutine(SuperComboA());
            }
            else
            {
                hitStopCoroutine = StartCoroutine(SuperComboB());
            }
        }
        else
        {
            hitStopCoroutine = StartCoroutine(CounterWait());
        }
    }

    #region Counter Combo
    private IEnumerator ComboA()
    {
        isCombo = true;

        // Combo Slash
        StartCoroutine(ComboSlash());
        while(isAttack)
        {
            yield return null;
        }

        // Backstep Slash
        StartCoroutine(BackstepSlash());
        while (isAttack)
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
        isCombo = false;
    }

    private IEnumerator ComboB()
    {
        isCombo = true;

        // Combo Slash
        StartCoroutine(ComboSlash());
        while (isAttack)
        {
            yield return null;
        }

        // Ground Rush
        StartCoroutine(GroundRush());
        while (isAttack)
        {
            yield return null;
        }

        // Backstep Slash
        StartCoroutine(BackstepSlash());
        while (isAttack)
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
        isCombo = false;
    }

    private IEnumerator ComboC_New()
    {
        isCombo = true;

        StartCoroutine(AirRush());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(ComboSlash());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(BackstepSlash());
        while (isAttack)
        {
            yield return null;
        }

        StartCoroutine(GroundRush());
        while (isAttack)
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
        isCombo = false;
    }
    #endregion

    #region None Hit Combo
    private IEnumerator ComboC()
    {
        isCombo = true;

        // Combo Slash
        StartCoroutine(ComboSlash());
        while (isAttack)
        {
            yield return null;
        }

        // Backstep Slash
        StartCoroutine(BackstepSlash());
        while (isAttack)
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
        isCombo = false;
    }

    private IEnumerator ComboD()
    {
        isCombo = true;

        // Backstep Slash
        StartCoroutine(BackstepSlash());
        while (isAttack)
        {
            yield return null;
        }

        // Air Rush
        StartCoroutine(AirRush());
        while (isAttack)
        {
            yield return null;
        }

        // Ground Rush
        StartCoroutine(GroundRush());
        while (isAttack)
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
        isCombo = false;
    }

    private IEnumerator ComboE()
    {
        isCombo = true;

        // Air Rush
        StartCoroutine(AirRush());
        while (isAttack)
        {
            yield return null;
        }

        // Backstep Slash
        StartCoroutine(BackstepSlash());
        while (isAttack)
        {
            yield return null;
        }

        // Ground Rush
        StartCoroutine(GroundRush());
        while (isAttack)
        {
            yield return null;
        }

        // Combo Slash
        StartCoroutine(ComboSlash());
        while (isAttack)
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
        isCombo = false;
    }
    #endregion

    #region Super Slash Combo
    private IEnumerator SuperComboA()
    {
        isCombo = true;
        attackCount = 0;

        // Ground Rush
        StartCoroutine(GroundRush());
        while (isAttack)
        {
            yield return null;
        }

        // Combo Slash
        StartCoroutine(ComboSlash());
        while (isAttack)
        {
            yield return null;
        }

        // Backstep Slash
        StartCoroutine(BackstepSlash());
        while (isAttack)
        {
            yield return null;
        }

        // Ground Rush
        StartCoroutine(GroundRush());
        while (isAttack)
        {
            yield return null;
        }

        // Super Slash
        StartCoroutine(SuperSlash());
        while (isAttack)
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
        isCombo = false;
    }

    private IEnumerator SuperComboB()
    {
        isCombo = true;
        attackCount = 0;

        // Ground Rush
        StartCoroutine(GroundRush());
        while (isAttack)
        {
            yield return null;
        }

        // Combo Slash
        StartCoroutine(ComboSlash());
        while (isAttack)
        {
            yield return null;
        }

        // Air Rush
        StartCoroutine(AirRush());
        while (isAttack)
        {
            yield return null;
        }

        // Backstep Slash
        StartCoroutine(BackstepSlash());
        while (isAttack)
        {
            yield return null;
        }

        // Super Slash
        StartCoroutine(SuperSlash());
        while (isAttack)
        {
            yield return null;
        }

        state = State.Idle;
        isAttack = false;
        isCombo = false;
    }
    #endregion

    private IEnumerator CounterWait()
    {
        anim.SetTrigger("Attack");
        anim.SetBool("isCountWait", true);
        anim.SetBool("isCount", true);

        // Sound
        sound.SoundPlay_Other(2);

        // Counter Wait
        float timer = hitTimer;
        while(timer > 0)
        {
            if(isHit)
            {
                // Hit Attack
                anim.SetTrigger("GuardHit");
                anim.SetBool("isCountWait", false);
                hitStopCoroutine = StartCoroutine(CountSlash());
                yield break;
            }

            timer -= Time.deltaTime;
            yield return null;
        }

        anim.SetBool("isCountWait", false);

        // None Hit Attack
        if (timer <= 0)
        {
            int ran = Random.Range(0, 100);
            if (ran <= 33)
            {
                hitStopCoroutine = StartCoroutine(ComboC());
            }
            else if (ran <= 66)
            {
                hitStopCoroutine = StartCoroutine(ComboD());
            }
            else
            {
                hitStopCoroutine = StartCoroutine(ComboE());
            }
        }
    }

    private IEnumerator CountSlash()
    {
        state = State.Attack;
        isAttack = true;
        attackCount++;

        // Target Look -> 애니메이션이 반대로 들어가 있어서 이렇게 만듬
        CurTarget_Check();
        LookAt();
        if (transform.localScale.x == -1)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Delay F
        yield return new WaitForSeconds(0.25f);

        // Animation Wait
        anim.SetTrigger("Attack");
        anim.SetBool("isCountSlash", true);
        while(anim.GetBool("isCountSlash"))
        {
            yield return null;
        }

        isAttack = false;

        // Next Attack Setting
        int ran = Random.Range(0, 100);
        if(ran <= 20)
        {
            hitStopCoroutine = StartCoroutine(ComboA());
        }
        else if(ran <= 40)
        {
            hitStopCoroutine = StartCoroutine(ComboB());
        }
        else if(ran <= 60)
        {
            hitStopCoroutine = StartCoroutine(ComboC());
        }
        else if (ran <= 60)
        {
            hitStopCoroutine = StartCoroutine(ComboC_New());
        }
        else if (ran <= 60)
        {
            hitStopCoroutine = StartCoroutine(ComboD());
        }
    }

    public void CoounterAttackCollider()
    {
        counterCollider.SetActive(counterCollider.activeSelf ? false : true);
        // Sound
        if (!counterCollider.activeSelf)
        {
            sound.SoundPlay_Other(2);
        }
    }

    public void CountShotCall()
    {
        for (int i = 0; i < (isPhase2 ? 2 : 1); i++)
        {
            Vector2 shotDir = (countSwordAuraShotDir[i].position - transform.position).normalized;
            GameObject obj = Instantiate(countSwordAura, transform.position, Quaternion.identity);
            obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.Red, shotDir, 15, 30, 15);
        }
    }

    private IEnumerator GroundRush()
    {
        state = State.Attack;
        isAttack = true;
        attackCount++;

        for (int i = 0; i < (isPhase2 ? 2 : 1); i++)
        {
            anim.SetTrigger("Attack");
            anim.SetBool("isRushReady", true);
            anim.SetBool("isGroundRush", true);
            CurTarget_Check();
            LookAt();

            // Sound
            sound.SoundPlay_Other(3);

            // Charge + LookAt
            float timer = 0;
            while (timer < (isPhase2 ? 0.66f : 1f))
            {
                LookAt();
                timer += Time.deltaTime;
                yield return null;
            }
            anim.SetBool("isRushReady", false);

            // Animation Wait
            while (anim.GetBool("isGroundRush"))
            {
                yield return null;
            }

            // Delay
            if(i < (isPhase2 ? 2 : 1))
            {
                yield return new WaitForSeconds(0.25f);
            }
        }
        isAttack = false;
    } // End

    public void GroundRushMoveCall()
    {
        StartCoroutine(GroundRushMove());
    } // End

    private IEnumerator GroundRushMove()
    {
        // Sound
        sound.SoundPlay_Other(4);

        // Move Pos Setting
        Vector3 startPos = transform.position;
        Vector3 endPos = groundRushPos.position;
        Vector3 rayDir = (groundRushPos.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(startPos, rayDir, (groundRushPos.position - transform.position).magnitude, groundLayer);
        if (hit.collider != null) endPos = hit.point + hit.normal * 1f;

        // Move
        groundRushCollider.SetActive(true);
        float timer = 0f;
        while (timer < 1 && !isWall)
        {
            timer += Time.deltaTime * (isPhase2 ? 3.5f : 3f);
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        groundRushCollider.SetActive(false);
    } // End

    private IEnumerator AirRush()
    {
        state = State.Attack;
        isAttack = true;

        // Attack
        int count = Random.Range(3, 4); // Attack Count
        for (int i = 0; i < count; i++)
        {
            Target_Setting();
            CurTarget_Check();
            LookAt();

            // Animation
            anim.SetTrigger("Attack");
            anim.SetBool("isRushReady", true);
            anim.SetBool("isAirRush", true);
            anim.SetBool("isAirRushLanding", true);

            // Move Up
            int posRan = Random.Range(0, 2); // 0 => Left  |  1 => Right
            rigid.velocity = Vector2.zero;
            rigid.gravityScale = 0;
            if (posRan == 0)
            {
                posRan = Random.Range(0, airRushPos1.Length);
                transform.position = airRushPos1[posRan].position;
            }
            else
            {
                posRan = Random.Range(0, airRushPos2.Length);
                transform.position = airRushPos2[posRan].position;
            }

            // Effect
            airRushEffect.SetActive(true);

            // Attack Delay
            line.enabled = true;
            line.SetPosition(0, transform.position);
            float timer = 0;
            while(timer < (isPhase2 ? 0.45f : 0.66f))
            {
                line.SetPosition(1, curTarget.transform.position);
                timer += Time.deltaTime;
                LookAt();
                yield return null;
            }
            line.enabled = false;

            // Move Setting
            Vector3 startPos = transform.position;
            Vector3 rayDir = (curTarget.transform.position - transform.position).normalized;
            endPos = Physics2D.Raycast(transform.position, rayDir, 150, groundLayer).point;
            rigid.velocity = Vector2.zero;
            anim.SetBool("isRushReady", false);

            // Sound
            sound.SoundPlay_Other(4);

            // Attack => Move Down
            airRushCollider.SetActive(true);
            timer = 0;
            while (timer < 1)
            {
                timer += (isPhase2 ? 1.5f : 1.25f) * Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.InQuart(timer));
                yield return null;
            }
            anim.SetBool("isAirRush", false);
            airRushCollider.SetActive(false);
            rigid.gravityScale = 1;

            // Explosion Attack  |  Explosion Delay => Phase 1 : 0.15f  |  Phase 2 : 0.1f
            StartCoroutine(AirExplosion(isPhase2 ? 0.1f : 0.15f));

            // Animation Wait
            while (anim.GetBool("isAirRushLanding"))
            {
                yield return null;
            }

            // Next Attack Delay
            yield return new WaitForSeconds(isPhase2 ? 0.25f : 0.5f);
        }

        isAttack = false;
    } // End

    private IEnumerator AirExplosion(float timer)
    {
        int ran = Random.Range(0, 100);
        for (int i = 0; i < airExplosionPosL.Length; i++)
        {
            // Explosion
            Instantiate(explosion, airExplosionPosL[i].position, Quaternion.identity);
            Instantiate(explosion, airExplosionPosR[i].position, Quaternion.identity);

            // Sound
            sound.SoundPlay_Other(5);

            // Delay
            yield return new WaitForSeconds(timer);
        }
    } // End

    public void AirShotCall1()
    {
        // Sound
        sound.SoundPlay_Other(1);

        // Shot Dir & Rotation Setting
        Vector3 shotDir = (curTarget.transform.position - transform.position).normalized;
        GameObject obj = Instantiate(airRushAura, transform.position, Quaternion.identity);

        // Sword Aura Move
        obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.None, shotDir, 20, 40, 15);

        // Sword Aura Explosion
        int ran = Random.Range(0, 3);
        if(isPhase2)
        {
            obj.GetComponent<Enemy_WaveExplosion>().ExplosionSetting(ran == 0 ? Enemy_WaveExplosion.Type.White : Enemy_WaveExplosion.Type.Black, 20f);
        }
        else
        {
            obj.GetComponent<Enemy_WaveExplosion>().ExplosionSetting(Enemy_WaveExplosion.Type.None, 15f);
        }
    }

    public void AirShotCall2()
    {
        // -> 이거 페이즈 2 되면 사용하게 할까?
        /*
        int ran = Random.Range(0, bullet.Length);
        for (int i = 0; i < airRushShotPos.Length; i++)
        {
            Vector2 shotDir = (airRushShotPos[i].position - transform.position).normalized;
            GameObject obj = Instantiate(bullet[ran], transform.position, Quaternion.identity);
            obj.GetComponent<Enemy_Bullet>().Bullet_Setting((ran == 0 ? Enemy_Bullet.BulletType.White : Enemy_Bullet.BulletType.Black), shotDir, 15, 30, 15);
        }
        */
    }

    private IEnumerator ComboSlash()
    {
        state = State.Attack;
        isAttack = true;
        attackCount++;

        // Target Look
        CurTarget_Check();
        LookAt();

        // Animtaion
        anim.SetTrigger("Attack");
        anim.SetBool("isComboSlash", true);
        while(anim.GetBool("isComboSlash"))
        {
            yield return null;
        }

        isAttack = false;
    } // End

    public void ComboColliderA()
    {
        comboCollider[0].SetActive(comboCollider[0].activeSelf ? false : true);

        // Sound
        if (!comboCollider[0].activeSelf)
        {
            sound.SoundPlay_Other(0);
        }
    }

    public void ComboColliderB()
    {
        comboCollider[0].SetActive(comboCollider[0].activeSelf ? false : true);

        // Sound
        if (!comboCollider[1].activeSelf)
        {
            sound.SoundPlay_Other(0);
        }
    }

    public void ComboColliderC()
    {
        comboCollider[0].SetActive(comboCollider[0].activeSelf ? false : true);

        // Sound
        if (!comboCollider[2].activeSelf)
        {
            sound.SoundPlay_Other(1);
        }
    }

    public void ComboShotCall()
    {
        Vector2 shotDir = (countSwordAuraShotDir[1].position - transform.position).normalized;
        GameObject obj = Instantiate(countSwordAura, transform.position, Quaternion.identity);
        obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.None, shotDir, 15, 30, 15);
    }

    public void ComboMoveCall()
    {
        StartCoroutine(ComboMove());
    } // End

    private IEnumerator ComboMove()
    {
        // Target Look
        CurTarget_Check();
        LookAt();

        // Move
        Vector2 startPos = transform.position;
        Vector2 endPos = comboMovePos.position;
        Vector3 rayDir = (comboMovePos.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(startPos, rayDir, (comboMovePos.position - transform.position).magnitude, groundLayer);
        if (hit.collider != null) endPos = hit.point + hit.normal * 1f;

        float timer = 0;
        while (timer < 1 && state == State.Attack && !isWall)
        {
            timer += Time.deltaTime * 3f;
            transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        transform.position = endPos;

    } // End

    private IEnumerator BackstepSlash()
    {
        state = State.Attack;
        isAttack = true;
        attackCount++;

        CurTarget_Check();
        LookAt();

        anim.SetTrigger("Attack");
        anim.SetBool("isBackstep", true);
        anim.SetBool("isBackstepSlash", true);

        Vector3 startPos = transform.position;
        Vector3 endPos = backstepPos.position;

        Vector3 rayDir = (backstepPos.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(startPos, rayDir, (backstepPos.position - transform.position).magnitude, groundLayer);
        if (hit.collider != null) endPos = hit.point + hit.normal * 0.5f;

        // Move
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime / 1f;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        transform.position = endPos;

        anim.SetBool("isBackstep", false);
        while(anim.GetBool("isBackstepSlash"))
        {
            yield return null;
        }

        isAttack = false;
    } // End

    public void BackstepCollider()
    {
        backstepcollider.SetActive(backstepcollider.activeSelf ? false : true);

        // Sound
        if (!backstepcollider.activeSelf)
        {
            sound.SoundPlay_Other(1);
        }
    }

    public void BackstepExplosionCall()
    {
        StartCoroutine(BackstepExplosion(isPhase2 ? 0.1f : 0.15f));
    } // End

    private IEnumerator BackstepExplosion(float timer)
    {
        // Attack
        List<Vector2> explosionPos = new List<Vector2>();
        for (int i = 0; i < backExplosionPos.Length; i++)
        {
            explosionPos.Add(backExplosionPos[i].transform.position);
        }

        for (int i = 0; i < explosionPos.Count; i++)
        {
            Vector2 pos = explosionPos[i];
            Instantiate(explosion, pos, Quaternion.identity);

            // Sound
            sound.SoundPlay_Other(5);

            yield return new WaitForSeconds(timer);
        }
    } // End

    private IEnumerator SuperSlash()
    {
        state = State.Attack;
        isAttack = true;
        attackCount = 0;

        CurTarget_Check();
        LookAt();

        anim.SetTrigger("Attack");
        anim.SetBool("isSuperSlashCharge", true);
        anim.SetBool("isSuperSlash", true);

        // Sound
        sound.SoundPlay_Other(6);

        // Charge & Fade
        float timer = 2f;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        anim.SetBool("isSuperSlashCharge", false);

        // Attack
        while(anim.GetBool("isSuperSlash"))
        {
            yield return null;
        }

        isAttack = false;
    }

    public void SuperColliderCall()
    {
        superCollider.SetActive(superCollider.activeSelf == true ? false : true);

        // Sound
        if(superCollider.activeSelf)
        {
            sound.SoundPlay_Other(7);
        }
    }

    private IEnumerator Phase2On()
    {
        state = State.Spawn;
        isAttack = false;
        isPhase2 = true;
        delay *= 0.75f;
        hitTimer = 0.5f;

        // Animation
        anim.SetTrigger("Phase2");
        anim.SetBool("isPhase2", true);
        anim.SetFloat("AttackSpeed", 1.25f);

        // Sound
        sound.SoundPlay_Other(8);

        // Animaton Wait
        while (anim.GetBool("isPhase2"))
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.25f);
        state = State.Idle;
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
        isInvincibility = true;

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

        state = State.Idle;
        isInvincibility = false;
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
        // Collider Off
        counterCollider.SetActive(false);
        groundRushCollider.SetActive(false);
        airRushCollider.SetActive(false);
        backstepcollider.SetActive(false);
        superCollider.SetActive(false);
        for (int i = 0; i < comboCollider.Length; i++)
        {
            comboCollider[i].SetActive(false);
        }

        // Effect Off
        airRushEffect.SetActive(false);

        // Sound
        sound.SoundPlay_public(Enemy_Sound.PublicSound.Die);

        // Animation
        anim.SetTrigger("Die");
        anim.SetBool("isDie", true);

        // Delay
        while (anim.GetBool("isDie"))
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
        yield return new WaitForSeconds(0.25f);
        Destroy(container);
    }
}
