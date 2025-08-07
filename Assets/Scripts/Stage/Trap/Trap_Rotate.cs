using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing;
using UnityEngine.Assertions.Must;

public class Trap_Rotate : Trap_Base
{
    [Header("--- Rotate Setting ---")]
    [SerializeField] private bool isRight;
    [SerializeField] private int damage;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float rotateDelay;
    [SerializeField] private int[] rotate;
    [SerializeField] private GameObject attackCollider;
    private Coroutine curCoroutine;

    private void Start()
    {   
        if (isActivate)
        {
            TrapActivate(true);
        }
    }

    public override void TrapActivate(bool activate)
    {
        isActivate = activate;
        if(isActivate)
        {
            curCoroutine = StartCoroutine(Rotate());
            attackCollider.SetActive(true);
        }
        else
        {
            StopCoroutine(curCoroutine);
            attackCollider.SetActive(false);
        }
    }

    private IEnumerator Rotate()
    {
        int a = isRight ? 0 : 1;
        while (isActivate)
        {
            // Rotate Setting
            Quaternion startRot = transform.rotation;
            Quaternion endRot = Quaternion.Euler(0, 0, (a % 2 == 0 ? rotate[0] : rotate[1]));
            a++;

            // Move
            float timer = 0;
            while (timer < 1)
            {

                timer += Time.deltaTime * rotateSpeed;
                transform.rotation = Quaternion.Lerp(startRot, endRot, EasingFunctions.OutSine(timer));
                yield return null;
            }

            // Delay
            yield return new WaitForSeconds(rotateDelay);
        }
    }
}
