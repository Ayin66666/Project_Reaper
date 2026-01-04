using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing;


public class Attack_Combo : Attack_Base
{
    [Header("---Skill setting---")]
    [SerializeField] private GameObject swordAura;
    [SerializeField] private Transform shootPos;
    [SerializeField] private List<GameObject> attackCollider;


    [Header("---Move Setting---")]
    [SerializeField] private Transform movePos;
    [SerializeField] private float moveSpeed;
    private Coroutine movecoroutine;


    public override void Use()
    {
        Debug.Log("Call Use");
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        isUsed = true;

        anim.SetTrigger("Action");
        anim.SetBool("isCombo", true);
        while (anim.GetBool("isCombo"))
        {
            yield return null;
        }

        isUsed = false;
    }


    public void Combo_Attack(int index)
    {
        attackCollider[index].SetActive(!attackCollider[index].activeSelf);
    }

    public void ComboMove()
    {
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        movecoroutine = StartCoroutine(ComboMoveCall());
    }

    private IEnumerator ComboMoveCall()
    {
        Vector3 startPos = body.transform.position;
        Vector3 endPos = movePos.position;
        float timer = 0f;
        while (timer < 1)
        {
            timer += Time.deltaTime / moveSpeed;
            body.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        body.transform.position = endPos;
    }

    public void SwordAura()
    {
        GameObject obj = Instantiate(swordAura, shootPos.position, Quaternion.identity);
        Enemy_Bullet aura = obj.GetComponent<Enemy_Bullet>();
        Vector3 dir = new Vector3(body.transform.localScale.normalized.x, 0, 0);
        aura.Bullet_Setting(Enemy_Bullet.BulletType.None, dir, 25, 45f, 10f);
    }

    public override void Reset()
    {
        Debug.Log("종료 호출");
        isUsed = false;
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        if (movecoroutine != null) StopCoroutine(movecoroutine);
        foreach (GameObject obj in attackCollider)
        {
            obj.SetActive(false);
        }
    }
}
