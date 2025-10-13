using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class Enemy_Normal_StatusUI : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private Enemy_Base enemy;
    [SerializeField] private Slider hpBarF;
    [SerializeField] private Slider hpBarB;
    [SerializeField] private Slider airBorneBar;
    private Coroutine hpCoroutine;
    private Coroutine airbonreCoroutine;


    [Header("---Damage UI---")]
    [SerializeField] private GameObject damageText;
    [SerializeField] private BoxCollider2D damagePosCollider;


    public void Status_Setting()
    {
        hpBarF.maxValue = enemy.hp;
        hpBarF.value = enemy.hp;
        hpBarF.minValue = 0;

        hpBarB.maxValue = enemy.hp;
        hpBarB.value = enemy.hp;
        hpBarB.minValue = 0;

        airBorneBar.maxValue = enemy.maxAirborneTimer;
        airBorneBar.value = 0;
        airBorneBar.minValue = 0;
    }

    /* 과거 코드
    private void FixedUpdate()
    {
        // Timer
        if (hitTimer > 0)
        {
            hitTimer -= Time.deltaTime;
        }

        // UI
        HpBar();
        AirBorneBar();
    }

    public void AddTimer()
    {
        hitTimer += 0.3f;
        if (hitTimer > 0.3f)
        {
            hitTimer = 0.3f;
        }
    }


    private void HpBar()
    {
        if (enemy.state == Enemy_Base.State.Die)
        {
            hpBarF.value = 0;
            hpBarB.value = Mathf.Lerp(hpBarB.value, 0, 20f * Time.deltaTime);
        }
        else
        {
            hpBarF.value = enemy.hp;
            if (hitTimer <= 0)
            {
                hpBarB.value = Mathf.Lerp(hpBarB.value, enemy.hp, 10f * Time.deltaTime);
            }
        }
    }
    */

    public void DamageUI(bool isCritical, int damage)
    {
        GameObject obj = Instantiate(damageText, GetPos(), Quaternion.identity);
        obj.GetComponent<DamageUI>().DamageMove(isCritical, damage);
    }


    #region 신규 코드
    /// <summary>
    /// 피격 시 체력바 최신화
    /// </summary>
    public void Hp()
    {
        if (hpCoroutine != null) StopCoroutine(hpCoroutine);
        hpCoroutine = StartCoroutine(HpCall());
    }

    private IEnumerator HpCall()
    {
        hpBarF.value = enemy.hp;
        yield return new WaitForSeconds(0.15f);

        float start = hpBarB.value;
        float end = enemy.hp;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / 0.5f;
            hpBarB.value = Mathf.Lerp(start, end, timer);
            yield return null;
        }

        hpBarB.value = enemy.hp;
    }

    /// <summary>
    /// 피격 시 에어본 타이머 최신화
    /// </summary>
    public void AirBorne()
    {
        if (airbonreCoroutine != null) StopCoroutine(airbonreCoroutine);
        airbonreCoroutine = StartCoroutine(AirBonreCall());
    }

    private IEnumerator AirBonreCall()
    {
        airBorneBar.value = enemy.airBorneTimer;
        float timer = enemy.airBorneTimer;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            airBorneBar.value = timer;
            yield return null;
        }
        airBorneBar.value = 0;
    }

    /// <summary>
    /// 사망 시 UI 초기화
    /// </summary>
    public void Die()
    {
        hpBarF.value = 0;
        hpBarB.value = 0;
        airBorneBar.value = 0;
    }

    private Vector2 GetPos()
    {
        Vector2 originPosition = damagePosCollider.transform.position;

        // 콜라이더의 사이즈를 가져오는 bound.size 사용
        float range_X = damagePosCollider.bounds.size.x;
        float range_Y = damagePosCollider.bounds.size.y;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Y = Random.Range((range_Y / 2) * -1, range_Y / 2);
        Vector2 RandomPostion = new Vector2(range_X, range_Y);

        Vector2 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }
    #endregion
}
