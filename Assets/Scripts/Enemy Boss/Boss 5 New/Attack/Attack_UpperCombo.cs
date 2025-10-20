using Easing;
using System.Collections;
using UnityEngine;


public class Attack_UpperCombo : Attack_Base
{
    [Header("---Attack Setting---")]
    [SerializeField] private Enemy_Boss5_New boss;
    [SerializeField] private float[] moveSpeed;
    [SerializeField] private GameObject[] attackCollider;
    [SerializeField] private Transform[] movePos;
    [SerializeField] private GameObject chargeVFX;
    [SerializeField] private Transform shootPos;
    [SerializeField] private GameObject swordAuraVFX;
    private Coroutine movementCoroutine;


    public override void Use()
    {
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        isUsed = true;

        // 돌진 - 어퍼 - 2연 베기(검기) - 돌진
        boss.LookAt();
        anim.SetTrigger("Action");
        anim.SetFloat("AnimValue", 0);
        anim.SetBool("isUpperComboUpper", true);
        anim.SetBool("isUpperComboSlash", true);
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

        // 2연 베기
        boss.LookAt();
        anim.SetTrigger("Action");
        yield return new WaitWhile(() => anim.GetBool("isUpperComboSlash"));
        
        // 차징
        chargeVFX.SetActive(true);
        yield return new WaitForSeconds(0.77f);

        // 돌진
        boss.LookAt();
        anim.SetTrigger("Action");
        anim.SetFloat("AnimValue", 0);

        attackCollider[1].SetActive(true);
        startPos = body.transform.position;
        endPos = movePos[1].position;
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
        anim.SetBool("isUpperCombo", false);

        isUsed = false;
    }

    public void Movement()
    {
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);
        movementCoroutine = StartCoroutine(MovementCall());
    }

    private IEnumerator MovementCall()
    {
        Vector3 startPos = body.transform.position;
        Vector3 endPos = movePos[0].position;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime / moveSpeed[0];
            body.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        body.transform.position = endPos;
    }

    public void Combo_Attack()
    {
        attackCollider[0].SetActive(attackCollider[0].activeSelf);
    }

    public void SwordAura()
    {
        GameObject obj = Instantiate(swordAuraVFX, shootPos.position, Quaternion.identity);
        Enemy_Bullet aura = obj.GetComponent<Enemy_Bullet>();
        Vector3 dir = new Vector3(body.transform.localScale.normalized.x, 0, 0);
        aura.Bullet_Setting(Enemy_Bullet.BulletType.None, dir, 25, 45f, 10f);
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
