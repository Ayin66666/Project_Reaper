using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_AOE : Attack_Base
{
    [Header("----Attack Setting---")]
    [SerializeField] private GameObject[] attackCollider;
    [SerializeField] private Transform[] movePos;


    // 돌진 - 지상 난무 - 공중 텔포 - 공중 난무 - 텔포
    public override void Use()
    {
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        isUsed = true;

        // 돌진

        // 지상 난무

        // 텔포

        // 공중 난무

        // 텔포
        yield return null;
        isUsed = false;
    }

    public override void Reset()
    {
        if (useCoroutine != null) StopCoroutine(useCoroutine);
        foreach(GameObject obj in attackCollider)
        {
            obj.SetActive(false);
        }
    }
}
