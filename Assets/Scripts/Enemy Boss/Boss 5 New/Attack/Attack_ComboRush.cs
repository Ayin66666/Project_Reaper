using System.Collections;
using UnityEngine;
using Easing;


public class Attack_ComboRush : Attack_Base
{
    [Header("---Attack Setting---")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject attackCollider;
    [SerializeField] private Transform[] movePos;


    [Header("---Slash VFX---")]
    [SerializeField] private GameObject slashVFX;
    [SerializeField] private Transform slashPos;


    public override void Use()
    {
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        useCoroutine = StartCoroutine(UseCall());
    }

    /*
    페이드
    좌 우 - 검기 
    우 좌 - 검기
    좌 우 - 검기
    */

    private IEnumerator UseCall()
    {
        isUsed = true;

        // 페이드 인
        Enemy_Boss5_New boss = body.GetComponent<Enemy_Boss5_New>();
        boss.Fade(true);

        // 딜레이
        yield return new WaitForSeconds(0.5f);

        // 3연 베기
        for (int i = 0; i < 3; i++)
        {
            // 페이드 아웃
            boss.Fade(false);

            // 애니
            anim.SetTrigger("Action");
            anim.SetBool("ComboRush", true);

            // 이동 설정
            Vector3 startPos = movePos[i % 2 == 0 ? 0 : 1].position;
            Vector3 endPos = movePos[i % 2 == 0 ? 1 : 0].position;
            float timer = 0;
            attackCollider.SetActive(true);
            while (timer < 1)
            {
                timer += Time.deltaTime / moveSpeed;
                body.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
                yield return null;
            }
            body.transform.position = endPos;
            attackCollider.SetActive(false);
            anim.SetBool("ComboRush", false);

            // 검기
            GameObject obj = Instantiate(slashVFX, slashPos.position, Quaternion.identity);

            // 딜레이
            if (i <= 2)
            {
                boss.Fade(true);
                yield return new WaitForSeconds(0.35f);
            }
        }

        boss.Fade(false);
        isUsed = false;
    }

    public override void Reset()
    {
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        body.GetComponent<Enemy_Boss5_New>().Fade(false);
        attackCollider.SetActive(false);
    }
}
