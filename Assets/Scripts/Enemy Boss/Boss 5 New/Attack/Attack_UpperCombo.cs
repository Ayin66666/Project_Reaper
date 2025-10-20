using Easing;
using System.Collections;
using UnityEngine;


public class Attack_UpperCombo : Attack_Base
{
    [Header("---Attack Setting---")]
    [SerializeField] private Enemy_Boss5_New boss;
    [SerializeField] private float[] moveSpeed;
    [SerializeField] private GameObject[] attackCollider;
    [SerializeField] private Transform movePos;
    [SerializeField] private GameObject chargeVFX;
    [SerializeField] private Transform shootPos;
    [SerializeField] private GameObject swordAuraVFX;


    public override void Use()
    {
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        // 돌진 - 어퍼 - 2연 베기(검기) - 돌진

        boss.LookAt();
        anim.SetTrigger("Action");
        anim.SetFloat("AnimValue", 0);
        anim.SetBool("isUpperComboUpper", true);
        anim.SetBool("isUpperCombo", true);

        // 벽 체크
        Vector2 startPos = body.transform.position;
        Vector2 endPos = boss.curTarget.transform.position;
        endPos.y = startPos.y;
        Vector2 moveDir = endPos - startPos;
        RaycastHit2D hit = Physics2D.Raycast(startPos, moveDir.normalized, moveDir.magnitude, groundLayer);
        if (hit.collider != null) endPos = hit.point + hit.normal * 2f;

        // 돌진
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / moveSpeed[0];
            body.transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            anim.SetFloat("AnimValue", timer);
            yield return null;
        }
        anim.SetFloat("AnimValue", 1);
        body.transform.position = endPos;

        // 어퍼
        boss.LookAt();
        anim.SetTrigger("Action");
        yield return new WaitWhile(() => anim.GetBool("isUpperComboUpper"));

        // 돌진
        boss.LookAt();
        anim.SetFloat("AnimValue", 0);
        attackCollider[1].SetActive(true);
        startPos = body.transform.position;
        endPos = movePos.position;
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / moveSpeed[1];
            body.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            anim.SetFloat("AnimValue", timer);
            yield return null;
        }
        body.transform.position = endPos;
        attackCollider[1].SetActive(false);

        isUsed = false;
    }

    public void SwordAura()
    {

    }

    public override void Reset()
    {
        chargeVFX.SetActive(false);
        foreach (GameObject obj in attackCollider)
        {
            obj.SetActive(false);
        }
    }
}
