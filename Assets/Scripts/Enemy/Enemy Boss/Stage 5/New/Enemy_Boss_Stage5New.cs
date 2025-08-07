using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#region 구조체
[System.Serializable]
public class Attack_Data
{
    [SerializeField] private string attackName;
    public GameObject[] warring;
    public GameObject[] attackCollider;
    public GameObject[] attackVFX;
}

[System.Serializable]
public class Cycle_Data // -> 가중치 렌덤 꼭 필요한가? -> 확률 다르게 설정하려면 필요한듯...
{
    [SerializeField] private string cycleName;
    public int cycle_Index;
    public int curWeight;
    public Vector2Int minMaxWeight;
    public int[] pattenList;

    public void Weight_Setting(int weight)
    {
        curWeight += weight;

        if(curWeight <= minMaxWeight.x)
        {
            curWeight = minMaxWeight.x;
        }

        if(curWeight >= minMaxWeight.y)
        {
            curWeight = minMaxWeight.y;
        }
    }
}
#endregion

public class Enemy_Boss_Stage5New : Enemy_Base
{
    [Header("--- State ---")]
    [SerializeField] private bool isPhase2;
    [SerializeField] private bool isCycle;
    [SerializeField] private bool isWall;
    [SerializeField] private int phase2Hp;


    [Header("--- Component ---")]
    [SerializeField] private Stage_Manager stage_Manager;
    [SerializeField] private GameObject container;
    private LineRenderer line;


    [Header("--- Attack Setting ---")]
    private int attackCount;
    [SerializeField] private int maxAttackCount;
    [SerializeField] private Attack_Data[] attackData;
    [SerializeField] private Cycle_Data[] cycleData_Phase1;
    [SerializeField] private Cycle_Data[] cycleData_Phase2;
    [SerializeField] private Enemy_Boss_PhaseChange[] changeBoss;
    private delegate IEnumerator patten_Delegate();
    private patten_Delegate[] patten;


    [Header("--- VFX ---")]
    public CanvasGroup fade;
    public GameObject explosion_Prefab;
    [SerializeField] private GameObject spawnVFX;

    [Header("---Pos Setting---")]
    [SerializeField] private Transform playerMovePos;
    [SerializeField] private Transform[] teleportSet;
    [SerializeField] private Transform[] teleportExplosionPosL;
    [SerializeField] private Transform[] teleportExplosionPosR;
    [SerializeField] private Transform comboMovePos;
    [SerializeField] private Transform rcrPos;
    [SerializeField] private Transform backstepPos;
    [SerializeField] private Transform upperComboPos;
    [SerializeField] private Transform[] upperComboExplosionPosL;
    [SerializeField] private Transform[] upperComboExplosionPosR;
    [SerializeField] private Transform[] sweepingSet;
    [SerializeField] private Transform[] halfmoonSet;
    [SerializeField] private Transform[] superHalfmoonSet;


    // 0 : 텔포
    // 1 : 콤보
    // 2 : 러쉬
    // 3 : 그라운드 슬래쉬
    // 4 : 백스탭 슬래쉬
    // 5 : 어퍼콤보
    // 6 : 스위핑 슬래쉬
    // 7 : 하프 문 슬래쉬
    // 8 : 슈퍼 하프 문 슬래쉬


    private void Start()
    {
        // 컴포넌트 셋팅
        line = GetComponent<LineRenderer>();

        // 델리게이트 셋팅
        patten = new patten_Delegate[9] 
        { Teleport, Combo, Rush, GroundSlash, BackstepSalsh, UpperCombo, SweepingSlash, HalfMoonSlash, SuperHalfMoon };

        // 스텟 셋팅
        Status_Setting();

        // 스폰 동작
        Spawn();
    }

    private void Update()
    {
        if(state == State.Spawn || state == State.Die)
        {
            return;
        }

        WallCheck();
        GroundCheck();

        // Find Target & Reset Enemy
        if (!haveTarget && state == State.Idle)
        {
            Target_Setting();
            if (!haveTarget && state != State.Await)
            {
                state = State.Await;
            }
        }

        if (state == State.Idle && !isCycle)
        {
            Think();
        }
    }

