using Easing;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;


public class Attack_AOE : Attack_Base
{
    [Header("----Attack Setting---")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject chargeVFX;
    [SerializeField] private GameObject teleportVFX;
    [SerializeField] private Transform[] movePos;
    [SerializeField] private GameObject[] attackCollider;
    [SerializeField] private Enemy_Boss5_New boss;


    // ���� - ���� ���� - ���� ���� - ���� ���� - ����
    public override void Use()
    {
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        isUsed = true;

        // ��¡
        chargeVFX.SetActive(true);
        anim.SetTrigger("Action");
        anim.SetBool("isAOECharge", true);
        anim.SetFloat("AnimValue", 0);
        anim.SetBool("isAOE", true);

        // ������
        boss.LookAt();
        yield return new WaitForSeconds(0.5f);

        // ����
        boss.LookAt();
        anim.SetBool("isAOECharge", false);

        // �� üũ
        Vector2 startPos = body.transform.position;
        Vector2 endPos = boss.curTarget.transform.position;
        endPos.y = startPos.y;
        Vector2 moveDir = endPos - startPos;
        RaycastHit2D hit = Physics2D.Raycast(startPos, moveDir.normalized, moveDir.magnitude, groundLayer);
        if (hit.collider != null) endPos = hit.point + hit.normal * 1.5f;

        // ����
        attackCollider[0].SetActive(true);
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / moveSpeed;
            body.transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            anim.SetFloat("AnimValue", timer);
            yield return null;
        }
        attackCollider[0].SetActive(false);
        anim.SetFloat("AnimValue", 1);
        body.transform.position = endPos;

        // ���� ����
        boss.LookAt();
        anim.SetTrigger("Action");
        attackCollider[1].SetActive(true);
        yield return new WaitForSeconds(2f);
        attackCollider[1].SetActive(false);

        // �ڷ���Ʈ
        anim.SetTrigger("Action");
        anim.SetBool("isAOECharge", true);

        boss.LookAt();
        boss.Rigid_Setting(false);
        body.transform.position = movePos[0].position;
        chargeVFX.SetActive(true);

        // ������
        yield return new WaitForSeconds(0.25f);

        boss.LookAt();
        anim.SetTrigger("Action");
        anim.SetBool("isAOECharge", false);

        // ���� ����
        attackCollider[2].SetActive(true);
        startPos = body.transform.position;
        endPos = movePos[1].position;
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / moveSpeed;
            body.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            anim.SetFloat("AnimValue", timer);
            yield return null;
        }
        body.transform.position = endPos;
        attackCollider[2].SetActive(false);
        anim.SetFloat("AnimValue", 1);
        boss.Body_Setting(false);

        // ���� ����
        attackCollider[3].SetActive(true);
        yield return new WaitForSeconds(2f);
        attackCollider[3].SetActive(false);
        boss.Body_Setting(true);

        // ���� - �ϰ� �ִϸ��̼�
        hit = Physics2D.Raycast(body.transform.position, Vector2.down, 50, groundLayer);
        body.transform.position = hit.point;
        boss.Rigid_Setting(true);
        teleportVFX.SetActive(true);

        anim.SetTrigger("Action");
        yield return new WaitWhile(() => anim.GetBool("isAOE"));

        isUsed = false;
    }

    public override void Reset()
    {
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        foreach (GameObject obj in attackCollider)
        {
            obj.SetActive(false);
        }
    }
}
