using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Easing;


public class DamageUI : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Text damageText;
    private bool isCritical;


    public void DamageMove(bool isCritical, int damage)
    {
        this.isCritical = isCritical;
        damageText.text = damage.ToString();

        StartCoroutine(Effct());
    }

    private IEnumerator Effct()
    {
        yield return new WaitForSeconds(0.25f);

        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer);
            yield return null;
        }
        canvasGroup.alpha = 0;

        // Destroy
        Destroy(gameObject);
    }
}
