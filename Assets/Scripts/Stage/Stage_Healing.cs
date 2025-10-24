using UnityEngine;


public class Stage_Healing : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private GameObject healingVFX;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            // »∏∫π
            Player_Status status = collision.GetComponent<Player_Status>();
            status.curHp = status.maxHp;

            // ¿Ã∆Â∆Æ
            healingVFX.SetActive(true);
            healingVFX.transform.position = status.gameObject.transform.position;
            healingVFX.transform.parent = status.gameObject.transform;

            Destroy(gameObject);
        }
    }
}
