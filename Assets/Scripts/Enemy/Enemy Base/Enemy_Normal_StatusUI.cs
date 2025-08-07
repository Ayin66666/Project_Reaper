using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Easing;

public class Enemy_Normal_StatusUI : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private Enemy_Base enemy;
    [SerializeField] private Slider hpBarF;
    [SerializeField] private Slider hpBarB;
    [SerializeField] private Slider airBorneBar;
    [SerializeField] private GameObject damageText;
    [SerializeField] private RectTransform[] damagePos;
    private float hitTimer;

    private void FixedUpdate()
    {
        // Timer
        if(hitTimer > 0)
        {
            hitTimer -= Time.deltaTime;
        }

        // UI
        HpBar();
        AirBorneBar();
    }
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

    public void AddTimer()
    {
        hitTimer += 0.3f;
        if(hitTimer > 0.3f)
        {
            hitTimer = 0.3f;
        }
    }

    private void HpBar()
    {
        if(enemy.state == Enemy_Base.State.Die)
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

    private void AirBorneBar()
    {
        if(enemy.state == Enemy_Base.State.Die)
        {
            airBorneBar.value = 0;
        }
        else
        {
            airBorneBar.value = Mathf.Lerp(airBorneBar.value, enemy.airBorneTimer, 10f * Time.deltaTime);
        }
    }

    public void DamageUI(bool isCritical, int damage)
    {
        GameObject obj = Instantiate(damageText, damagePos[0].anchoredPosition, Quaternion.identity);
        obj.GetComponent<Enemy_DamageUI>().DamageMove(damagePos[0].anchoredPosition, damagePos[1].anchoredPosition, 1.5f, damage, isCritical);
    }
}
