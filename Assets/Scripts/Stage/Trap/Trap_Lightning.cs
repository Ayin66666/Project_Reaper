using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Trap_Lightning : Trap_Base
{
    [Header("--- Lightning Pos ---")]
    [SerializeField] private float delay;
    [SerializeField] private float nextAttackDelay;
    [SerializeField] private GameObject[] lightning;
    private Coroutine curCoroutine;

    public override void TrapActivate(bool activate)
    {
        isActivate = activate;
        if (isActivate)
        {
            curCoroutine = StartCoroutine(Lightning());
        }
        else
        {
            StopCoroutine(curCoroutine);
        }
    }

    private IEnumerator Lightning()
    {
        while(isActivate)
        {
            for(int i = 0; i < lightning.Length; i++)
            {
                // Attack
                // ���߿� ��ƼŬ �ý������� ����ٸ� �̰� ����� ��!
                lightning[i].SetActive(true);
                while (lightning[i].activeSelf)
                {
                    yield return null;
                }
            }

            // Delay
            yield return new WaitForSeconds(delay);
        }
    }
}
