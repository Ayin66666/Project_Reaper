using Easing;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class Stage_Manager : MonoBehaviour
{
    [Header("---Start Setting---")]
    [SerializeField] private AudioSource bgmSound;
    [SerializeField] private bool haveStartBGM;
    [SerializeField] private string startT1;
    [SerializeField] private string startT2;
    public bool isUI;


    [Header("---Start UI---")]
    [SerializeField] private GameObject startUI;
    [SerializeField] private Image startBorder;
    [SerializeField] private Text startText1;
    [SerializeField] private Text startText2;
    [SerializeField] private CanvasGroup startCanvasGroup;


    [Header("---Room UI---")]
    [SerializeField] private GameObject roomUI;
    [SerializeField] private Image roomBorder;
    [SerializeField] private CanvasGroup roomCanvasGroup;


    [Header("---Clear UI---")]
    [SerializeField] private GameObject cleartUI;
    [SerializeField] private Image claerBorder;
    [SerializeField] private CanvasGroup clearCanvasGroup;


    [Header("---Next Scene Move Setting---")]
    [SerializeField] private string nextScene;
    [SerializeField] private float waitTime;
    [SerializeField] private bool isLastStage;
    [SerializeField] private int safeZoneMapIndex;
    [SerializeField] private Vector3 NextScenePlayerPos;
    private float curTimer;


    [Header("---Player---")]
    public List<GameObject> playerList = new List<GameObject>();
    public int playerCount;


    [Header("---Waiting UI---")]
    [SerializeField] private GameObject waitingUI;
    [SerializeField] private Text waitingText;


    [Header("---Fade---")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    private bool isFade;
    private bool isNext = false;


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

    private void Start()
    {
        // Player Find
        playerCount = GameObject.FindGameObjectsWithTag("Player").Length;

        // Clear Timer Setting
        curTimer = waitTime;

        // BGM Play
        if (haveStartBGM)
        {
            bgmSound.Play();
        }

        // Start UI
        StartCoroutine(StartUI());
    }

    private void Update()
    {
        Timer();
    }


    private IEnumerator StartUI()
    {
        StartCoroutine(Fade(false, 1.25f));
        yield return new WaitWhile(() => isFade);

        isUI = true;
        startUI.SetActive(true);
        startText1.text = startT1;
        startText2.text = startT2;

        // UI On => Border &  MainText & SubText
        float a = 0;
        while (a < 1)
        {
            a += Time.deltaTime / 1f;
            startCanvasGroup.alpha = a;
            yield return null;
        }
        a = 1;
        startCanvasGroup.alpha = a;

        // Delay
        yield return new WaitForSeconds(0.5f);

        // UI Off
        while (a > 0)
        {
            a -= Time.deltaTime / 1.25f;
            startCanvasGroup.alpha = a;
            yield return null;
        }

        startCanvasGroup.alpha = 0;
        startUI.SetActive(false);
        isUI = false;
    }

    public void Room_Clear()
    {
        StartCoroutine(Room_ClearCall());
    }

    private IEnumerator Room_ClearCall()
    {
        isUI = true;
        roomUI.SetActive(true);

        // UI On
        roomCanvasGroup.alpha = 0;
        float a = 0;
        while (a < 1)
        {
            a += Time.deltaTime / 1.25f;
            roomCanvasGroup.alpha = a;
            yield return null;
        }
        a = 1;
        roomCanvasGroup.alpha = a;

        // Delay
        yield return new WaitForSeconds(0.35f);

        // UI Off
        while (a > 0)
        {
            a -= Time.deltaTime;
            roomCanvasGroup.alpha = a;
            yield return null;
        }

        roomCanvasGroup.alpha = 0;
        roomUI.SetActive(false);
        isUI = false;
    }

    public void Stage_Clear()
    {
        StartCoroutine(Stage_ClearCall());
    }

    private IEnumerator Stage_ClearCall()
    {
        isUI = true;
        cleartUI.SetActive(true);

        // UI on => Border &  MainText
        float a = 0;
        while (a < 1)
        {
            a += Time.deltaTime / 1f;
            clearCanvasGroup.alpha = a;
            yield return null;
        }
        a = 1;
        clearCanvasGroup.alpha = a;

        // Delay
        yield return new WaitForSeconds(0.35f);

        a = 1;
        while (a > 0)
        {
            a -= Time.deltaTime / 1f;
            clearCanvasGroup.alpha = a;
            yield return null;
        }

        clearCanvasGroup.alpha = 0;
        cleartUI.SetActive(false);
        isUI = false;
    }

    private IEnumerator Next()
    {
        isNext = true;
        StartCoroutine(Fade(true, 1.25f));
        yield return new WaitWhile(() => isFade);

        if (isLastStage)
        {
            Scene_Loading_Manager.LoadScene(nextScene);
        }
        else
        {
            // 다음 스테이지 입력
            Sene_SafeZone_Manager.NextSceneSetting(nextScene, safeZoneMapIndex, NextScenePlayerPos);

            // 대기실 이동
            Scene_Loading_Manager.LoadScene(nextScene);
        }
    }

    private void Timer()
    {
        // Timer Check
        if (playerList.Count > 0)
        {
            waitingUI.SetActive(true);
            waitingText.text = (int)curTimer + " 초 뒤 다음 스테이지로 넘어갑니다...";
            curTimer -= Time.deltaTime;
        }
        else
        {
            waitingUI.SetActive(false);
            curTimer = waitTime;
        }

        // Next Stage Move
        if (curTimer <= 0)
        {
            if (isLastStage)
            {
                Scene_Loading_Manager.LoadScene(nextScene);
            }
            else
            {
                // 다음 스테이지 입력
                Sene_SafeZone_Manager.NextSceneSetting(nextScene, safeZoneMapIndex, NextScenePlayerPos);

                // 대기실 이동
                Scene_Loading_Manager.LoadScene("Scene_SafeZone");
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isNext)
        {
            StartCoroutine(Next());
           
            /*
            if (!playerList.Contains(collision.gameObject))
            {
                playerList.Add(collision.gameObject);
                if (playerList.Count == playerCount)
                {
                    if (isLastStage)
                    {
                        Scene_Loading_Manager.LoadScene(nextScene);
                    }
                    else
                    {
                        // 다음 스테이지 입력
                        Sene_SafeZone_Manager.NextSceneSetting(nextScene, safeZoneMapIndex, NextScenePlayerPos);

                        // 대기실 이동
                        Scene_Loading_Manager.LoadScene("Scene_SafeZone");
                    }
                }
            }
            */
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (playerList.Contains(collision.gameObject))
            {
                playerList.Remove(collision.gameObject);
            }
        }
    }
}
