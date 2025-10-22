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
            // �ʻ� ����
            attackCount = 0;

            if (hitStopCoroutine != null) StopCoroutine(hitStopCoroutine);
            hitStopCoroutine = StartCoroutine(Attack(GetPatten(special_PattenList)));
        }
        else
        {
            // �Ϲ� ����
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
            // ���� ȣ��
            int index = data.pattenCount[i];
            attack[index].Use();
            yield return new WaitWhile(() => attack[index].isUsed);

            // ���� ���� ������
            yield return new WaitForSeconds(0.15f);
        }

        // �Ǵ� ������
        state = State.Idle;
        yield return new WaitForSeconds(delay);

        // �Ǵ�
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
    /// �ٵ� Ȱ��ȭ & ��Ȱ��ȭ
    /// </summary>
    /// <param name="isOn"></param>
    public void Body_Setting(bool isOn)
    {
        // body.SetActive(isOn);
        bodySprite.color = new Color(1, 1, 1, isOn ? 1 : 0);
    }

    /// <summary>
    /// �߷� ���� Ȱ��ȭ & ��Ȱ��ȭ
    /// </summary>
    /// <param name="isOn"></param>
    public void Rigid_Setting(bool isOn)
    {
        rigid.gravityScale = isOn ? 1 : 0;
    }


    #region ���� & ���
    protected override void Spawn()
    {
        movementCoroutine = StartCoroutine(SpawnCall());
    }

    private IEnumerator SpawnCall()
    {
        state = State.Spawn;

        // �⺻ ����
        Target_Setting();
        Status_Setting();

        // Spawn UI
        statusUI_Boss.StartFadeCall();

        // Sound
        sound.SoundPlay_public(Enemy_Sound.PublicSound.Spawn);

        // ��ȯ �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isSpawn", true);
        yield return new WaitWhile(() => anim.GetBool("isSpawn"));

        // UI Wait
        yield return new WaitWhile(() => statusUI_Boss.isFade);

        // ����
        state = State.Idle;
        Think();
    }

    protected override void Stagger()
    {
        // ���� ����
    }

    public override void Die()
    {
        if (hitStopCoroutine != null) StopCoroutine(hitStopCoroutine);
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);
        StopAllCoroutines();

        // ���� ����
        foreach (Attack_Base attack in attack)
        {
            attack.Reset();
        }

        // �ִϸ��̼� ����
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

        // ���̵� ��
        ((Attack_HaifMoon)attack[5]).BackgroundFade(true, 1f);

        // ��� ����
        dieMovePosCollider.transform.parent = null;
        for (int i = 0; i < 15; i++)
        {
            // ���� �̵� & ����
            Vector3 pos = GetPos();
            transform.position = pos;
            Instantiate(dieVFX[0], pos, Quaternion.identity);
            ShakeEffect(0.1f, Random.Range(0.3f, 1.5f));
            LookAt();

            // ��� �ִϸ��̼�
            anim.SetTrigger("Action");
            anim.SetBool("isDie", true);
            yield return new WaitWhile(() => anim.GetBool("isDie"));

            // ���� �̵� ������
            yield return new WaitForSeconds(0.25f - (i * 0.1f));
        }

        // �߾� ���� ����
        transform.position = dieExplosionEndPos.position;

        // ������
        ShakeEffect(1.5f, 1.5f);
        yield return new WaitForSeconds(1.5f);

        // ��� �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isDie", true);
        Instantiate(dieVFX[1], dieExplosionEndPos.position, Quaternion.identity);
        ShakeEffect(2f, 0.5f);
        yield return new WaitWhile(() => anim.GetBool("isDie"));
        Body_Setting(false);

        // ���̵� �ƿ�
        ((Attack_HaifMoon)attack[5]).BackgroundFade(false, 0f);

        yield return new WaitForSeconds(1.5f);

        // Destroy
        Destroy(container);
    }

    private Vector2 GetPos()
    {
        Vector2 originPosition = dieMovePosCollider.transform.position;

        // �ݶ��̴��� ����� �������� bound.size ���
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
