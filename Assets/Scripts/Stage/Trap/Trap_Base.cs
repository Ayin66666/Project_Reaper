using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trap_Base : MonoBehaviour
{
    [Header("---Trap Base Setting---")]
    [SerializeField] public TrapType trapType;
    [SerializeField] protected bool isActivate;
    [SerializeField] private bool isStartActivate;

    public enum TrapType { Shooter, Spike, electric, Rotate, Flames }

    private void Start()
    {
        if(isStartActivate)
        {
            TrapActivate(true);
        }
    }

    public abstract void TrapActivate(bool activate);
}
