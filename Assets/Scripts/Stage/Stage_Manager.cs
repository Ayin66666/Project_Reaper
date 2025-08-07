using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage_Manager : MonoBehaviour
{
    [Header("---Start Setting---")]
    [SerializeField] private AudioSource bgmSound;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private bool haveStartBGM;
    public bool isUI;


    [Header("---Start UI---")]
    [SerializeField] private GameObject startUI;
    [SerializeField] private Image startBorder;
    [SerializeField] private Text startMText;
    [SerializeField] private Text startSText;


    [Header("---Room UI---")]
    [SerializeField] private GameObject roomUI;
    [SerializeField] private Image roomBorder;
    [SerializeField] private Text roomMText;
    [SerializeField] private Text roomsText;


    [Header("---Clear UI---")]
    [SerializeField] private GameObject cleartUI;
    [SerializeField] private Image claerBorder;
    [SerializeField] private Text claerMText;
    [SerializeField] private Text claerSText;


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


    private void Start()
    {
        // Player Find
        playerCount = GameObject.FindGameObjectsWithTag("Player").Length;

        // Clear Timer Setting
        curTimer = waitTime;

        // BGM Play
        if(haveStartBGM)
        {
            bgmSound.clip = audioClips[0];
            bgmSound.Play();
        }

        // Start UI
        StartCoroutine(StartUI());
    }

    private void Update()
    {
        Timer();
    }

    public void BossRoom_Sound()
    {
        bgmSound.Stop();
        bgmSound.clip = audioClips[1];
        bgmSound.Play();
    }

    private IEnumerator StartUI()
    {
        isUI = true;
        startUI.SetActive(true);

        // UI On => Border &  MainText
        float a = 0;
        while(a < 1)
        {
            a += Time.deltaTime;
            startBorder.color =new Color(startBorder.color.r, startBorder.color.g, startBorder.color.b, a);
            startMText.color =new Color(startMText.color.r, startMText.color.g, startMText.color.b, a);
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.5f);

        // UI On = SubText
        a = 0;
        while(a < 1)
        {
            a += Time.deltaTime;
            startSText.color = new Color(startSText.color.r, startSText.color.g, startSText.color.b, a);
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.75f);

        a = 1;
        while (a > 0)
        {
            a -= Time.deltaTime * 0.75f;
            startBorder.color = new Color(startBorder.color.r, startBorder.color.g, startBorder.color.b, a);
            startMText.color = new Color(startMText.color.r, startMText.color.g, startMText.color.b, a);
            startSText.color = new Color(startSText.color.r, startSText.color.g, startSText.color.b, a);
            yield return null;
        }

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
        float a = 0;
        while (a < 1)
        {
            a += Time.deltaTime * 0.75f;
            roomMText.color = new Color(roomMText.color.r, roomMText.color.g, roomMText.color.b, a);
            roomsText.color = new Color(roomsText.color.r, roomsText.color.g, roomsText.color.b, a);
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.5f);

        // UI Off
        a = 1;
        while (a > 0)
        {
            a -= Time.deltaTime;
            roomMText.color = new Color(roomMText.color.r, roomMText.color.g, roomMText.color.b, a);
            roomsText.color = new Color(roomsText.color.r, roomsText.color.g, roomsText.color.b, a);
            yield return null;
        }

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
        while(a < 1)
        {
            a += Time.deltaTime;
            claerBorder.color = new Color(claerBorder.color.r, claerBorder.color.g, claerBorder.color.b, a);
            claerMText.color = new Color(claerMText.color.r, claerMText.color.g, claerMText.color.b, a);
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.25f);

        // UI On = SubText
        a = 0;
        while(a < 1)
        {
            a += Time.deltaTime;
            claerSText.color = new Color(claerSText.color.r, claerSText.color.g, claerSText.color.b, a);
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.25f);

        a = 1;
        while (a > 0)
        {
            a -= Time.deltaTime;
            claerBorder.color = new Color(claerBorder.color.r, claerBorder.color.g, claerBorder.color.b, a);
            claerMText.color = new Color(claerMText.color.r, claerMText.color.g, claerMText.color.b, a);
            claerSText.color = new Color(claerSText.color.r, claerSText.color.g, claerSText.color.b, a);
            yield return null;
        }

        cleartUI.SetActive(false);
        isUI = false;
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
            if(isLastStage)
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
        if (collision.CompareTag("Player"))
        {
            if (!playerList.Contains(collision.gameObject))
            {
                playerList.Add(collision.gameObject);
                if (playerList.Count == playerCount)
                {
                    if(isLastStage)
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
