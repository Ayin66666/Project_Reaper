using UnityEngine;


public abstract class Attack_Base : MonoBehaviour
{
    [Header("---Status---")]
    public bool isUsed;
    [SerializeField] protected LayerMask groundLayer;
    protected Coroutine useCoroutine;


    [Header("---Component---")]
    [SerializeField] protected GameObject body;
    [SerializeField] protected Animator anim;


    public abstract void Use();

    public abstract void Reset();
}
