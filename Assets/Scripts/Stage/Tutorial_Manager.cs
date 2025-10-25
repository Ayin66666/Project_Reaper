using System.Collections;
using UnityEngine;


public class Tutorial_Manager : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private CanvasGroup fadeCanvas;
    private Coroutine fadeCoroutine;
    private bool isFade = false;


    private IEnumerator Fade()
    {
        isFade = true;

        // 페이드 인
        fadeCanvas.gameObject.SetActive(true);
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(0, 1, timer);
            yield return null;
        }

        // 딜레이
        yield return new WaitForSeconds(0.5f);

        // 씬 전환 -> 1스테이지
        Scene_Loading_Manager.LoadScene("Scene_Stage1");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isFade)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(Fade());
        }
    }
}
