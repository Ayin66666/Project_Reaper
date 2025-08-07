using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_Destroy : Platform_Base
{
    [Header("---Setting ( Destroy )---")]
    [SerializeField] private GameObject platformBody;
    [SerializeField] private Animator anim;

    [SerializeField] private float activateTime;
    [SerializeField] private float regenTime;

    private Coroutine destroyCoroutine;

    public override void PlatformActivate(bool isActivate)
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator DestroyTimerOn()
    {
        isActivate = true;

        // Destroy Delay
        yield return new WaitForSeconds(activateTime);
        
        platformBody.SetActive(false);
        destroyCoroutine = StartCoroutine(RegenTimerOn());
    }

    private IEnumerator RegenTimerOn()
    {
        // Regen Delay
        yield return new WaitForSeconds(regenTime);

        isActivate = false;
        platformBody.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !isActivate)
        {
            destroyCoroutine = StartCoroutine(DestroyTimerOn());
        }
    }
}

