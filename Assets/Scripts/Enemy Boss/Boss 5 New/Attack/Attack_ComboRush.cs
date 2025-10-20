using System.Collections;
using UnityEngine;
using Easing;
using System.Collections.Generic;


public class Attack_ComboRush : Attack_Base
{
    [Header("---Attack Setting---")]
    [SerializeField] private float[] moveSpeed;
    [SerializeField] private GameObject chargeVFX;
    [SerializeField] private GameObject chargeVFX2;
    [SerializeField] private GameObject[] attackCollider;
    [SerializeField] private Transform[] movePos;
    [SerializeField] private Enemy_Boss5_New boss;


    [Header("---Slash VFX---")]
    [SerializeField] private GameObject slashVFX;
    [SerializeField] private Transform slashPos;


    public override void Use()
    {
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        useCoroutine = StartCoroutine(UseCall());
    }


    // 차징 - 페이드 - 등장 - 2연베기 - 강돌진
    private IEnumerator UseCall()
    {
        isUsed = true;

        anim.SetTrigger("Action");
        anim.SetFloat("AnimValue", 0);
        anim.SetBool("isComboRushCharge", true);
        anim.SetBool("isComboRush", true);

        chargeVFX.SetActive(true);
        chargeVFX2.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("isComboRushCharge", false);

        // 고속베기 x2
        Vector3 startPos;
        Vector3 endPos;
        float timer;
        List<Vector3> pos = new List<Vector3> { movePos[0].position, movePos[1].position };
        for (int i = 0; i < pos.Count; i++)
        {
            anim.SetTrigger("Action");
            anim.SetFloat("AnimValue", 0);

            // 돌진
            boss.LookAt();
            attackCollider[i].SetActive(true);
            startPos = body.transform.position;
            endPos = movePos[0].position;
            timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime / moveSpeed[0];
                anim.SetFloat("AnimValue", timer);
                body.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
                yield return null;
            }
            anim.SetFloat("AnimValue", 1);
            body.transform.position = endPos;
            attackCollider[i].SetActive(false);
            Slash();

            // 딜레이
            yield return new WaitForSeconds(0.15f);
            
        }

        // 차징
        anim.SetTrigger("Action");
        anim.SetBool("isComboRushCharge", true);
        boss.LookAt();
        chargeVFX.SetActive(true);
        chargeVFX2.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("isComboRushCharge", false);

        // 강 베기
        anim.SetTrigger("Action");
        anim.SetFloat("AnimValue", 0);
        chargeVFX.SetActive(true);
        boss.LookAt();
        startPos = body.transform.position;
        endPos = movePos[1].position;
        timer = 0;
        attackCollider[1].SetActive(true);
        while (timer < 1)
        {
            timer += Time.deltaTime / moveSpeed[1];
            anim.SetFloat("AnimValue", timer);
            body.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        body.transform.position = endPos;
        attackCollider[1].SetActive(false);
        anim.SetBool("isComboRush", false);
        Slash();

        isUsed = false;
    }

    private void Slash()
    {
        Instantiate(slashVFX, slashPos.position, Quaternion.identity);
    }

    public override void Reset()
    {
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        body.GetComponent<Enemy_Boss5_New>().Body_Setting(true);
        chargeVFX.SetActive(false);
        foreach (GameObject obj in attackCollider)
        {
            obj.SetActive(false);
        }
    }
}
