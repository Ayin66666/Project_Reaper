using Easing;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;


public class Attack_HaifMoon : Attack_Base
{
    [Header("---Attack Setting---")]
    [SerializeField] private float[] moveSpeed;
    [SerializeField] private GameObject[] attackCollider;
    [SerializeField] private Transform[] movePos;
    [SerializeField] private GameObject chargeVFX;
    [SerializeField] private GameObject teleportVFX;
    [SerializeField] private GameObject eyeVFX;
    [SerializeField] private Enemy_Boss5_New boss;


    public override void Use()
    {
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        isUsed = true;

        anim.SetTrigger("Action");
        anim.SetFloat("AnimValue", 0);
        anim.SetBool("isHaifMoonCharge", true);
        anim.SetBool("isHaifMoonSlash", true);

        // 차징
        boss.LookAt();
        chargeVFX.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        chargeVFX.SetActive(false);


        // 텔포 - 돌진 x 4
        Vector3 startPos;
        Vector3 endPos;
        float timer;
        for (int i = 0; i < 4; i++)
        {
            boss.Body_Setting(true);
            anim.SetTrigger("Action");
            anim.SetFloat("AnimValue", 0);

            Instantiate(teleportVFX, body.transform.position, Quaternion.identity);
            startPos = movePos[i % 2 == 0 ? 0 : 1].position;
            endPos = movePos[i % 2 == 0 ? 1 : 0].position;
            timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime / moveSpeed[i];
                body.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
                yield return null;
            }
            anim.SetFloat("AnimValue", 1);
            body.transform.position = endPos;

            // 딜레이
            boss.Body_Setting(false);
            yield return new WaitForSeconds(0.15f - (0.025f * i));
        }

        // 암전
        boss.Rigid_Setting(false);
        boss.Body_Setting(false);
        body.transform.position = movePos[2].position;
        Instantiate(teleportVFX, body.transform.position, Quaternion.identity);

        // 딜레이
        yield return new WaitForSeconds(Random.Range(0.72f, 1.25f));

        // 반원 베기
        anim.SetTrigger("Action");
        eyeVFX.SetActive(true);
        attackCollider[4].SetActive(true);
        yield return new WaitWhile(() => anim.GetBool("isHaifMoonSlash"));

        // 내려오기
        RaycastHit2D hit = Physics2D.Raycast(body.transform.position, Vector2.down, 50, groundLayer);
        body.transform.position = hit.point;
        Instantiate(teleportVFX, body.transform.position, Quaternion.identity);
        boss.Rigid_Setting(true);
        boss.Body_Setting(true);

        isUsed = false;
    }

    public void AttackCollider()
    {
        attackCollider[4].SetActive(attackCollider[4].activeSelf);
    }

    public override void Reset()
    {
        throw new System.NotImplementedException();
    }
}
