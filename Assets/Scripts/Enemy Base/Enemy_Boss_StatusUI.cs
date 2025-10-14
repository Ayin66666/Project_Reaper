using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class Enemy_Boss_StatusUI : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private Enemy_Base enemy;
    [SerializeField] private GameObject damageText;
    [SerializeField] private BoxCollider2D damagePosCollider;
    [SerializeField] private bool isStage4;
    public bool isFade;
    private bool isDie;
    private float hitTimer;
    private Coroutine hpCoroutine;


    [Header("---In Game UI---")]
    [SerializeField] private Slider hpBarF;
    [SerializeField] private Slider hpBarB;
    [SerializeField] private GameObject hpBorder;
    [SerializeField] private Text nameText;


    [Header("---Fade UI---")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private Text fadeNameText;


    private void Start()
    {
        StatusUI_Setting();
    }

    private void StatusUI_Setting()
    {
        hpBarF.maxValue = enemy.hp;
        hpBarF.minValue = 0;

        hpBarB.maxValue = enemy.hp;
        hpBarB.minValue = 0;

        nameText.text = enemy.enemyName;
        fadeNameText.text = enemy.enemySudName;
    }

    public void Die()
    {
        isDie = true;
        hpBarF.gameObject.SetActive(false);
        hpBarB.gameObject.SetActive(false);
        hpBorder.SetActive(false);
        nameText.gameObject.SetActive(false);
    }

    public void StartFadeCall()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        isFade = true;
        fadeImage.gameObject.SetActive(true);

        // Fade In
        float a = 0;
        while(a < 1)
        {
            a += Time.deltaTime / 0.75f;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, a);
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.15f);

        // Name Fade In
        fadeNameText.gameObject.SetActive(true);
        a = 0;
        while(a < 1)
        {
            a += Time.deltaTime / 0.75f;
            fadeNameText.color = new Color(fadeNameText.color.r, fadeNameText.color.g, fadeNameText.color.b, a);
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.25f);

        // Fade Out
        a = 1;
        while (a > 0)
        {
            a -= Time.deltaTime / 0.75f;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, a);
            fadeNameText.color = new Color(fadeNameText.color.r, fadeNameText.color.g, fadeNameText.color.b, a);
            yield return null;
        }

        isFade = false;
        fadeImage.gameObject.SetActive(false);
        fadeNameText.gameObject.SetActive(false);
    }

    public void AddTimer()
    {
        hitTimer += 0.3f;
        if (hitTimer > 0.3f)
        {
            hitTimer = 0.3f;
        }
    }

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

    public void DamageUI(bool isCritical, int damage)
    {
        GameObject obj = Instantiate(damageText, GetPos(), Quaternion.identity);
        obj.GetComponent<DamageUI>().DamageMove(isCritical, damage);
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
}
