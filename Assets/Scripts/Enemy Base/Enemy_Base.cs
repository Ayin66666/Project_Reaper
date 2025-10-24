using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing;


public abstract class Enemy_Base : MonoBehaviour
{
    [Header("---Compoment---")]
    [SerializeField] protected Enemy_Boss_StatusUI statusUI_Boss;
    [SerializeField] protected Enemy_Normal_StatusUI statusUI_Normal;
    [SerializeField] protected Collider2D aiCollider;
    [SerializeField] protected Rigidbody2D rigid;
    [SerializeField] protected Animator anim;
    [SerializeField] protected Transform bodyObj;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    public Enemy_StatusDataSO status;
    public Enemy_Sound sound;


    [Header("---State---")]
    public float maxAirborneTimer;
    public float airBorneTimer;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected bool isGround;
    [SerializeField] protected bool isAttack;
    protected bool isDie;
    protected bool isHit;
    protected bool isInvincibility;
    [SerializeField] protected bool canHitEffect;
    [SerializeField] protected bool haveTarget;

    // Hit
    [SerializeField] protected bool isAirBorne;
    [SerializeField] private bool isAirBorneMove;
    protected bool isKnockBack;
    protected bool isDownAttack;

    // Hit Coroutine
    protected Coroutine hitStopCoroutine;
    protected Coroutine hitInvincibilityCoroutine;
    protected Coroutine hitAirborneCoroutine;
    protected Coroutine hitKnockbackCoroutine;
    protected Coroutine hitDownAttackCoroutine;
    protected Coroutine ShakeCorouttine;


    protected enum EnemyType { Normal, Tower, Part, Object, Boss, BossTower }
    [SerializeField] protected EnemyType enemyType;
    public enum State { Spawn, Await, Idle, Think, Move, Attack, Groggy, Die }
    public State state;
    protected enum CurLook { None, Left, Right }
    [SerializeField] protected CurLook curLook;
    public enum HitType { None, Stagger, AirBorne, DownAttack, KnockBack }


    [Header("---Status---")]
    [SerializeField] public int hp;
    [SerializeField] protected float delay;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float chaseTime;
    [HideInInspector] public int damage;
    [HideInInspector] public int criticalChance;
    [HideInInspector] public float criticalMultiplier;
    public string enemyName;
    public string enemySudName;
    protected float attackSpeed;
    protected int score;


    [Header("---Target---")]
    public GameObject curTarget;
    protected Vector2 targetVector;
    protected float targetDir;
    protected List<GameObject> targetList = new List<GameObject>();


    [Header("---Animtion---")]
    [SerializeField] protected string[] animationTrigger;
    [SerializeField] protected string[] animationbool;


    protected void Status_Setting()
    {
        enemyName = status.ObjectName;
        hp = status.Hp;

        damage = status.Damage;
        criticalChance = status.CriticalChance;
        criticalMultiplier = status.CriticalMultiplier;

        delay = status.Delay;
        moveSpeed = status.MoveSpeed;
        attackSpeed = status.AttackSpeed;
        score = status.Score;
        canHitEffect = enemyType == EnemyType.Boss ? false : true;

        switch (enemyType)
        {
            case EnemyType.Normal:
            case EnemyType.Boss:
                anim.SetFloat("AttackSpeed", attackSpeed);
                break;

            case EnemyType.Tower:
            case EnemyType.Part:
            case EnemyType.Object:
            case EnemyType.BossTower:
                break;
        }
    }

