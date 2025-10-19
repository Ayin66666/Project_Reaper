using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing;


public class Attack_GroundRush : Attack_Base
{
    [Header("---Attack Setting---")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject[] attackCollider;
    [SerializeField] private Transform movePos;
    [SerializeField] private Enemy_Boss5_New boss;


    [Header("---Explosion Setting---")]
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private List<Transform> explosionPos;
    private Coroutine explosionCoroutine;


    [Header("---Sword Aura---")]
    [SerializeField] private GameObject swordAura;
    [SerializeField] private Transform shootPos;


    public override void Use()
    {
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        isUsed = true;

        // 차징
        anim.SetTrigger("Action");
        anim.SetBool("isGroundCharge", true);
        anim.SetBool("isGroundRush", true);
        anim.SetFloat("AnimValue", 0);

        // 딜레이
        yield return new WaitForSeconds(0.35f);
        anim.SetBool("isGroundCharge", false);
        boss.LookAt();

        // 돌진
        Vector3 startPos = body.transform.position;
        Vector3 endPos = movePos.position;
        Vector2 moveDir = endPos - startPos;
        RaycastHit2D hit = Physics2D.Raycast(startPos, moveDir.normalized, moveDir.magnitude, groundLayer);
        if (hit.collider != null) endPos = hit.point + hit.normal * 0.5f;

        attackCollider[0].SetActive(true);
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime / moveSpeed;
            anim.SetFloat("AnimValue", timer);
            body.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        transform.position = endPos;
        attackCollider[0].SetActive(false);

        // 지면 폭발
        Exposion();

        // 딜레이
        boss.LookAt();
        yield return new WaitForSeconds(0.15f);

        // 돌진 후 추가 베기
        anim.SetTrigger("Action");
        anim.SetFloat("AnimValue", 1);
        yield return new WaitWhile(() => anim.GetBool("isGroundRush"));
        isUsed = false;
    }


    public void GroundRush_Attack()
    {
        attackCollider[1].SetActive(!attackCollider[1].activeSelf);
    }

    public void SwordAura()
    {
        GameObject obj = Instantiate(swordAura, shootPos.position, Quaternion.identity);
        Vector3 shootDir = (shootPos.position - body.transform.position).normalized;
        Enemy_Bullet bullet = obj.GetComponent<Enemy_Bullet>();
        bullet.Bullet_Setting(Enemy_Bullet.BulletType.None, shootDir, 15f, 30f, 15f);
    }

    private void Exposion()
    {
        if (explosionCoroutine != null) StopCoroutine(explosionCoroutine);
        explosionCoroutine = StartCoroutine(ExposionCall());
    }

    private IEnumerator ExposionCall()
    {
        WaitForSeconds wait = new WaitForSeconds(0.05f);

        // 위치 저장
        List<Vector3> pos = new List<Vector3>();
        for (int i = 0; i < explosionPos.Count; i++)
        {
            pos.Add(explosionPos[i].position);
        }

        // 폭발
        for (int i = 0; i < pos.Count; i++)
        {
            Instantiate(explosionVFX, pos[i], Quaternion.identity);
            yield return wait;
        }
    }

    public override void Reset()
    {
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        if (explosionCoroutine != null) StopCoroutine(explosionCoroutine);
        
        foreach(GameObject obj in attackCollider)
        {
            obj.SetActive(false);
        }
    }
}
