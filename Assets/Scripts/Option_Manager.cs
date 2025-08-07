using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Option_Manager : MonoBehaviour
{
    public static Option_Manager instance;

    [Header("---Sound UI---")]
    [SerializeField] private GameObject optionUI;
    [SerializeField] private AudioMixer mixer;

    [Header("---Sound Setting---")]
    [SerializeField] private float masterSound;
    [SerializeField] private float bgmSound;
    [SerializeField] private float sfxSound;
    [SerializeField] private bool masterSoundOn;
    [SerializeField] private bool bgmSoundOn;
    [SerializeField] private bool sfxSoundOn;
    [SerializeField] private bool isOptionOn;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        StartSoundOn();
        DontDestroyOnLoad(gameObject);
    }

    private void FixedUpdate()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        int sceneBuildIndex = currentScene.buildIndex;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(sceneBuildIndex == 0)
            {
                if(isOptionOn)
                {
                    Check_Out();
                }
                else
                {
                    OptionOn();
                }
            }
            else
            {
                Check_Out();
            }
        }
    }

    // Sound Setting
    #region
    private void StartSoundOn()
    {
        masterSoundOn = true;
        bgmSoundOn = true;
        sfxSoundOn = true;

        masterSound = 1f;
        mixer.SetFloat("Master", Mathf.Log10(masterSound) * 20);

        bgmSound = 1f;
        mixer.SetFloat("BGM", Mathf.Log10(bgmSound) * 20);

        sfxSound = 1f;
        mixer.SetFloat("SFX", Mathf.Log10(sfxSound) * 20);
    }

    public void Toggle_Master(bool val)
    {
        masterSoundOn = val;
        if (masterSoundOn)
        {
            mixer.SetFloat("Master", Mathf.Log10(masterSound) * 20);
        }
        else
        {
            mixer.SetFloat("Master", 0.01f);
        }
    }

    public void Toggle_BGM(bool val)
    {
        bgmSoundOn = val;
        if (bgmSoundOn)
        {
            mixer.SetFloat("BGM", Mathf.Log10(bgmSound) * 20);
        }
        else
        {
            mixer.SetFloat("BGM", 0.01f);
        }
    }

    public void Toggle_SFX(bool val)
    {
        sfxSoundOn = val;
        if (sfxSoundOn)
        {
            mixer.SetFloat("SFX", Mathf.Log10(sfxSound) * 20);
        }
        else
        {
            mixer.SetFloat("SFX", 0.01f);
        }
    }

    public void Sound_Master(float val)
    {
        masterSound = val;
        if (masterSoundOn)
        {
            mixer.SetFloat("Master", Mathf.Log10(masterSound) * 20);
        }
    }

    public void Sound_BGM(float val)
    {
        bgmSound = val;
        if (bgmSoundOn)
        {
            mixer.SetFloat("BGM", Mathf.Log10(bgmSound) * 20);
        }
    }

    public void Sound_SFX(float val)
    {
        sfxSound = val;
        if (sfxSoundOn)
        {
            mixer.SetFloat("SFX", Mathf.Log10(sfxSound) * 20);
        }
    }

    public void OptionOn()
    {
        if(!isOptionOn)
        {
            isOptionOn = true;
            optionUI.SetActive(true);
        }
    }

    public void Check_Out()
    {
        if(isOptionOn)
        {
            isOptionOn = false;
            optionUI.SetActive(false);
        }
    }
    #endregion
}
