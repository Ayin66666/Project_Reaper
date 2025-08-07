using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Click_ESC();
        }
    }

    public void Click_ESC()
    {
        if(isroomOn)
        {
            isroomOn = false;
            roomUI.SetActive(false);
        }

        if(isOptionOn)
        {
            isOptionOn = false;
            optionUI.SetActive(false);
        }

        if(exitUI)
        {
            isExitOn = false;
            exitUI.SetActive(false);
        }
    }

    public void Click_Start()
    {
        isroomOn = true;
        roomUI.SetActive(true);
    }

    public void Click_Room()
    {
        // Sene_SafeZone_Manager.NextSceneSetting("Scene_Stage1", 0, new Vector3(-32, 4.5f, 0));
        Scene_Loading_Manager.LoadScene("Scene_Waiting");
    }

    public void Click_Option()
    {
        isOptionOn = true;
        optionUI.SetActive(true);
    }

    public void Click_Exit()
    {
        isExitOn = true;
        exitUI.SetActive(true);
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
