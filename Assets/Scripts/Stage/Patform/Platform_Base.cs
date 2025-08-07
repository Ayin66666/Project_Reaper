using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Platform_Base : MonoBehaviour
{
    [Header("---Compoment---")]
    [SerializeField] protected Collider2D colider;
    [SerializeField] protected Rigidbody2D rigid;

    [Header("---Setting ( Base )---")]
    [SerializeField] public bool isActivate;
    [SerializeField] public bool isReset;
    [SerializeField] public PlatformType platformtype;

    public enum PlatformType { Movement, Destroy, Combo }

    public abstract void PlatformActivate(bool isActivate);
}
