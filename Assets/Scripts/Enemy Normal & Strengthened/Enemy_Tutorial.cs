using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Tutorial : Enemy_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject dieVFX;


    private void Start()
    {
        Spawn();    
    }


    protected override void Spawn()
    {
        state = State.Spawn;

        Target_Setting();
        Status_Setting();
        statusUI_Normal.Status_Setting();

        LookAt();
        state = State.Idle;
    }

    protected override void Stagger()
    {

    }

    public override void Die()
    {
        if(hitStopCoroutine != null) StopCoroutine(hitStopCoroutine);
        hitStopCoroutine = StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        state = State.Die;

        foreach(string s in animationTrigger)
        {
            anim.ResetTrigger(s);
        }

        foreach(string s in animationbool)
        {
            anim.SetBool(s, false);
        }

        Instantiate(dieVFX, transform.position, Quaternion.identity);

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isDie", true);
        yield return new WaitWhile(() => anim.GetBool("isDie"));

        Destroy(gameObject);
    }
}
