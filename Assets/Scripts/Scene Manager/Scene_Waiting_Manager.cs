using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scene_Waiting_Manager : MonoBehaviour
{
    [Header("---Scene---")]
    [SerializeField] private string nextScene;
    [SerializeField] private float waitTime;
    private float curTimer;

    [Header("---Player Check---")]
    private List<GameObject> playerList = new List<GameObject>();
    private int playerCount;

    [Header("---Waiting UI---")]
    [SerializeField] private GameObject waitingUI;
    [SerializeField] private Text waitingText;

    private void Awake()
    {
        playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
        curTimer = waitTime;
    }

    private void Update()
    {
        Timer();
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
