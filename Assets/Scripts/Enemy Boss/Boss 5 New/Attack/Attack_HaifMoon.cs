using Easing;
using System.Collections;
using UnityEngine;


public class Attack_HaifMoon : Attack_Base
{
    [Header("---Attack Setting---")]
    [SerializeField] private float[] moveSpeed;
    [SerializeField] private GameObject[] attackCollider;
    [SerializeField] private GameObject moveHolder;
    [SerializeField] private Transform[] movePos;
    [SerializeField] private GameObject chargeVFX;
    [SerializeField] private GameObject teleportVFX;
    [SerializeField] private GameObject eyeVFX;
    [SerializeField] private GameObject haifMoonVFX;
    [SerializeField] private Enemy_Boss5_New boss;


    [Header("---Sword Aura---")]
    [SerializeField] private GameObject swordAuraVFX;
    [SerializeField] private Transform[] shootPos;
    [SerializeField] private LineRenderer[] shootLine;
    private Coroutine slashCoroutine;


    [Header("---Background Fade---")]
    [SerializeField] private SpriteRenderer backgroundFade;
    private Coroutine backgroundFadeCoroutine;
    public bool isBackgroundFade;


    public override void Use()
    {
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        isUsed = true;

        anim.SetTrigger("Action");
        anim.SetBool("isHaifMoonCharge", true);
        anim.SetBool("isHaifMoonSlashAttack", true);
        anim.SetBool("isHaifMoonSlash", true);
        anim.SetFloat("AnimValue", 0);

        // 차징
        boss.LookAt();
        chargeVFX.SetActive(true);
        boss.Rigid_Setting(false);
        yield return new WaitForSeconds(0.5f);
        chargeVFX.SetActive(false);
        anim.SetBool("isHaifMoonCharge", false);

        // 공중 검기
        if (slashCoroutine != null) StopCoroutine(slashCoroutine);
        slashCoroutine = StartCoroutine(Slash());

        // 텔포 - 돌진 x 4
        Vector3 startPos;
        Vector3 endPos;
        float timer;
        for (int i = 0; i < 4; i++)
        {
            // 위치 설정
            moveHolder.transform.position = boss.curTarget.transform.position;
            startPos = movePos[i % 2 == 0 ? 0 : 1].position;
            endPos = movePos[i % 2 == 0 ? 1 : 0].position;

            // 보스 등장
            Instantiate(teleportVFX, body.transform.position, Quaternion.identity);
            boss.transform.position = startPos;
            boss.LookAt();
            anim.SetTrigger("Action");
            anim.SetFloat("AnimValue", 0);

            // 돌진
            timer = 0;
            attackCollider[i].SetActive(true);
            while (timer < 1)
            {
                timer += Time.deltaTime / moveSpeed[i];
                body.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
                anim.SetFloat("AnimValue", timer);
                yield return null;
            }
            attackCollider[i].SetActive(false);
            anim.SetFloat("AnimValue", 1);
            body.transform.position = endPos;

            // 딜레이
            yield return new WaitForSeconds(0.1f - (0.025f * i));
        }

        // 암전
        BackgroundFade(true, 1f);
        yield return new WaitForSeconds(0.1f);

        // 상단 이동
        boss.Body_Setting(false);
        Vector3 move = boss.curTarget.transform.position;
        move.y += 3f;
        body.transform.position = move;
        Instantiate(teleportVFX, body.transform.position, Quaternion.identity);

        // 딜레이
        yield return new WaitForSeconds(Random.Range(0.72f, 1.25f));

        // 반원 베기
        boss.Body_Setting(true);
        anim.SetTrigger("Action");
        yield return new WaitWhile(() => anim.GetBool("isHaifMoonSlashAttack"));
        yield return new WaitForSeconds(0.5f);

        // 내려오기
        anim.SetTrigger("Action");
        RaycastHit2D hit = Physics2D.Raycast(body.transform.position, Vector2.down, 50, groundLayer);
        Vector3 landPos = hit.point + Vector2.up * 0.5f;
        body.transform.position = landPos;
        Instantiate(teleportVFX, body.transform.position, Quaternion.identity);
        boss.Rigid_Setting(true);

        isUsed = false;
    }

