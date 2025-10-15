using UnityEngine;


public class Enemy_ExplosionCollider : MonoBehaviour
{
    [Header("---Attack Setting---")]
    [SerializeField] private int damage;
    public AttackType attackType;
    public enum AttackType { Normal, Red, Blue }


    public void Setting(int damage)
    {
        this.damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            switch (attackType)
            {
                case AttackType.Normal:
                    collision.GetComponent<Player_Status>().TakeDamage(damage, 1, false, Player_Status.HitColor.None, Player_Status.HitType.None);
                    break;

                case AttackType.Red:
                    collision.GetComponent<Player_Status>().TakeDamage(damage, 1, false, Player_Status.HitColor.Red, Player_Status.HitType.None);
                    break;

                case AttackType.Blue:
                    collision.GetComponent<Player_Status>().TakeDamage(damage, 1, false, Player_Status.HitColor.Blue, Player_Status.HitType.None);
                    break;
            }
        }
    }
}
