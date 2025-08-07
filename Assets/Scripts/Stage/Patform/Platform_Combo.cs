using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_Combo : Platform_Base
{
    [Header("---Setting ( Combo )---")]
    [SerializeField] private GameObject[] platformBodys;
    [SerializeField] private GameObject startPlatformBody;
    [SerializeField] private Platform_Check[] platformChecks;

    [SerializeField] private float activateTime;
    [SerializeField] private int curActivatePlatform;
    [SerializeField] private float curTimer;

    private void Update()
    {
        PlatformOff();
    }

    public override void PlatformActivate(bool isActivate)
    {
        // this.isActivate = isActivate;
    }

    public void NextPlatformOn(int platofrmIndex)
    {
        isActivate = true;
        curTimer = activateTime;
        platformBodys[platofrmIndex].SetActive(true);
    }

    private void PlatformOff()
    {
        if (curTimer > 0)
        {
            curTimer -= Time.deltaTime;
        }

        if (curTimer <= 0 && isActivate)
        {
            for (int i = 0; i < platformBodys.Length; i++)
            {
                platformBodys[i].SetActive(false);
            }

            isActivate = false;
            curTimer = 0;
        }
    }
}
