using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_Boss5_New : Enemy_Base
{
    [Header("---State---")]
    [SerializeField] private int attackCount;


    [Header("---Attack---")]
    [SerializeField] private List<Attack_Base> attack;
    

    [Header("---Object---")]
    [SerializeField] private GameObject container;
    private Coroutine movementCoroutine;


    private void Think()
    {
        state = State.Think;

        if(attackCount >= 5)
        {
            // �ʻ� ����
        }
        else
        {
            // �Ϲ� ����
        }
    }


    /// <summary>
    /// �ٵ� Ȱ��ȭ & ��Ȱ��ȭ
    /// </summary>
    /// <param name="isOn"></param>
    public void Body_Setting(bool isOn)
    {

    }

    #region �⺻ ����
    protected override void Spawn()
    {
        movementCoroutine = StartCoroutine(SpawnCall());
    }

    private IEnumerator SpawnCall()
    {
        state = State.Spawn;
        anim.SetTrigger("Trigger");
        anim.SetBool("isSpawn", true);
        yield return new WaitWhile(() => anim.GetBool("isSpawn"));

        state = State.Idle;
        Think();
    }

    protected override void Stagger()
    {
        // ���� ����
    }

    public override void Die()
    {
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);
        StopAllCoroutines();

        movementCoroutine = StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        state = State.Die;
        anim.SetTrigger("Trigger");
        anim.SetBool("isDie", true);
        yield return new WaitWhile(() => anim.GetBool("isDie"));

        // Collider Off

        // Destroy
        Destroy(container);
    }
    #endregion
}
