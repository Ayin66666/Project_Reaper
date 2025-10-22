using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class PattenData
{
    public string pattenName;
    public int pattenWeight;
    public List<int> pattenCount;
}


public class Enemy_Boss5_New : Enemy_Base
{
    [Header("---State---")]
    [SerializeField] private int attackCount;


    [Header("---Attack---")]
    [SerializeField] private List<PattenData> normal_pattenList;
    [SerializeField] private List<PattenData> special_PattenList;
    [SerializeField] private List<Attack_Base> attack;


    [Header("---Object---")]
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject body;
    [SerializeField] private SpriteRenderer bodySprite;
    private Coroutine movementCoroutine;


    [Header("---Die VFX---")]
    [SerializeField] private GameObject[] dieVFX;
    [SerializeField] private Transform dieExplosionEndPos;
    [SerializeField] private BoxCollider2D dieMovePosCollider;


    private void Start()
    {
        Spawn();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TakeDamage(gameObject, 8888, 1, true, HitType.None, 0, transform.position);
        }
    }

    private void Think()
    {
        state = State.Think;

        if (state == State.Die) return;
        CurTarget_Check();
        if (attackCount >= 3)
        {
            // 필살 패턴
            attackCount = 0;

            if (hitStopCoroutine != null) StopCoroutine(hitStopCoroutine);
            hitStopCoroutine = StartCoroutine(Attack(GetPatten(special_PattenList)));
        }
        else
        {
            // 일반 패턴
            attackCount++;

            if (hitStopCoroutine != null) StopCoroutine(hitStopCoroutine);
            hitStopCoroutine = StartCoroutine(Attack(GetPatten(normal_pattenList)));
        }
    }

    private IEnumerator Attack(PattenData data)
    {
        state = State.Attack;
        for (int i = 0; i < data.pattenCount.Count; i++)
        {
            // 공격 호출
            int index = data.pattenCount[i];
            attack[index].Use();
            yield return new WaitWhile(() => attack[index].isUsed);

            // 다음 공격 딜레이
            yield return new WaitForSeconds(0.15f);
        }

        // 판단 딜레이
        state = State.Idle;
        yield return new WaitForSeconds(delay);

        // 판단
        Think();
    }

    private PattenData GetPatten(List<PattenData> data)
    {
        if (data == null || data.Count == 0) return null;

        int totalWeight = 0;
        foreach (var p in data)
        {
            totalWeight += Mathf.Max(0, p.pattenWeight);
        }

        if (totalWeight <= 0) return null;
        int randomValue = Random.Range(0, totalWeight);
        int cumulative = 0;
        foreach (var p in data)
        {
            cumulative += Mathf.Max(0, p.pattenWeight);
            if (randomValue < cumulative) return p;
        }

        return data[data.Count - 1];
    }


    /// <summary>
    /// 바디 활성화 & 비활성화
    /// </summary>
    /// <param name="isOn"></param>
    public void Body_Setting(bool isOn)
    {
        // body.SetActive(isOn);
        bodySprite.color = new Color(1, 1, 1, isOn ? 1 : 0);
    }

    /// <summary>
    /// 중력 설정 활성화 & 비활성화
    /// </summary>
    /// <param name="isOn"></param>
    public void Rigid_Setting(bool isOn)
    {
        rigid.gravityScale = isOn ? 1 : 0;
    }


    #region 스폰 & 사망
    protected override void Spawn()
    {
        movementCoroutine = StartCoroutine(SpawnCall());
    }

    private IEnumerator SpawnCall()
    {
        state = State.Spawn;

        // 기본 셋팅
        Target_Setting();
        Status_Setting();

        // Spawn UI
        statusUI_Boss.StartFadeCall();

        // Sound
        sound.SoundPlay_public(Enemy_Sound.PublicSound.Spawn);

        // 소환 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isSpawn", true);
        yield return new WaitWhile(() => anim.GetBool("isSpawn"));

        // UI Wait
        yield return new WaitWhile(() => statusUI_Boss.isFade);

        // 동작
        state = State.Idle;
        Think();
    }

    protected override void Stagger()
    {
        // 동작 없음
    }

    public override void Die()
    {
        if (hitStopCoroutine != null) StopCoroutine(hitStopCoroutine);
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);
        StopAllCoroutines();

        // 공격 종료
        foreach (Attack_Base attack in attack)
        {
            attack.Reset();
        }

        // 애니메이션 종료
        foreach (string s in animationTrigger)
        {
            anim.ResetTrigger(s);
        }

        foreach (string s in animationbool)
        {
            anim.SetBool(s, false);
        }

        Body_Setting(true);
        Rigid_Setting(true);

        hitStopCoroutine = StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        state = State.Die;
        isInvincibility = true;
        Rigid_Setting(false);

        // UI Off
        statusUI_Boss.Die();

        // 페이드 인
        ((Attack_HaifMoon)attack[5]).BackgroundFade(true, 1f);

        // 사망 폭발
        dieMovePosCollider.transform.parent = null;
        for (int i = 0; i < 15; i++)
        {
            // 보스 이동 & 폭발
            Vector3 pos = GetPos();
            transform.position = pos;
            Instantiate(dieVFX[0], pos, Quaternion.identity);
            ShakeEffect(0.1f, Random.Range(0.3f, 1.5f));
            LookAt();

            // 사망 애니메이션
            anim.SetTrigger("Action");
            anim.SetBool("isDie", true);
            yield return new WaitWhile(() => anim.GetBool("isDie"));

            // 다음 이동 딜레이
            yield return new WaitForSeconds(0.25f - (i * 0.1f));
        }

        // 중앙 대형 폭발
        transform.position = dieExplosionEndPos.position;

        // 딜레이
        ShakeEffect(1.5f, 1.5f);
        yield return new WaitForSeconds(1.5f);

        // 사망 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isDie", true);
        Instantiate(dieVFX[1], dieExplosionEndPos.position, Quaternion.identity);
        ShakeEffect(2f, 0.5f);
        yield return new WaitWhile(() => anim.GetBool("isDie"));
        Body_Setting(false);

        // 페이드 아웃
        ((Attack_HaifMoon)attack[5]).BackgroundFade(false, 0f);

        yield return new WaitForSeconds(1.5f);

        // Destroy
        Destroy(container);
    }

    private Vector2 GetPos()
    {
        Vector2 originPosition = dieMovePosCollider.transform.position;

        // 콜라이더의 사이즈를 가져오는 bound.size 사용
        float range_X = dieMovePosCollider.bounds.size.x;
        float range_Y = dieMovePosCollider.bounds.size.y;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Y = Random.Range((range_Y / 2) * -1, range_Y / 2);
        Vector2 RandomPostion = new Vector2(range_X, range_Y);

        Vector2 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }
    #endregion
}