    protected void Target_Setting()
    {
        if (targetList.Count <= 0)
        {
            // Find Player
            GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < targets.Length; i++)
            {
                targetList.Add(targets[i]);
            }

            if (targets.Length > 0)
            {
                haveTarget = true;

                // Cal Target Dir / Range
                curTarget = targets[Random.Range(0, targets.Length)];
                targetVector = curTarget.transform.position - transform.position;
                targetDir = targetVector.magnitude;
            }
            else
            {
                Debug.Log("Ÿ���� �����ϴ�!");
                haveTarget = false;
            }
        }
        else
        {
            // List Check
            for (int i = 0; i < targetList.Count; i++)
            {
                if (targetList[i] == null)
                {
                    targetList.RemoveAt(i);
                }
            }

            // Target Setting
            if (targetList.Count > 0)
            {
                int ran = Random.Range(0, targetList.Count);
                curTarget = targetList[ran];
            }
            else
            {
                Debug.Log("Ÿ���� �����ϴ�!");
                haveTarget = false;
            }
        }
    }

    protected void CurTarget_Check()
    {
        if (haveTarget)
        {
            targetVector = curTarget.transform.position - transform.position;
            targetDir = targetVector.magnitude;
        }
    }

    public void LookAt()
    {
        if (haveTarget)
        {
            // �̰� �������� ȣ��Ǹ� �̻��ϰ� �Ĵٺ��� ������ �ִ°� ������ Ȯ���غ���!
            targetVector = curTarget.transform.position - transform.position;
            if (targetVector.x > 0)
            {
                curLook = CurLook.Right;
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
            else if (targetVector.x < 0)
            {
                curLook = CurLook.Left;
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                return;
            }
        }
    }

    public void TakeDamage(GameObject player, int damage, int hitCount, bool isCritcal, HitType hitType, float t, Vector3 pos)
    {
        // Die Check
        if (isDie || isHit || isInvincibility)
        {
            return;
        }

        if (damage > 0)
        {
            // �ǰ� ����
            if (hitInvincibilityCoroutine != null) StopCoroutine(hitInvincibilityCoroutine);
            hitInvincibilityCoroutine = StartCoroutine(HitInvincible());

            // Hit Sound
            sound.SoundPlay_public(Enemy_Sound.PublicSound.Hit);

            if (enemyType == EnemyType.Boss)
            {
                statusUI_Boss.Hp();
            }
            else
            {
                statusUI_Normal.Hp();
                statusUI_Normal.AirBorne();
            }

            // Attack Type Check
            if (canHitEffect) // -> �� �κп� ������ ���� -> ��Ȱ��ȭ �Ǽ� ���� ���Ѵ���?
            {
                if (hitStopCoroutine != null) StopCoroutine(hitStopCoroutine);
                state = State.Groggy;
                isAttack = false;

                switch (hitType)
                {
                    case HitType.None:
                        if (isAirBorne) TimerAdd(t);
                        break;

                    case HitType.Stagger:
                        if (isAirBorne) TimerAdd(t);
                        StopHitMoveCoroutine();
                        break;

                    case HitType.AirBorne:
                        StopHitMoveCoroutine();
                        hitAirborneCoroutine = StartCoroutine(AirBorneMove(pos, t));
                        break;

                    case HitType.DownAttack:
                        StopHitMoveCoroutine();
                        hitDownAttackCoroutine = StartCoroutine(DownAttackMove(t));
                        break;

                    case HitType.KnockBack:
                        if (isAirBorne) TimerAdd(t);
                        StopHitMoveCoroutine();
                        hitKnockbackCoroutine = StartCoroutine(KnockBackMove(player, t));
                        break;
                }
            }

            // Damage cal
            HitEffect(hitCount, damage, isCritcal);
        }
    }

    private void HitStopCoroutine()
    {
        switch (enemyType)
        {
            case EnemyType.Normal:
                if (hitStopCoroutine != null) StopCoroutine(hitStopCoroutine);
                state = State.Groggy;
                isAttack = false;
                break;

            case EnemyType.Tower:
            case EnemyType.Part:
            case EnemyType.Object:
            case EnemyType.Boss:
            case EnemyType.BossTower:
                break;
        }
    }

    private void StopHitMoveCoroutine()
    {
        // �ǰ� �̵� ���� ���� �ǰݵǾ��� �� ���� �̵���� �ʱ�ȭ
        isAirBorneMove = false;
        isDownAttack = false;
        isKnockBack = false;

        // �̰� stopcoroutine ���� �ȵǴ°�����? - �ϴ� bool �� ������ �ڷ�ƾ�� ���ᰡ ����
        /*
        if (hitAirborneCoroutine != null) StopCoroutine(hitAirborneCoroutine);
        if (hitDownAttackCoroutine != null) StopCoroutine(hitDownAttackCoroutine);
        if (hitKnockbackCoroutine != null) StopCoroutine(hitKnockbackCoroutine);
        */
    }

    /// <summary>
    /// �ǰ� �� �ٵ� ��鸲
    /// </summary>
    /// <param name="hitCount"></param>
    /// <param name="damage"></param>
    /// <param name="isCritcal"></param>
    private void HitEffect(int hitCount, int damage, bool isCritcal)
    {
        ShakeEffect(0.1f, Random.Range(0.1f, 0.3f));
        for (int i = 0; i < hitCount; i++)
        {
            // Damage Cal -> Ÿ�� Ƚ����ŭ �������� ������ ���
            int calDamage = (int)(damage / hitCount);
            hp -= calDamage;

            // ������ UI
            switch (enemyType)
            {
                case EnemyType.Normal:
                case EnemyType.Tower:
                case EnemyType.Part:
                    statusUI_Normal.DamageUI(isCritcal, calDamage);
                    break;

                case EnemyType.Boss:
                case EnemyType.BossTower:
                    statusUI_Boss.DamageUI(isCritcal, calDamage);
                    break;
            }

            // ��� üũ
            if (hp <= 0)
            {
                Die();
                break;
            }
        }
    }

    public void ShakeEffect(float duration, float strength)
    {
        if (ShakeCorouttine != null) StopCoroutine(ShakeCorouttine);
        StartCoroutine(ShakeRoutine(duration, strength));
    }

    private IEnumerator ShakeRoutine(float duration, float strength)
    {
        Vector3 originalPos = bodyObj.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * strength;
            float offsetY = Random.Range(-1f, 1f) * strength;

            bodyObj.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        bodyObj.localPosition = originalPos; // ��ġ ����
    }

    private IEnumerator HitInvincible()
    {
        isHit = true;
        yield return new WaitForSeconds(0.07f);
        isHit = false;
    }

    private void TimerAdd(float time)
    {
        if (airBorneTimer + time > maxAirborneTimer)
        {
            airBorneTimer = maxAirborneTimer;
        }
        else
        {
            airBorneTimer += time;
        }
    }

    protected void TimerCheck()
    {
        if (isAirBorneMove)
        {
            return;
        }
        else
        {
            // airborne Timer Check
            if (airBorneTimer > 0)
            {
                airBorneTimer -= Time.deltaTime;
                if (airBorneTimer <= 0)
                {
                    state = State.Idle;
                    airBorneTimer = 0;
                    rigid.gravityScale = 1;
                }
            }
        }
    }


    private IEnumerator AirBorneMove(Vector3 airBornePos, float power)
    {
        if (enemyType != EnemyType.Normal)
        {
            yield break;
        }

        isAirBorneMove = true;
        isAirBorne = true;
        rigid.gravityScale = 0;
        rigid.velocity = Vector2.zero;

        TimerAdd(1f);

        // Move
        Vector3 startPos = transform.position;
        /*
        Vector2 dir = airBornePos - startPos;
        RaycastHit2D hit = Physics2D.Raycast(startPos, dir.normalized, dir.magnitude);
        if (hit.collider != null) airBornePos = hit.point + hit.normal * 1f;
        */
        float timer = 0;
        while (timer < 1 && isAirBorneMove)
        {
            timer += Time.deltaTime * power;
            transform.position = Vector2.Lerp(startPos, airBornePos, EasingFunctions.OutExpo(timer));

            yield return null;
        }

        isAirBorneMove = false;
        rigid.gravityScale = 1;
        state = State.Idle;
    }

    private IEnumerator KnockBackMove(GameObject player, float power)
    {
        isKnockBack = true;
        isAirBorneMove = true;
        rigid.velocity = Vector2.zero;
        rigid.gravityScale = 0;

        // MovePos Setting
        Vector3 knockBackDir = (transform.position - player.transform.position).normalized;
        knockBackDir.y = 0;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, knockBackDir, 5f, groundLayer);
        Debug.DrawRay(transform.position, knockBackDir, Color.red, 5f);

        Vector3 startPos = transform.position;
        Vector3 endPos = hit.collider != null ? hit.point : knockBackDir * 5f;
        endPos.y = transform.position.y;

        // Move
        float timer = 0;
        while (timer < 1 && isAirBorneMove)
        {
            timer += Time.deltaTime * power;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        isKnockBack = false;
        isAirBorneMove = false;
        rigid.gravityScale = 1;

        state = State.Idle;
    }

    private IEnumerator DownAttackMove(float power)
    {
        if (isGround)
        {
            yield break;
        }
        else
        {
            isDownAttack = true;
            isAirBorneMove = true;
            rigid.velocity = Vector2.zero;
            airBorneTimer = 0;

            // MovePos Setting
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 100f, groundLayer);
            Vector2 startPos = transform.position;
            Vector2 endPos = hit.point + hit.normal * 1f;

            // Move
            float timer = 0;
            while (timer < 1 && isAirBorneMove)
            {
                timer += Time.deltaTime * power;
                transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
                yield return null;
            }

            isDownAttack = false;
            isAirBorneMove = false;
            rigid.velocity = Vector2.zero;
            rigid.gravityScale = 1;

            state = State.Idle;
        }
    }


    protected void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, aiCollider.bounds.size.y + 0.15f, groundLayer);
        if (rigid.velocity.y > 0)
        {
            isGround = false;
        }
        else
        {
            isGround = hit;
            Debug.DrawRay(transform.position, Vector2.down, Color.red, aiCollider.bounds.size.y + 0.15f);
        }

        if (isGround)
        {
            isAirBorne = false;
            airBorneTimer = 0;
        }
    }

    protected abstract void Spawn();

    protected abstract void Stagger();

    public abstract void Die();
}