    private void Think()
    {
        state = State.Think;

        // Phase2 전환
        if (hp <= phase2Hp && !isPhase2)
        {
            hitStopCoroutine = StartCoroutine(Phase2());
        }
        else
        {
            Attack_Think();
        }
    }

    private void Attack_Think()
    {
        // 타겟 셋팅
        Target_Setting();

        if (attackCount >= maxAttackCount)
        {
            // 강화 패턴
            StartCoroutine(isPhase2 ? Phase2_Enhance() : Phase1_Enhance());
        }
        else
        {
            // 일반 패턴
            if (targetDir <= 5)
            {
                // 근거리 공격
                var list = isPhase2 ? cycleData_Phase2 : cycleData_Phase1;
                StartCoroutine(Phase_Cycle(list[WeightCal(list, 0, 2)]));
            }
            else
            {
                // 원거리 공격
                var list = isPhase2 ? cycleData_Phase2 : cycleData_Phase1;
                StartCoroutine(Phase_Cycle(list[WeightCal(list, 3, 6)]));
            }
        }
    }

    private void WallCheck()
    {
        isWall = Physics2D.Raycast(transform.position, new Vector2(transform.localScale.x, 0), 1f, groundLayer);
    }

    #region 공격 동작
    /// <summary>
    /// 패턴 가중치 계산
    /// </summary>
    /// <param name="data">페이즈 데이터</param>
    /// <param name="startIndex">사이클 인덱스 - 시작부분</param>
    /// <param name="endIndex">사이클 인덱스 - 종료부분</param>
    /// <returns></returns>
    private int WeightCal(Cycle_Data[] data, int startIndex, int endIndex)
    {
        // 가중치 토탈 계산
        int total = 0;
        for (int i = startIndex; i < endIndex; i++)
        {
            total += data[i].curWeight;
        }

        // 사이클 선택 계산
        int randomValue = Random.Range(0, total);
        int accumulatedWeight = 0;
        for (int i = startIndex; i < endIndex; i++)
        {
            accumulatedWeight += data[i].curWeight;
            if (randomValue < accumulatedWeight)
            {
                // 가중치 증감
                for (int i1 = startIndex; i1 < endIndex; i1++)
                {
                    data[i1].Weight_Setting(i == i1 ? -10 : 5);
                }

                // 값 전달
                return data[i].cycle_Index;
            }
        }

        // 예외 처리
        return 0;
    }

    /// <summary>
    /// 페이즈 1,2 둘다 사용함!
    /// </summary>
    /// <param name="data">동작할 사이클 데이터</param>
    /// <returns></returns>
    private IEnumerator Phase_Cycle(Cycle_Data data)
    {
        state = State.Attack;
        isCycle = true;
        attackCount++;

        for (int i = 0; i < (data.pattenList.Length); i++)
        {
            yield return ExecuteAction(patten[data.pattenList[i]]());
        }

        state = State.Idle;
        isCycle = false;
    }

    private IEnumerator ExecuteAction(IEnumerator action)
    {
        hitStopCoroutine = StartCoroutine(action);
        while (isAttack)
        {
            yield return null;
        }
    }
    #endregion


    #region 공격 패턴
    /// <summary>
    /// 대상 지정 후 해당위치 폭발 + 텔레포트
    /// </summary>
    /// <returns></returns>
    private IEnumerator Teleport()
    {
        isAttack = true;

        // 비활성화
        isInvincibility = true;
        attackData[0].attackVFX[0].SetActive(true);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);