    public void AttackCollider()
    {
        eyeVFX.SetActive(true);
        BackgroundFade(false, 0);
        Instantiate(haifMoonVFX, body.transform.position, Quaternion.identity);
    }

    /*
    private void Slash()
    {
        int ran = Random.Range(1, 3);
        for (int i = 0; i < ran; i++)
        {
            int ran2 = Random.Range(0, shootPos.Length);
            GameObject obj = Instantiate(swordAuraVFX, shootPos[ran2].position, Quaternion.identity);
            Enemy_Bullet aura = obj.GetComponent<Enemy_Bullet>();
            Instantiate(teleportVFX, obj.transform.position, Quaternion.identity);

            Vector3 dir = boss.curTarget.transform.position - obj.transform.position;
            aura.Bullet_Setting(Enemy_Bullet.BulletType.None, dir.normalized, Random.Range(15f, 25f), 45f, 10f);

            shootLine[ran2].SetPosition(0, shootPos[ran2].position);
            shootLine[ran2].SetPosition(1, boss.curTarget.transform.position);
        }
    }
    */

    private IEnumerator Slash()
    {
        for (int i = 0; i < 4; i++)
        {
            int ran = Random.Range(1, 2);
            for (int i2 = 0; i2 < ran; i2++)
            {
                int ran2 = Random.Range(0, shootPos.Length);
                GameObject obj = Instantiate(swordAuraVFX, shootPos[ran2].position, Quaternion.identity);
                Enemy_Bullet aura = obj.GetComponent<Enemy_Bullet>();
                Instantiate(teleportVFX, obj.transform.position, Quaternion.identity);

                Vector3 dir = boss.curTarget.transform.position - obj.transform.position;
                aura.Bullet_Setting(Enemy_Bullet.BulletType.None, dir.normalized, Random.Range(15f, 25f), 45f, 10f);

                shootLine[ran2].SetPosition(0, shootPos[ran2].position);
                shootLine[ran2].SetPosition(1, boss.curTarget.transform.position);
            }

            yield return new WaitForSeconds(0.5f);
            foreach (LineRenderer line in shootLine)
            {
                line.SetPosition(0, Vector3.zero);
                line.SetPosition(1, Vector3.zero);
            }

            yield return new WaitForSeconds(0.15f);
        }
    }

    /// <summary>
    /// 암전 효과
    /// </summary>
    /// <param name="isOn"></param>
    /// <param name="speed"></param>
    public void BackgroundFade(bool isOn, float speed)
    {
        if (backgroundFadeCoroutine != null) StopCoroutine(backgroundFadeCoroutine);
        backgroundFadeCoroutine = StartCoroutine(BackgroundFadeCall(isOn, speed));
    }

    private IEnumerator BackgroundFadeCall(bool isOn, float speed)
    {
        isBackgroundFade = true;
        backgroundFade.gameObject.SetActive(true);
        backgroundFade.color = new Color(0, 0, 0, isOn ? 0 : 1);

        float start = isOn ? 0 : 1;
        float end = isOn ? 1 : 0;

        // speed가 0 이하일 경우 즉시 전환
        if (speed <= 0f)
        {
            backgroundFade.color = new Color(0, 0, 0, start);
            if (!isOn) backgroundFade.gameObject.SetActive(false);
            isBackgroundFade = false;
            yield break;
        }

        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / speed;
            backgroundFade.color = new Color(0, 0, 0, Mathf.Lerp(start, end, timer));
            yield return null;
        }
        backgroundFade.color = new Color(0, 0, 0, end);

        if (!isOn) backgroundFade.gameObject.SetActive(true);
        isBackgroundFade = false;
    }

    public override void Reset()
    {
        isUsed = false;

        if (useCoroutine != null) StopCoroutine(useCoroutine);
        boss.Body_Setting(true);
        boss.Rigid_Setting(true);
        chargeVFX.SetActive(false);
        eyeVFX.SetActive(false);
        BackgroundFade(false, 0);
    }
}
