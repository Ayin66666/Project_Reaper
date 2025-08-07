using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Easing;
using System;

public class Enemy_DamageUI : MonoBehaviour
{
    [SerializeField] private Text damageText;
    private bool isCritical;

    public void DamageMove(Vector2 startPos, Vector2 endPos, float speed, int damage, bool isCritical)
    {
        this.isCritical = isCritical;
        damageText.text = damage.ToString();
        StartCoroutine(DamageMovement(startPos, endPos, speed));
    }

    private IEnumerator DamageMovement(Vector2 startPos, Vector2 endPos, float speed)
    {
        // Move
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * (isCritical ? speed * 1.5f : speed);
            transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.OutElastic(timer));
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.1f);

        // Color Setting
        if(isCritical)
            damageText.color = new Color(1, 0, 0, 1);

        // Fade Out
        timer = 1;
        while(timer > 0)
        {
            timer -= Time.deltaTime * 2f;
            damageText.color = new Color(damageText.color.r, damageText.color.g, damageText.color.b, Mathf.Lerp(1, 0, timer));
            yield return null;
        }

        // Destroy
        Destroy(gameObject);
    }
}
