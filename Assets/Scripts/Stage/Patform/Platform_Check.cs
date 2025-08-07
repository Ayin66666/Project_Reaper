using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_Check : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private Type type;
    [SerializeField] private Platform_Combo platform;
    [SerializeField] private int nextPlatformIndex;
    [SerializeField] private bool isActivate;
    private enum Type { First, Normal, Last }

    private void OnDisable()
    {
        isActivate = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (type)
        {
            case Type.First:
                if (collision.CompareTag("Player") && !isActivate)
                {
                    platform.NextPlatformOn(nextPlatformIndex);
                }
                break;

            case Type.Normal:
                if (collision.CompareTag("Player") && !isActivate)
                {
                    isActivate = true;
                    platform.NextPlatformOn(nextPlatformIndex);
                }
                break;

            case Type.Last:
                break;
        }
    }
}
