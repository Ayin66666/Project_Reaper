using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Easing;

public class Enemy_Boss_StatusUI : MonoBehaviour
{
    [SerializeField] private Enemy_Base enemy;
    [SerializeField] private GameObject damageText;
    [SerializeField] private RectTransform[] damagePos;
    [SerializeField] private bool isStage4;
    public bool isFade;
    private bool isDie;
    private float hitTimer;

    [Header("---In Game UI---")]
    [SerializeField] private Slider hpBarF;
    [SerializeField] private Slider hpBarB;
    [SerializeField] private Text nameText;

    [Header("---Fade UI---")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private Text fadeNameText;

    private void Start()
    {
        StatusUI_Setting();
    }

    private void FixedUpdate()
    {
        if (isFade || isDie)
        {
            return;
        }

        // Timer
        if (hitTimer > 0)
        {
            hitTimer -= Time.deltaTime;
        }

        HpBar();
    }

    private void StatusUI_Setting()
    {
        hpBarF.maxValue = enemy.hp;
        hpBarF.minValue = 0;

        hpBarB.maxValue = enemy.hp;
        hpBarB.minValue = 0;

        nameText.text = enemy.enemyName;
        fadeNameText.text = enemy.enemyName;
    }

    public void Die()
    {
        isDie = true;
        hpBarF.gameObject.SetActive(false);
        hpBarB.gameObject.SetActive(false);
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
            a += Time.deltaTime * 1.25f;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, a);
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.1f);

        // Name Fade In
        fadeNameText.gameObject.SetActive(true);
        a = 0;
        while(a < 1)
        {
            a += Time.deltaTime * 1.5f;
            fadeNameText.color = new Color(fadeNameText.color.r, fadeNameText.color.g, fadeNameText.color.b, a);
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.25f);

        // Fade Out
        a = 1;
        while (a > 0)
        {
            a -= Time.deltaTime * 1.25f;
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

    public void DamageUI(bool isCritical, int damage)
    {
        GameObject obj = Instantiate(damageText, damagePos[0].anchoredPosition, Quaternion.identity);
        obj.GetComponent<DamageUI>().DamageMove(isCritical, damage);
    }
}