        // 추적
        attackData[0].warring[0].SetActive(true);
        attackData[0].attackVFX[1].SetActive(true);
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * (isPhase2 ? 2f : 1.5f);
            attackData[0].attackVFX[1].transform.position = curTarget.transform.position;
            teleportSet[0].position = new Vector3(curTarget.transform.position.x, transform.position.y, curTarget.transform.position.z);
            attackData[0].warring[0].transform.position = new Vector3(curTarget.transform.position.x, transform.position.y, curTarget.transform.position.z);
            yield return null;
        }
        attackData[0].attackVFX[1].SetActive(false);

        // 이동
        isInvincibility = false;
        LookAt();
        attackData[0].attackVFX[0].SetActive(true);
        int ran = Random.Range(0, 100);
        transform.position = ran <= 50 ? teleportSet[1].position : teleportSet[2].position;

        // 애니메이션
        if (anim != null)
        {
            anim.SetTrigger("Action");
            anim.SetBool("isTeleprot", true);
        }

        // 지면 폭발 - 3회 정도?
        for (int i = 0; i < teleportExplosionPosR.Length; i++)
        {
            Instantiate(explosion_Prefab, teleportExplosionPosR[i].position, Quaternion.identity);
            Instantiate(explosion_Prefab, teleportExplosionPosL[i].position, Quaternion.identity);
            yield return new WaitForSeconds(0.15f);
        }

        // 애니메이션 대기
        if (anim != null)
        {
            while (anim.GetBool("isTeleprot"))
            {
                yield return null;
            }
        }

        isAttack = false;
    }

    /// <summary>
    /// 일반 3연타 공격 - 페이즈2에서 마지막 공격 검기 발사
    /// </summary>
    /// <returns></returns>
    private IEnumerator Combo()
    {
        isAttack = true;

        // 공격 애니메이션
        if (anim != null)
        {
            LookAt();
            anim.SetTrigger("Action");
            anim.SetBool("isCombo", true);
            anim.SetBool("isAttack", true);
            while (anim.GetBool("isCombo"))
            {
                yield return null;
            }
        }

        isAttack = false;
    }

    /// <summary>
    /// 돌진 공격 - 페이즈2에서 돌진 속도 증가
    /// </summary>
    /// <returns></returns>
    private IEnumerator Rush()
    {
        isAttack = true;

        // 에니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isRushCharge", true);
        anim.SetBool("isRush", true);

        // 딜레이
        attackData[2].warring[0].SetActive(true);
        float timer = isPhase2 ? 0.16f : 0.21f;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            LookAt();
            yield return null;
        }
        anim.SetBool("isRushCharge", false);
        attackData[2].warring[0].SetActive(false);

        // Rush Setting
        Vector3 startPos = transform.position;
        Vector3 endPos = rcrPos.position;
        Vector3 dir = (endPos - startPos).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 7f, groundLayer);
        if (hit)
        {
            Vector3 pos = hit.point;
            pos.x = transform.position.x < 0 ? +0.5f : -0.5f;
            endPos = pos;
        }

        // Rush
        attackData[2].attackCollider[0].SetActive(true);
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * (isPhase2 ? 2.5f : 2f);
            transform.position = Vector3.Lerp(startPos, endPos, Easing.EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetBool("isRush", false);
        attackData[2].attackCollider[0].SetActive(false);

        isAttack = false;
    }

    /// <summary>
    /// 타겟 대상 대쉬 - 지상 난무 - 마지막 2타는 텔레포트 후 공격
    /// </summary>
    /// <returns></returns>
    private IEnumerator GroundSlash()
    {
        isAttack = true;

        // 워닝
        attackData[3].warring[0].SetActive(true);

        // 대쉬
        LookAt();
        Vector3 startPos = transform.position;
        Vector3 endPos = curTarget.transform.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * (isPhase2 ? 2.5f : 2f);
            transform.position = Vector3.Lerp(startPos, endPos, Easing.EasingFunctions.OutExpo(timer));
            yield return null;
        }
        LookAt();
        attackData[3].warring[0].SetActive(false);

        // 난무
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isGroundSlash", true);
        attackData[3].attackCollider[0].SetActive(true);
        yield return new WaitForSeconds(isPhase2 ? 1.55f : 2f);
        attackData[3].attackCollider[0].SetActive(false);
        anim.SetBool("isAttack", false);

        // 텔레포트
        isInvincibility = true;
        attackData[0].attackVFX[0].SetActive(true);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);

        // 추적
        attackData[0].attackVFX[1].SetActive(true);
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * (isPhase2 ? 2f : 1.5f);
            attackData[0].attackVFX[1].transform.position = curTarget.transform.position;
            teleportSet[0].position = new Vector3(curTarget.transform.position.x, transform.position.y, curTarget.transform.position.z);
            yield return null;
        }
        attackData[0].attackVFX[1].SetActive(false);

        // 이동
        LookAt();
        attackData[0].attackVFX[0].SetActive(true);
        int ran = Random.Range(0, 100);
        transform.position = ran <= 50 ? teleportSet[1].position : teleportSet[2].position;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);

        // 워닝 딜레이
        LookAt();
        attackData[3].warring[1].SetActive(true);
        yield return new WaitForSeconds(isPhase2 ? 0.35f : 0.5f);
        attackData[3].warring[1].SetActive(false);

        // 베어내기
        anim.SetTrigger("Action");
        while (anim.GetBool("isGroundSlash"))
        {
            yield return null;
        }

        isAttack = false;
    }

    /// <summary>
    /// 백스탭 + 올려베기 + 폭발 - 페이즈2에선 폭발 마지막에 대형 폭발
    /// </summary>
    /// <returns></returns>
    private IEnumerator BackstepSalsh()
    {
        isAttack = true;

        attackData[4].warring[0].SetActive(true);

        // 이동
        LookAt();
        Vector3 startPos = transform.position;
        Vector3 endPos = backstepPos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * (isPhase2 ? 2.25f : 1.75f);
            transform.position = Vector3.Lerp(startPos, endPos, Easing.EasingFunctions.OutExpo(timer));
            yield return null;
        }
        attackData[4].warring[0].SetActive(false);

        // 공격
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isBackstepSlash", true);
        while (anim.GetBool("isBackstepSlash"))
        {
            yield return null;
        }

        isAttack = false;
    }

    /// <summary>
    /// 올려베기 + 공중난무 + 내려찍기 - 내려찍는 위치에 폭발 - 페이즈 2에선 타겟 위치로 돌진 후 올려베기
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpperCombo()
    {
        isAttack = true;

        // 페이즈 2 타겟 추적
        attackData[5].warring[0].SetActive(true); // -> 워닝 끄는건 애니메이션 이벤트로!
        if (isPhase2)
        {
            LookAt();
            Vector3 startPos = transform.position;
            Vector3 endPos = curTarget.transform.position;
            endPos.x += transform.localScale.x < 0 ? -2f : 2f;
            float timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime * 1.55f;
                transform.position = Vector3.Lerp(startPos, endPos, Easing.EasingFunctions.OutExpo(timer));
                yield return null;
            }
        }

        // 올려베기 - 공격에서 워닝 Off
        LookAt();
        anim.SetTrigger("Action");
        anim.SetBool("isUpper", true);
        anim.SetBool("isUpperAir", true);
        anim.SetBool("isUpperStrike", true);
        while (anim.GetBool("isUpper"))
        {
            yield return null;
        }

        // 공중 난무 - 사라지기
        isInvincibility = true;
        attackData[5].warring[1].SetActive(true);
        anim.SetTrigger("Action");
        float a = 1;
        while (a > 0)
        {
            a -= Time.deltaTime * (isPhase2 ? 2.1f : 1.7f);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, a);
            yield return null;
        }
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        attackData[5].warring[1].SetActive(false);

        // 공중 난무 - 공격
        attackData[5].attackVFX[0].SetActive(true);
        yield return new WaitForSeconds(isPhase2 ? 1.5f : 1.7f);
        attackData[5].attackVFX[0].SetActive(false);

        // 공중 난무 - 등장
        rigid.velocity = Vector2.zero;
        rigid.gravityScale = 0;
        transform.position = upperComboPos.position;
        a = 0;
        while (a < 1)
        {
            a += Time.deltaTime * (isPhase2 ? 2.1f : 1.7f);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, a);
            yield return null;
        }
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        isInvincibility = false;

        // 내려찍기 - 조준
        Vector3 targetDir = Vector3.zero;
        line.enabled = true;
        line.SetPosition(0, transform.position);
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * (isPhase2 ? 1.75f : 1.5f);
            targetDir = curTarget.transform.position - transform.position;
            line.SetPosition(1, curTarget.transform.position);
            yield return null;
        }
        line.enabled = false;

        yield return new WaitForSeconds(isPhase2 ? 0.12f : 0.17f);

        // 내려찍기 - 이동
        anim.SetTrigger("Action");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDir.normalized, 100, groundLayer);
        Vector3 sPos = transform.position;
        Vector3 ePos = hit.point;
        ePos.x += transform.localScale.x < 0 ? -0.5f : 0.5f;
        ePos.y += 0.2f;
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * (isPhase2 ? 1.67f : 1.42f);
            transform.position = Vector3.Lerp(sPos, ePos, Easing.EasingFunctions.OutExpo(t));
            yield return null;
        }
        rigid.velocity = Vector2.zero;
        rigid.gravityScale = 0;

        // 내려찍기 - 폭발
        for (int i = 0; i < upperComboExplosionPosL.Length; i++)
        {
            Instantiate(explosion_Prefab, upperComboExplosionPosL[i].position, Quaternion.identity);
            Instantiate(explosion_Prefab, upperComboExplosionPosR[i].position, Quaternion.identity);
            yield return new WaitForSeconds(isPhase2 ? 0.13f : 0.15f);
        }

        // 애니메이션 대기
        while (anim.GetBool("isUpperStrike"))
        {
            yield return null;
        }

        isAttack = false;
    }

    /// <summary>
    /// 차지 + 범위 베기 - 페이즈2에선 돌진 후 추가베기 1회
    /// </summary>
    /// <returns></returns>
    private IEnumerator SweepingSlash()
    {
        isAttack = true;

        // 차징 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isSweepingCharge", true);
        anim.SetBool("isSweepingAttack", true);

        // 차지 - 워닝 표시
        attackData[6].warring[0].SetActive(true);
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * (isPhase2 ? 1.65f : 1f);
            attackData[6].warring[0].transform.position = curTarget.transform.position;
            sweepingSet[0].transform.position = curTarget.transform.position;
            yield return null;
        }
        attackData[6].warring[0].SetActive(false);

        // 텔레포트
        int ran = Random.Range(0, 100);
        Vector3 startPos = ran <= 50 ? sweepingSet[1].position : sweepingSet[2].position;
        Vector3 endPos = ran <= 50 ? sweepingSet[2].position : sweepingSet[1].position;
        transform.position = startPos;

        // 딜레이
        yield return new WaitForSeconds(isPhase2 ? 0.22f : 0.31f);
        anim.SetBool("isSweepingCharge", false);

        // 범위 베기 x 2회
        for (int i = 0; i < 2; i++)
        {
            timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime * (isPhase2 ? 1.77f : 1.54f);
                transform.position = i == 0
                    ? Vector3.Lerp(startPos, endPos, Easing.EasingFunctions.OutExpo(timer))
                    : Vector3.Lerp(endPos, startPos, Easing.EasingFunctions.OutExpo(timer));
                yield return null;
            }
        }

        // 종료 애니메이션
        anim.SetBool("isAttack", false);
        while (anim.GetBool("isSweepingAttack"))
        {
            yield return null;
        }

        isAttack = false;
    }

    /// <summary>
    ///  텔레포트 + 반원베기 + 내려찍기 - 페이즈2에선 반원베기에 탄막 추가
    /// </summary>
    /// <returns></returns>
    private IEnumerator HalfMoonSlash()
    {
        isAttack = true;

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isHaifmoonCharge", true);
        anim.SetBool("isHaifmoonSlash", true);
        anim.SetBool("isHaifmoonStrike", true);

        // 텔레포트
        transform.position = new Vector3(curTarget.transform.position.x, transform.position.y + 10f, curTarget.transform.position.z);
        attackData[0].attackVFX[0].SetActive(true);

        // 딜레이
        LookAt();
        attackData[7].warring[0].SetActive(true);
        yield return new WaitForSeconds(isPhase2 ? Random.Range(0.12f, 0.156f) : Random.Range(0.15f, 0.22f));
        attackData[7].warring[0].SetActive(false);

        // 반원베기
        anim.SetBool("isHaifmoonCharge", false);
        while (anim.GetBool("isHaifmoonSlash"))
        {
            yield return null;
        }

        // 내려찍기
        LookAt();
        yield return new WaitForSeconds(isPhase2 ? Random.Range(0.12f, 0.156f) : Random.Range(0.15f, 0.22f));
        halfmoonSet[0].transform.position = curTarget.transform.position;
        anim.SetTrigger("Action");

        // 이동
        Vector3 startPos = transform.position;
        Vector3 endPos = halfmoonSet[Random.Range(0, 1)].position;
        endPos.y += -0.1f;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * (isPhase2 ? 1.77f : 1.56f);
            transform.position = Vector3.Lerp(startPos, endPos, Easing.EasingFunctions.OutExpo(timer));
            yield return null;
        }

        // 애니메이션 대기
        while (anim.GetBool("isHaifmoonStrike"))
        {
            yield return null;
        }

        isAttack = false;
    }

    /// <summary>
    /// 각각 좌 우 하프문 공격 1회 + 중앙 슈퍼문 공격 1회 + 매 공격 당 반원 탄막
    /// </summary>
    /// <returns></returns>
    private IEnumerator SuperHalfMoon()
    {
        isAttack = true;
        anim.SetInteger("SuperCount", 0);

        // 좌,우 2회 베기 - 이동 위치는 렌덤 + 일반 하프문 + 탄막
        int ran = Random.Range(0, 100);
        int[] p = ran <= 50 ? new int[3] { 0, 1, 2 } : new int[3] { 1, 0, 2 };
        for (int i = 0; i < 2; i++)
        {
            // 이동
            transform.position = superHalfmoonSet[p[i]].position;

            // 차지 애니메이션
            anim.SetTrigger("Action");
            anim.SetInteger("SuperCount", i);
            anim.SetBool("isSuperHaifmoonSlash", true);
            anim.SetBool("isSuperHaifmoonCharge", true);

            // 공격 딜레이
            attackData[8].warring[0].SetActive(true);
            yield return new WaitForSeconds(0.17f - i < 0 ? (i * 0.03f) : 0);
            attackData[8].warring[0].SetActive(false);

            // 애니메이션 대기
            anim.SetBool("isSuperHaifmoonCharge", false);
            while (anim.GetBool("isSuperHaifmoonSlash"))
            {
                yield return null;
            }
        }

        // 최종 베기 - 대형 하프문 + 탄막
        transform.position = superHalfmoonSet[p[2]].position;
        attackData[8].warring[1].SetActive(true);

        anim.SetTrigger("Action");
        anim.SetFloat("SuperCount", 2);
        anim.SetBool("isSuperHaifmoonSlash", true);
        anim.SetBool("isSuperHaifmoonCharge", true);

        // 공격 딜레이
        yield return new WaitForSeconds(0.19f);
        attackData[8].warring[1].SetActive(false);

        // 애니메이션 대기
        anim.SetBool("isSuperHaifmoonCharge", false);
        while (anim.GetBool("isSuperHaifmoonSlash"))
        {
            yield return null;
        }
        isAttack = false;
    }

    /// <summary>
    /// 러쉬 - 백스탭 - 스위핑 - 어퍼콤보 - 스위핑 - 하프문
    /// </summary>
    /// <returns></returns>
    private IEnumerator Phase1_Enhance()
    {
        isCycle = true;
        attackCount++;
        IEnumerator[] attack = new IEnumerator[6]
        { Rush(), BackstepSalsh(), SweepingSlash(), UpperCombo(), SweepingSlash(), HalfMoonSlash() };

        for (int i = 0; i < attack.Length; i++)
        {
            yield return ExecuteAction(attack[i]);
        }
        isCycle = false;
    }

    /// <summary>
    /// 러쉬 - 스위핑 - 어퍼콤보 - 스위핑 - 백스탭 - 슈퍼하프문
    /// </summary>
    /// <returns></returns>
    private IEnumerator Phase2_Enhance()
    {
        isCycle = true;
        attackCount++;
        IEnumerator[] attack = new IEnumerator[6]
        { Rush(), SweepingSlash(), UpperCombo(), SweepingSlash(), BackstepSalsh(), SuperHalfMoon() };

        for (int i = 0; i < attack.Length; i++)
        {
            yield return ExecuteAction(attack[i]);
        }
        isCycle = false;
    }
    #endregion


    #region 스폰 / 페이즈2 / 기절 / 사망
    protected override void Spawn()
    {
        StartCoroutine(SpawnCall());
    }

    private IEnumerator SpawnCall()
    {
        state = State.Spawn;
        isInvincibility = true;

        // Fade
        statusUI_Boss.Fade();
        yield return new WaitForSeconds(1.35f);

        // Player Move
        GameObject obj = GameObject.Find("Player");
        obj.transform.position = playerMovePos.position;

        // Fade Wait
        while (statusUI_Boss.isFade)
        {
            yield return null;
        }

        // Spawn UI
        statusUI_Boss.StartNameFadeCall();

        // Sound
        // sound.SoundPlay_public(Enemy_Sound.PublicSound.Spawn);

        // Spawn VFX
        spawnVFX.SetActive(true);
        while(spawnVFX.activeSelf)
        {
            yield return null;
        }

        state = State.Idle;
        isInvincibility = false;
    }

    private IEnumerator Phase2()
    {
        state = State.Groggy;
        isInvincibility = true;
        isPhase2 = true;

        // 보스 공격 - 러쉬 - 어퍼콤보 - 하프문
        yield return ExecuteAction(Rush());
        yield return ExecuteAction(UpperCombo());
        yield return ExecuteAction(HalfMoonSlash());

        // 보스 비활성화? 암전?
        BossFade(true);

        // 암전
        fade.gameObject.SetActive(true);
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 0.55f;
            fade.alpha = timer;
            yield return null;
        }
        fade.alpha = 1;

        // 딜레이
        yield return new WaitForSeconds(0.22f);

        // 시작 공격 - 페이즈 1 보스 / AOE + 스위핑 - 어퍼콤보 - 스위핑
        changeBoss[0].Use();
        float[] t = new float[3] { 1.5f, 0.45f, 1.3f };
        IEnumerator[] attack = new IEnumerator[3] { SweepingSlash(), UpperCombo(), SweepingSlash() };
        for (int i = 0; i < attack.Length; i++)
        {
            BossFade(false);
            yield return ExecuteAction(attack[i]);
            BossFade(true);
            yield return new WaitForSeconds(t[i]);
        }

        // 딜레이
        yield return new WaitForSeconds(0.15f);

        // 중간 공격 - 페이즈 2 보스 / 에어러쉬 - 어퍼콤보 - 에어러쉬
        changeBoss[0].Use();
        t = new float[3] { 1.4f, 0.32f, 1.2f };
        attack = new IEnumerator[3] { SweepingSlash(), UpperCombo(), SweepingSlash() };
        for (int i = 0; i < attack.Length; i++)
        {
            BossFade(false);
            yield return ExecuteAction(attack[i]);
            BossFade(true);
            yield return new WaitForSeconds(t[i]);
        }

        // 딜레이
        yield return new WaitForSeconds(0.13f);

        // 종료 공격 - 슈퍼 하프문
        BossFade(false);
        yield return ExecuteAction(HalfMoonSlash());

        // 암전 해제
        fade.alpha = 0;
        fade.gameObject.SetActive(false);

        state = State.Idle;
        isInvincibility = false;
    }

    private void BossFade(bool isON)
    {
        // 보스 비활성화? 암전?
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, isON ? 0 : 1);
    }

    protected override void Stagger()
    {

    }

    public override void Die()
    {
        if(hitStopCoroutine != null)
        {
            StopCoroutine(hitStopCoroutine);
        }

        StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        state = State.Die;
        isDie = true;

        // 애니메이션 리셋
        anim.ResetTrigger("Action");
        for (int i = 0; i < animationbool.Length; i++)
        {
            anim.SetBool(animationbool[i], false);
        }

        // 사망 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isDie", true);
        while(anim.GetBool("isDie"))
        {
            yield return null;
        }

        // UI Call
        stage_Manager.Stage_Clear();
        while (stage_Manager.isUI)
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.25f);
        Destroy(container);
    }
    #endregion
}
