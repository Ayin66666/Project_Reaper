using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Easing;

public class Scene_Loading_Manager : MonoBehaviour
{
    public static Scene_Loading_Manager instance;

    [SerializeField] static string nextScene;
    public static int curStage;
    private bool isLoading;

    [Header("---Loading Image---")]
    [SerializeField] private Slider progressbar;
    [SerializeField] private Text loadText;
    [SerializeField] private Text tipText;
    [SerializeField] private string[] texts;


    [Header("---Fade---")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    private bool isFade;


    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        StartCoroutine(Fade(false, 0.5f));
        StartCoroutine(Loading());
    }


    public static void LoadScene(string sceneName)
    {
        // Next Scene Name Setting 
        curStage++;
        nextScene = sceneName;
        SceneManager.LoadScene("Scene_Loading");
    }

    public static void ReturnMain()
    {
        Time.timeScale = 1;
        curStage = 0;
        nextScene = "Scene_Start";
        SceneManager.LoadScene("Scene_Loading");
    }

    private IEnumerator Fade(bool isOn, float speed)
    {
        isFade = true;
        fadeCanvasGroup.gameObject.SetActive(true);
        float start = isOn ? 0 : 1;
        float end = isOn ? 1 : 0;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / speed;
            fadeCanvasGroup.alpha = Mathf.Lerp(start, end, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        fadeCanvasGroup.alpha = end;

        if (!isOn) fadeCanvasGroup.gameObject.SetActive(false);
        isFade = false;
    }

    private IEnumerator Tip()
    {
        // Tip Text Fade Setting
        tipText.color = new Color(tipText.color.r, tipText.color.g, tipText.color.b, 0);
        while (isLoading)
        {
            tipText.text = texts[Random.Range(0, texts.Length)];

            float a = 0;
            while (a < 1)
            {
                tipText.color = new Color(tipText.color.r, tipText.color.g, tipText.color.b, a);
                a += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(2f);

            a = 1;
            while (a > 0)
            {
                tipText.color = new Color(tipText.color.r, tipText.color.g, tipText.color.b, a);
                a -= Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.75f);
        }
    }

    private IEnumerator Loading()
    {
        Cursor.lockState = CursorLockMode.None;

        Debug.Log(nextScene);
        isLoading = true;
        StartCoroutine(nameof(Tip));
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);
        operation.allowSceneActivation = false;


        while (!operation.isDone)
        {
            yield return null;
            if (progressbar.value < 0.9f)
            {
                Debug.Log("call Vel up");
                progressbar.value = Mathf.MoveTowards(progressbar.value, 0.9f, Time.deltaTime);
            }
            else if (operation.progress >= 0.9f)
            {
                Debug.Log("call Vel up2");
                progressbar.value = Mathf.MoveTowards(progressbar.value, 1f, Time.deltaTime);
            }

            if (progressbar.value >= 1f)
            {
                Debug.Log("call Vel up3");
                loadText.text = "Press SpaceBar to Start.";
            }

            bool isInput = false;
            if (Input.GetKeyDown(KeyCode.Space) && progressbar.value >= 1f && operation.progress >= 0.9f && !isInput)
            {
                isInput = true;

                // Fade
                StartCoroutine(Fade(true, 1.25f));
                yield return new WaitWhile(() => isFade);

                // Scene Move
                isLoading = false;
                operation.allowSceneActivation = true;
            }
        }
    }
}
