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
    ���̵�
    �� �� - �˱� 
    �� �� - �˱�
    �� �� - �˱�
    */

    private IEnumerator UseCall()
    {
        isUsed = true;

        // ���̵� ��
        Enemy_Boss5_New boss = body.GetComponent<Enemy_Boss5_New>();
        boss.Fade(true);

        // ������
        yield return new WaitForSeconds(0.5f);

        // 3�� ����
        for (int i = 0; i < 3; i++)
        {
            // ���̵� �ƿ�
            boss.Fade(false);

            // �ִ�
            anim.SetTrigger("Action");
            anim.SetBool("ComboRush", true);

            // �̵� ����
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

            // �˱�
            GameObject obj = Instantiate(slashVFX, slashPos.position, Quaternion.identity);

            // ������
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
