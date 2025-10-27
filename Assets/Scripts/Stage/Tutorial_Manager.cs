using System.Collections;
using UnityEngine;


public class Tutorial_Manager : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private CanvasGroup fadeCanvas;
    private Coroutine fadeCoroutine;
    private bool isFade = false;


    private void Start()
    {
        fadeCoroutine = StartCoroutine(Fade(false));
    }


    private IEnumerator Fade(bool isOn)
    {
        // 페이드 인
        fadeCanvas.gameObject.SetActive(true);
        float start = isOn ? 0 : 1;
        float end = isOn ? 1 : 0;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(start, end, timer);
            yield return null;
        }

        // 씬 전환 -> 1스테이지
        if (isOn)
        {
            // 딜레이
            yield return new WaitForSeconds(0.5f);

            Scene_Loading_Manager.LoadScene("Scene_Stage1");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isFade)
        {
            isFade = true;
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(Fade(true));
        }
    }
}
