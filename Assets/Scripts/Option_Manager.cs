using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Option_Manager : MonoBehaviour
{
    public static Option_Manager instance;

    [Header("=== Sound Setting ===")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider master_slider;
    [SerializeField] private Slider bgm_slider;
    [SerializeField] private Slider sfx_slider;


    [Header("=== FPS Setting ===")]
    [SerializeField] private Dropdown fpsDropDown;


    // (장비, 인벤, 스킬, 옵션) 창
    [Header("=== Option UI ===")]
    public GameObject menuSet;
    public GameObject optionSet;
    public GameObject exitSet;
    [SerializeField] private Toggle fpsToggle;
    private float deltaTime = 0f;
    [SerializeField] private GameObject option_BackGround;



    // 여기 두개 불값으로 옵션창 켜진거 확인 & 옵션창에서 나가기 버튼 누른건지 확인
    public bool isOptionOn;
    public bool isExitOn;
    public bool isFpsOn;
    public bool isMenuOn;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        master_slider.onValueChanged.AddListener(SetMasterVolume);
        bgm_slider.onValueChanged.AddListener(SetBgmVolume);
        sfx_slider.onValueChanged.AddListener(SetSfxVolume);

        fpsDropDown.value = 1;
    }

    private void Update()
    {
        //FPS 표시 및 설정용
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMenuOn)
            {
                Off_Menu();
            }
            else
            {
                On_Menu();
            }
        }
    }

    public void On_Menu()
    {
        menuSet.SetActive(true);
        isMenuOn = true;

        if (SceneManager.GetActiveScene().name == "Scene_Start")
        {
            option_BackGround.SetActive(true);
        }
        else
        {
            option_BackGround.SetActive(true);
            exitSet.SetActive(false);
        }
    }

    public void Off_Menu()
    {
        menuSet.SetActive(false);
        isMenuOn = false;
    }

    public void Menu_Click_Exit()
    {
        if (SceneManager.GetActiveScene().name == "Scene_Start")
        {
            Off_Menu();
            Sound_Manager.instance.SFXPlay_OneShot(Sound_Manager.instance.onClick);
        }
        else
        {
            isExitOn = true;
            exitSet.SetActive(true);
            Sound_Manager.instance.SFXPlay_OneShot(Sound_Manager.instance.onClick);
        }
    }

    public void Menu_Click_ExitOff()
    {
        isExitOn = false;
        exitSet.SetActive(false);
        Sound_Manager.instance.SFXPlay_OneShot(Sound_Manager.instance.onClick);
    }

    public void Menu_Click_Option()
    {
        //옵션창 키기
        OnOff_Option(true);

        // 클릭 사운드
        Sound_Manager.instance.SFXPlay_OneShot(Sound_Manager.instance.onClick);
    }

    public void OnOff_Option(bool isOn)
    {
        isOptionOn = isOn;
        optionSet.SetActive(isOn);
    }


    public void Click_Start()
    {
        Scene_Loading_Manager.LoadScene("Scene_Stage1");
    }

    public void Exit_Game()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    #region FPS Setting

    public void FpsOn()
    {
        if (fpsToggle.isOn)
        {
            isFpsOn = true;
        }
        else
        {
            isFpsOn = false;
        }
    }

    private void OnGUI()
    {
        if (isFpsOn && SceneManager.GetActiveScene().name != "Loading_Scene")
        {
            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(30, 30, Screen.width, Screen.height);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = 25;
            style.normal.textColor = Color.red;

            float ms = deltaTime * 1000f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.} FPS ({1:0.0} ms)", fps, ms);

            GUI.Label(rect, text, style);
        }
        else
        {
            return;
        }
    }

    public void SetFPS(int value)
    {
        if (fpsDropDown.value == 0)
        {
            value = 30;
        }
        else if (fpsDropDown.value == 1)
        {
            value = 60;
        }
        else if (fpsDropDown.value == 2)
        {
            value = 90;
        }
        else if (fpsDropDown.value == 3)
        {
            value = 120;
        }
        else if (fpsDropDown.value == 4)
        {
            value = 144;
        }
        else if (fpsDropDown.value == 5)
        {
            value = 165;
        }

        Application.targetFrameRate = value;
    }
    #endregion


    #region Sound Setting
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }

    public void SetBgmVolume(float volume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    }
    public void SetSfxVolume(float volume)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }
    #endregion
}
