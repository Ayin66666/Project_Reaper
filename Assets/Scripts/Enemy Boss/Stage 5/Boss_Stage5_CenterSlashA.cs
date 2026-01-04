using Easing;
using System.Collections;
using UnityEngine;

public class Boss_Stage5_CenterSlashA : MonoBehaviour
{
    [SerializeField] private TrailRenderer trail;

    [Header("---Attack Setting---")]
    [SerializeField] private Transform returnPos;
    [SerializeField] private GameObject target;
    [SerializeField] private float speed;
    public bool isAttack;

    [Header("---Attack Collider---")]
    [SerializeField] private GameObject warringObj;
    [SerializeField] private GameObject attackCollider;

    private void Start()
    {
        trail = GetComponent<TrailRenderer>();
    }

    public void Setting(GameObject target, Vector2 pos, float moveSpeed, float attackDelay)
    {
        if (isAttack)
        {
            return;
        }
        else
        {
            this.target = target;
            isAttack = true;
            speed = moveSpeed;
            StartCoroutine(Move(pos, attackDelay));
        }
    }

    private IEnumerator Move(Vector2 pos, float attackDelay)
    {
        trail.enabled = true;

        Vector2 startPos = transform.position;
        Vector2 endPos = pos;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * speed;
            transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        trail.Clear();
        trail.enabled = false;
        StartCoroutine(Attack(attackDelay));
    }

    private IEnumerator Attack(float attackDelay)
    {
        // Warring
        warringObj.SetActive(true);
        float timer = 0;
        while(timer < attackDelay)
        {
            timer += Time.deltaTime;

            Vector3 dir = (target.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            warringObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
            attackCollider.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            yield return null;
        }
        warringObj.SetActive(false);

        // Attack
        attackCollider.SetActive(true);
        while(attackCollider.activeSelf)
        {
            yield return null;
        }
        isAttack = false;

        // Delay
        yield return new WaitForSeconds(0.1f);

        // Off
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        transform.rotation = Quaternion.identity;
    }
}
