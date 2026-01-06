using System.Collections;
using UnityEngine;
using Easing;



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


    [Header("---Movement---")]
    [SerializeField] private Vector2 moveSpeed;
    [SerializeField] private Vector2 delay;
    [SerializeField] private RectTransform camMovePos;
    [SerializeField] private RectTransform background;


    [Header("---Side Fade---")]
    [SerializeField] private CanvasGroup sideFade;


    private void Start()
    {
        StartCoroutine(Fade(false));
        // StartCoroutine(Background());
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Click_ESC();
        }
    }

    private IEnumerator Background()
    {
        while (true)
        {
            int ran = Random.Range(3, 5);
            for (int i = 0; i < ran; i++)
            {
                Vector2 startPos = background.anchoredPosition;
                Vector2 endPos = GetmovePos();
                float timer = 0;
                float speed = Random.Range(moveSpeed.x, moveSpeed.y);

                while (timer < 1)
                {
                    timer += Time.deltaTime / speed;
                    background.anchoredPosition = Vector2.Lerp(startPos, endPos, timer);
                    yield return null;
                }
            }

            yield return new WaitForSeconds(Random.Range(delay.x, delay.y));
        }
    }

    private IEnumerator SideFade()
    {
        float start = sideFade.alpha;
        float end = sideFade.alpha + Random.Range(-0.3f, 0.3f);
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private Vector2 GetmovePos()
    {
        float x = Random.Range(-camMovePos.sizeDelta.x / 2, camMovePos.sizeDelta.x / 2);
        float y = Random.Range(-camMovePos.sizeDelta.y / 2, camMovePos.sizeDelta.y / 2);
        Vector2 movePos = new Vector2(x, y);

        return movePos;
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
        //Scene_Loading_Manager.LoadScene("Scene_Tutorial");
        Scene_Loading_Manager.LoadScene("Scene_Stage5");
    }

    public void Click_Room()
    {
        //Scene_Loading_Manager.LoadScene("Scene_Tutorial");
        Scene_Loading_Manager.LoadScene("Scene_Stage5");
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
