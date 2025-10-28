using System.Collections;
using UnityEngine;


public class Tutorial_Manager : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private CanvasGroup fadeCanvas;
    private Coroutine fadeCoroutine;
    private bool isFade = false;


    [Header("---Title---")]
    [SerializeField] private GameObject titleSet;
    [SerializeField] private CanvasGroup titleCanvasGroup;
    private bool isTitle = true;


    private void Start()
    {
        fadeCoroutine = StartCoroutine(Fade(false));
    }


    private IEnumerator Title()
    {
        titleSet.SetActive(true);
        titleCanvasGroup.alpha = 0;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            titleCanvasGroup.alpha = Mathf.Lerp(0, 1, timer);
            yield return null;
        }
        titleCanvasGroup.alpha = 1;

        yield return new WaitForSeconds(0.5f);

        timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            titleCanvasGroup.alpha = Mathf.Lerp(1, 0, timer);
            yield return null;
        }
        titleCanvasGroup.alpha = 0;
        titleSet.SetActive(false);
    }

    private IEnumerator Fade(bool isOn)
    {
        // ���̵� ��
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

        // �� ��ȯ -> 1��������
        if (isOn)
        {
            // ������
            yield return new WaitForSeconds(0.5f);

            Scene_Loading_Manager.LoadScene("Scene_Stage1");
        }

        if(!isOn && isTitle)
        {
            isTitle = false;
            StartCoroutine(Title());
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
