using System.Collections;
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


    // 돌진 - 지상 난무 - 공중 텔포 - 공중 난무 - 텔포
    public override void Use()
    {
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        isUsed = true;

        // 차징
        chargeVFX.SetActive(true);
        anim.SetTrigger("Action");
        anim.SetBool("isAOECharge", true);
        anim.SetBool("isAOE", true);

        // 딜레이
        yield return new WaitForSeconds(0.5f);

        // 돌진
        boss.LookAt();
        anim.SetBool("isAOECharge", true);

        // 벽 체크
        Vector2 startPos = body.transform.position;
        Vector2 endPos = boss.curTarget.transform.position;
        Vector2 moveDir = endPos - startPos;
        RaycastHit2D hit = Physics2D.Raycast(startPos, moveDir.normalized, moveDir.magnitude, groundLayer);
        if (hit.collider != null) endPos = hit.point + hit.normal * 0.5f;

        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / moveSpeed;
            body.transform.position = Vector2.Lerp(startPos, endPos, timer);
            yield return null;
        }
        body.transform.position = movePos[0].position;

        // 지상 난무
        boss.LookAt();
        anim.SetTrigger("Action");
        yield return new WaitForSeconds(1f);

        // 텔포
        teleportVFX.SetActive(true);
        body.transform.position = movePos[1].position;

        // 공중 난무
        boss.LookAt();
        boss.Body_Setting(false);
        anim.SetTrigger("Action");
        yield return new WaitForSeconds(1f);
        anim.SetTrigger("Action");

        // 텔포 (지상)
        hit = Physics2D.Raycast(body.transform.position, Vector2.down, 100, groundLayer);
        body.transform.position = hit.point;
        boss.Body_Setting(true);

        isUsed = false;
    }

    public override void Reset()
    {
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        foreach(GameObject obj in attackCollider)
        {
            obj.SetActive(false);
        }
    }
}
