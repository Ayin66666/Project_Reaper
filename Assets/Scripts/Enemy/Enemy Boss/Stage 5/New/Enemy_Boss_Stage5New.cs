using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#region ����ü
[System.Serializable]
public class Attack_Data
{
    [SerializeField] private string attackName;
    public GameObject[] warring;
    public GameObject[] attackCollider;
    public GameObject[] attackVFX;
}

[System.Serializable]
public class Cycle_Data // -> ����ġ ���� �� �ʿ��Ѱ�? -> Ȯ�� �ٸ��� �����Ϸ��� �ʿ��ѵ�...
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


    // 0 : ����
    // 1 : �޺�
    // 2 : ����
    // 3 : �׶��� ������
    // 4 : �齺�� ������
    // 5 : �����޺�
    // 6 : ������ ������
    // 7 : ���� �� ������
    // 8 : ���� ���� �� ������


    private void Start()
    {
        // ������Ʈ ����
        line = GetComponent<LineRenderer>();

        // ��������Ʈ ����
        patten = new patten_Delegate[9] 
        { Teleport, Combo, Rush, GroundSlash, BackstepSalsh, UpperCombo, SweepingSlash, HalfMoonSlash, SuperHalfMoon };

        // ���� ����
        Status_Setting();

        // ���� ����
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

        // Phase2 ��ȯ
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
        // Ÿ�� ����
        Target_Setting();

        if (attackCount >= maxAttackCount)
        {
            // ��ȭ ����
            StartCoroutine(isPhase2 ? Phase2_Enhance() : Phase1_Enhance());
        }
        else
        {
            // �Ϲ� ����
            if (targetDir <= 5)
            {
                // �ٰŸ� ����
                var list = isPhase2 ? cycleData_Phase2 : cycleData_Phase1;
                StartCoroutine(Phase_Cycle(list[WeightCal(list, 0, 2)]));
            }
            else
            {
                // ���Ÿ� ����
                var list = isPhase2 ? cycleData_Phase2 : cycleData_Phase1;
                StartCoroutine(Phase_Cycle(list[WeightCal(list, 3, 6)]));
            }
        }
    }

    private void WallCheck()
    {
        isWall = Physics2D.Raycast(transform.position, new Vector2(transform.localScale.x, 0), 1f, groundLayer);
    }

    #region ���� ����
    /// <summary>
    /// ���� ����ġ ���
    /// </summary>
    /// <param name="data">������ ������</param>
    /// <param name="startIndex">����Ŭ �ε��� - ���ۺκ�</param>
    /// <param name="endIndex">����Ŭ �ε��� - ����κ�</param>
    /// <returns></returns>
    private int WeightCal(Cycle_Data[] data, int startIndex, int endIndex)
    {
        // ����ġ ��Ż ���
        int total = 0;
        for (int i = startIndex; i < endIndex; i++)
        {
            total += data[i].curWeight;
        }

        // ����Ŭ ���� ���
        int randomValue = Random.Range(0, total);
        int accumulatedWeight = 0;
        for (int i = startIndex; i < endIndex; i++)
        {
            accumulatedWeight += data[i].curWeight;
            if (randomValue < accumulatedWeight)
            {
                // ����ġ ����
                for (int i1 = startIndex; i1 < endIndex; i1++)
                {
                    data[i1].Weight_Setting(i == i1 ? -10 : 5);
                }

                // �� ����
                return data[i].cycle_Index;
            }
        }

        // ���� ó��
        return 0;
    }

    /// <summary>
    /// ������ 1,2 �Ѵ� �����!
    /// </summary>
    /// <param name="data">������ ����Ŭ ������</param>
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


    #region ���� ����
    /// <summary>
    /// ��� ���� �� �ش���ġ ���� + �ڷ���Ʈ
    /// </summary>
    /// <returns></returns>
    private IEnumerator Teleport()
    {
        isAttack = true;

        // ��Ȱ��ȭ
        isInvincibility = true;
        attackData[0].attackVFX[0].SetActive(true);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);

        // ����
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

        // �̵�
        isInvincibility = false;
        LookAt();
        attackData[0].attackVFX[0].SetActive(true);
        int ran = Random.Range(0, 100);
        transform.position = ran <= 50 ? teleportSet[1].position : teleportSet[2].position;

        // �ִϸ��̼�
        if (anim != null)
        {
            anim.SetTrigger("Action");
            anim.SetBool("isTeleprot", true);
        }

        // ���� ���� - 3ȸ ����?
        for (int i = 0; i < teleportExplosionPosR.Length; i++)
        {
            Instantiate(explosion_Prefab, teleportExplosionPosR[i].position, Quaternion.identity);
            Instantiate(explosion_Prefab, teleportExplosionPosL[i].position, Quaternion.identity);
            yield return new WaitForSeconds(0.15f);
        }

        // �ִϸ��̼� ���
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
    /// �Ϲ� 3��Ÿ ���� - ������2���� ������ ���� �˱� �߻�
    /// </summary>
    /// <returns></returns>
    private IEnumerator Combo()
    {
        isAttack = true;

        // ���� �ִϸ��̼�
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
    /// ���� ���� - ������2���� ���� �ӵ� ����
    /// </summary>
    /// <returns></returns>
    private IEnumerator Rush()
    {
        isAttack = true;

        // ���ϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isRushCharge", true);
        anim.SetBool("isRush", true);

        // ������
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
    /// Ÿ�� ��� �뽬 - ���� ���� - ������ 2Ÿ�� �ڷ���Ʈ �� ����
    /// </summary>
    /// <returns></returns>
    private IEnumerator GroundSlash()
    {
        isAttack = true;

        // ����
        attackData[3].warring[0].SetActive(true);

        // �뽬
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

        // ����
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isGroundSlash", true);
        attackData[3].attackCollider[0].SetActive(true);
        yield return new WaitForSeconds(isPhase2 ? 1.55f : 2f);
        attackData[3].attackCollider[0].SetActive(false);
        anim.SetBool("isAttack", false);

        // �ڷ���Ʈ
        isInvincibility = true;
        attackData[0].attackVFX[0].SetActive(true);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);

        // ����
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

        // �̵�
        LookAt();
        attackData[0].attackVFX[0].SetActive(true);
        int ran = Random.Range(0, 100);
        transform.position = ran <= 50 ? teleportSet[1].position : teleportSet[2].position;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);

        // ���� ������
        LookAt();
        attackData[3].warring[1].SetActive(true);
        yield return new WaitForSeconds(isPhase2 ? 0.35f : 0.5f);
        attackData[3].warring[1].SetActive(false);

        // �����
        anim.SetTrigger("Action");
        while (anim.GetBool("isGroundSlash"))
        {
            yield return null;
        }

        isAttack = false;
    }

    /// <summary>
    /// �齺�� + �÷����� + ���� - ������2���� ���� �������� ���� ����
    /// </summary>
    /// <returns></returns>
    private IEnumerator BackstepSalsh()
    {
        isAttack = true;

        attackData[4].warring[0].SetActive(true);

        // �̵�
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

        // ����
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
    /// �÷����� + ���߳��� + ������� - ������� ��ġ�� ���� - ������ 2���� Ÿ�� ��ġ�� ���� �� �÷�����
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpperCombo()
    {
        isAttack = true;

        // ������ 2 Ÿ�� ����
        attackData[5].warring[0].SetActive(true); // -> ���� ���°� �ִϸ��̼� �̺�Ʈ��!
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

        // �÷����� - ���ݿ��� ���� Off
        LookAt();
        anim.SetTrigger("Action");
        anim.SetBool("isUpper", true);
        anim.SetBool("isUpperAir", true);
        anim.SetBool("isUpperStrike", true);
        while (anim.GetBool("isUpper"))
        {
            yield return null;
        }

        // ���� ���� - �������
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

        // ���� ���� - ����
        attackData[5].attackVFX[0].SetActive(true);
        yield return new WaitForSeconds(isPhase2 ? 1.5f : 1.7f);
        attackData[5].attackVFX[0].SetActive(false);

        // ���� ���� - ����
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

        // ������� - ����
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

        // ������� - �̵�
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

        // ������� - ����
        for (int i = 0; i < upperComboExplosionPosL.Length; i++)
        {
            Instantiate(explosion_Prefab, upperComboExplosionPosL[i].position, Quaternion.identity);
            Instantiate(explosion_Prefab, upperComboExplosionPosR[i].position, Quaternion.identity);
            yield return new WaitForSeconds(isPhase2 ? 0.13f : 0.15f);
        }

        // �ִϸ��̼� ���
        while (anim.GetBool("isUpperStrike"))
        {
            yield return null;
        }

        isAttack = false;
    }

    /// <summary>
    /// ���� + ���� ���� - ������2���� ���� �� �߰����� 1ȸ
    /// </summary>
    /// <returns></returns>
    private IEnumerator SweepingSlash()
    {
        isAttack = true;

        // ��¡ �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isSweepingCharge", true);
        anim.SetBool("isSweepingAttack", true);

        // ���� - ���� ǥ��
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

        // �ڷ���Ʈ
        int ran = Random.Range(0, 100);
        Vector3 startPos = ran <= 50 ? sweepingSet[1].position : sweepingSet[2].position;
        Vector3 endPos = ran <= 50 ? sweepingSet[2].position : sweepingSet[1].position;
        transform.position = startPos;

        // ������
        yield return new WaitForSeconds(isPhase2 ? 0.22f : 0.31f);
        anim.SetBool("isSweepingCharge", false);

        // ���� ���� x 2ȸ
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

        // ���� �ִϸ��̼�
        anim.SetBool("isAttack", false);
        while (anim.GetBool("isSweepingAttack"))
        {
            yield return null;
        }

        isAttack = false;
    }

    /// <summary>
    ///  �ڷ���Ʈ + �ݿ����� + ������� - ������2���� �ݿ����⿡ ź�� �߰�
    /// </summary>
    /// <returns></returns>
    private IEnumerator HalfMoonSlash()
    {
        isAttack = true;

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isHaifmoonCharge", true);
        anim.SetBool("isHaifmoonSlash", true);
        anim.SetBool("isHaifmoonStrike", true);

        // �ڷ���Ʈ
        transform.position = new Vector3(curTarget.transform.position.x, transform.position.y + 10f, curTarget.transform.position.z);
        attackData[0].attackVFX[0].SetActive(true);

        // ������
        LookAt();
        attackData[7].warring[0].SetActive(true);
        yield return new WaitForSeconds(isPhase2 ? Random.Range(0.12f, 0.156f) : Random.Range(0.15f, 0.22f));
        attackData[7].warring[0].SetActive(false);

        // �ݿ�����
        anim.SetBool("isHaifmoonCharge", false);
        while (anim.GetBool("isHaifmoonSlash"))
        {
            yield return null;
        }

        // �������
        LookAt();
        yield return new WaitForSeconds(isPhase2 ? Random.Range(0.12f, 0.156f) : Random.Range(0.15f, 0.22f));
        halfmoonSet[0].transform.position = curTarget.transform.position;
        anim.SetTrigger("Action");

        // �̵�
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

        // �ִϸ��̼� ���
        while (anim.GetBool("isHaifmoonStrike"))
        {
            yield return null;
        }

        isAttack = false;
    }

    /// <summary>
    /// ���� �� �� ������ ���� 1ȸ + �߾� ���۹� ���� 1ȸ + �� ���� �� �ݿ� ź��
    /// </summary>
    /// <returns></returns>
    private IEnumerator SuperHalfMoon()
    {
        isAttack = true;
        anim.SetInteger("SuperCount", 0);

        // ��,�� 2ȸ ���� - �̵� ��ġ�� ���� + �Ϲ� ������ + ź��
        int ran = Random.Range(0, 100);
        int[] p = ran <= 50 ? new int[3] { 0, 1, 2 } : new int[3] { 1, 0, 2 };
        for (int i = 0; i < 2; i++)
        {
            // �̵�
            transform.position = superHalfmoonSet[p[i]].position;

            // ���� �ִϸ��̼�
            anim.SetTrigger("Action");
            anim.SetInteger("SuperCount", i);
            anim.SetBool("isSuperHaifmoonSlash", true);
            anim.SetBool("isSuperHaifmoonCharge", true);

            // ���� ������
            attackData[8].warring[0].SetActive(true);
            yield return new WaitForSeconds(0.17f - i < 0 ? (i * 0.03f) : 0);
            attackData[8].warring[0].SetActive(false);

            // �ִϸ��̼� ���
            anim.SetBool("isSuperHaifmoonCharge", false);
            while (anim.GetBool("isSuperHaifmoonSlash"))
            {
                yield return null;
            }
        }

        // ���� ���� - ���� ������ + ź��
        transform.position = superHalfmoonSet[p[2]].position;
        attackData[8].warring[1].SetActive(true);

        anim.SetTrigger("Action");
        anim.SetFloat("SuperCount", 2);
        anim.SetBool("isSuperHaifmoonSlash", true);
        anim.SetBool("isSuperHaifmoonCharge", true);

        // ���� ������
        yield return new WaitForSeconds(0.19f);
        attackData[8].warring[1].SetActive(false);

        // �ִϸ��̼� ���
        anim.SetBool("isSuperHaifmoonCharge", false);
        while (anim.GetBool("isSuperHaifmoonSlash"))
        {
            yield return null;
        }
        isAttack = false;
    }

    /// <summary>
    /// ���� - �齺�� - ������ - �����޺� - ������ - ������
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
    /// ���� - ������ - �����޺� - ������ - �齺�� - ����������
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


    #region ���� / ������2 / ���� / ���
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

        // ���� ���� - ���� - �����޺� - ������
        yield return ExecuteAction(Rush());
        yield return ExecuteAction(UpperCombo());
        yield return ExecuteAction(HalfMoonSlash());

        // ���� ��Ȱ��ȭ? ����?
        BossFade(true);

        // ����
        fade.gameObject.SetActive(true);
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 0.55f;
            fade.alpha = timer;
            yield return null;
        }
        fade.alpha = 1;

        // ������
        yield return new WaitForSeconds(0.22f);

        // ���� ���� - ������ 1 ���� / AOE + ������ - �����޺� - ������
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

        // ������
        yield return new WaitForSeconds(0.15f);

        // �߰� ���� - ������ 2 ���� / ����� - �����޺� - �����
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

        // ������
        yield return new WaitForSeconds(0.13f);

        // ���� ���� - ���� ������
        BossFade(false);
        yield return ExecuteAction(HalfMoonSlash());

        // ���� ����
        fade.alpha = 0;
        fade.gameObject.SetActive(false);

        state = State.Idle;
        isInvincibility = false;
    }

    private void BossFade(bool isON)
    {
        // ���� ��Ȱ��ȭ? ����?
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

        // �ִϸ��̼� ����
        anim.ResetTrigger("Action");
        for (int i = 0; i < animationbool.Length; i++)
        {
            anim.SetBool(animationbool[i], false);
        }

        // ��� �ִϸ��̼�
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
