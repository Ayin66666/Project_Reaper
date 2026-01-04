using UnityEngine;

public class SwordAuraAnim : MonoBehaviour
{
    [SerializeField] private Animator anim;

    public void HitOver()
    {
        anim.SetBool("isHit", false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            anim.SetTrigger("Hit");
            anim.SetBool("isHit", true);
        }
    }
}
