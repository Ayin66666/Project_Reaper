using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing;


public class Attack_GroundRush : Attack_Base
{
    [Header("---Attack Setting---")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject attackCollider;
    [SerializeField] private Transform movePos;


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

        // Â÷Â¡
        anim.SetTrigger("Action");
        anim.SetBool("isGroundCharge", true);
        anim.SetBool("isGroundRush", true);
        anim.SetFloat("AnimValue", 0);

        // µô·¹ÀÌ
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("isGroundCharge", false);

        // µ¹Áø
        Vector3 startPos = body.transform.position;
        Vector3 endPos = movePos.position;
        Vector2 moveDir = endPos - startPos;
        RaycastHit2D hit = Physics2D.Raycast(startPos, moveDir.normalized, moveDir.magnitude, groundLayer);
        if (hit.collider != null) endPos = hit.point + hit.normal * 0.5f;

        attackCollider.SetActive(true);
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime / moveSpeed;
            anim.SetFloat("AnimValue", timer);
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        transform.position = endPos;
        attackCollider.SetActive(false);
        anim.SetBool("isGroundRush", false);
        anim.SetFloat("AnimValue", 1);
        SwordAura();

        isUsed = false;
    }

    private void SwordAura()
    {
        GameObject obj = Instantiate(swordAura, shootPos.position, Quaternion.identity);
        Vector3 shootDir = (shootPos.position - body.transform.position).normalized;
        Enemy_Bullet bullet = obj.GetComponent<Enemy_Bullet>();
        bullet.Bullet_Setting(Enemy_Bullet.BulletType.None, shootDir, 15f, 30f, 15f);
    }

    public void Exposion()
    {
        if (explosionCoroutine != null) StopCoroutine(explosionCoroutine);
        explosionCoroutine = StartCoroutine(ExposionCall());
    }

    private IEnumerator ExposionCall()
    {
        WaitForSeconds wait = new WaitForSeconds(0.15f);

        // À§Ä¡ ÀúÀå
        List<Vector3> pos = new List<Vector3>();
        for (int i = 0; i < explosionPos.Count; i++)
        {
            pos.Add(explosionPos[i].position);
        }

        // Æø¹ß
        for (int i = 0; i < pos.Count; i++)
        {
            GameObject obj = Instantiate(explosionVFX, pos[i], Quaternion.identity);
            yield return wait;
        }
    }

    public override void Reset()
    {
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        if (explosionCoroutine != null) StopCoroutine(explosionCoroutine);
        attackCollider.SetActive(false);
    }
}
