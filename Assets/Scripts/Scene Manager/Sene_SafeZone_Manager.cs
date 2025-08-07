using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sene_SafeZone_Manager : MonoBehaviour
{
    public static Sene_SafeZone_Manager Instance;

    [Header("---Scene---")]
    [SerializeField] private float waitTime;
    private float curTimer;
    private static string nextScene;

    [Header("---SafeZone Map---")]
    [SerializeField] private GameObject[] mapType;
    private static int nextMapIndex;

    [Header("---Player Check & Setting---")]
    private List<GameObject> playerList = new List<GameObject>();
    private int playerCount;
    [SerializeField] private Vector3 startPos;
    private static Vector3 nextScenePlayerStartpos;

    [Header("---Waiting UI---")]
    [SerializeField] private GameObject waitingUI;
    [SerializeField] private Text waitingText;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // Player Find & Timer Setting
        playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
        curTimer = waitTime;

        // Map Off
        for (int i = 0; i < mapType.Length; i++)
        {
            mapType[i].SetActive(false);
        }

        // Map On
        mapType[nextMapIndex].SetActive(true);
    }

    private void Update()
    {
        Timer();
    }

    public static void NextSceneSetting(string nextSceneName, int mapIndex, Vector3 playerPos)
    {
        nextScene = nextSceneName;
        nextMapIndex = mapIndex;
        nextScenePlayerStartpos = playerPos;
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
            Scene_Loading_Manager.LoadScene(nextScene);
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
                    Scene_Loading_Manager.LoadScene(nextScene);
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
