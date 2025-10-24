using System.Collections;
using UnityEngine;
using Easing;
using Unity.VisualScripting.Antlr3.Runtime;


public class Scene_Start_Manager : MonoBehaviour
{
    [Header("---UI state---")]
    [SerializeField] private bool isroomOn;
    [SerializeField] private bool isOptionOn;
    [SerializeField] private bool isExitOn;


    [Header("---UI---")]
    [SerializeField] private GameObject roomUI;
    [SerializeField] private GameObject optionUI;
    [SerializeField] private GameObject exitUI;


    [Header("---Fade---")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    private bool isFade;


    private void Start()
    {
        StartCoroutine(Fade(false));
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Click_ESC();
        }
    }


    private IEnumerator Fade(bool isOn)
    {
        isFade = true;
        fadeCanvasGroup.gameObject.SetActive(true);
        float start = isOn ? 0 : 1;
        float end = isOn ? 1 : 0;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / 1.25f;
            fadeCanvasGroup.alpha = Mathf.Lerp(start, end, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        fadeCanvasGroup.alpha = end;

        if (!isOn) fadeCanvasGroup.gameObject.SetActive(false);
        isFade = false;
    }

    public void Click_ESC()
    {
        if (isroomOn)
        {
            isroomOn = false;
            roomUI.SetActive(false);
        }

        if (isOptionOn)
        {
            isOptionOn = false;
            optionUI.SetActive(false);
        }

        if (exitUI)
        {
            isExitOn = false;
            exitUI.SetActive(false);
        }
    }

    public void Click_Start()
    {
        StartCoroutine(StartCall());
    }

    private IEnumerator StartCall()
    {
        StartCoroutine(Fade(true));
        yield return new WaitWhile(() => isFade);
        Scene_Loading_Manager.LoadScene("Scene_Stage1");
    }

    public void Click_Room()
    {
        Scene_Loading_Manager.LoadScene("Scene_Stage1");
    }

    public void Click_Option()
    {
        isOptionOn = true;
        optionUI.SetActive(true);
        Sound_Manager.instance.SFXPlay_OneShot(Sound_Manager.instance.onClick);
    }

    public void Click_Exit()
    {
        isExitOn = true;
        exitUI.SetActive(true);
        Sound_Manager.instance.SFXPlay_OneShot(Sound_Manager.instance.onClick);
    }

    public void Click_ExitOff()
    {
        isExitOn = false;
        exitUI.SetActive(false);
        Sound_Manager.instance.SFXPlay_OneShot(Sound_Manager.instance.onClick);
    }

    public void Click_GameOut()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
