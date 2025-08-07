using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing;

public class Trap_Flames : Trap_Base
{
    [Header("---Flames Setting---")]
    [SerializeField] private float moveTime;
    [SerializeField] private GameObject body;
    [SerializeField] private Transform[] movePos;
    private Coroutine curCoroutine;

    public override void TrapActivate(bool activate)
    {
        this.isActivate = activate;
        if(isActivate)
        {
            curCoroutine = StartCoroutine(Move());
        }
        else
        {
            Over();
        }
    }

    private IEnumerator Move()
    {
        Vector2 startPos = movePos[0].position;
        Vector2 endPos = movePos[1].position;
        float timer = 0;

        while(timer < 1 && isActivate)
        {
            timer += Time.deltaTime / moveTime;
            body.transform.position = Vector2.Lerp(startPos, endPos, timer);
            yield return null;
        }
    }

    private void Over()
    {
        StopCoroutine(curCoroutine);
    }
}
