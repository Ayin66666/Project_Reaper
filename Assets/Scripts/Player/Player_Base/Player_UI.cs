using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_UI : MonoBehaviour
{
    public Image[] skill_Image;
    public Image[] skill_CoolTime_Image;
    [SerializeField] Image blackScreen;
    [SerializeField] Text youDie;
    [SerializeField] Text pressText;

    public Slider hp_Slider;
    public Slider damageSlider;

    void Awake()
    {
        blackScreen.color = new Color(0, 0, 0, 0);
        youDie.color = new Color(255, 255, 255, 0);
        pressText.color = new Color(255, 255, 255, 0);

        blackScreen.gameObject.SetActive(false);
        youDie.gameObject.SetActive(false);
        pressText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UI_Update();
    }

    void UI_Update()
    {
        // 체력
        hp_Slider.value = (float)(Player_Status.instance.curHp / (float)Player_Status.instance.maxHp);

        // 받은 데미지
        damageSlider.value = Mathf.Lerp(damageSlider.value, Player_Status.instance.getDamaged / Player_Status.instance.maxHp, Time.deltaTime * 5f);

        // 스킬
        if(Player_Status.instance.myColor == Player_Status.Mycolor.Blue)
        {
            skill_Image[0].gameObject.SetActive(true); // White_Skill1
            skill_Image[1].gameObject.SetActive(true); // White_Skill2
            skill_Image[2].gameObject.SetActive(false); // Black_Skill1
            skill_Image[3].gameObject.SetActive(false); // Black_Skill2

            skill_CoolTime_Image[0].fillAmount = Player_Status.instance.blue_Skill1_CoolTime / Player_Status.instance.blue_Skill1_CoolTime_Max;
            skill_CoolTime_Image[1].fillAmount = Player_Status.instance.blue_Skill2_CoolTime / Player_Status.instance.blue_Skill2_CoolTime_Max;
        }
        else
        {
            skill_Image[0].gameObject.SetActive(false); // White_Skill1
            skill_Image[1].gameObject.SetActive(false); // White_Skill2
            skill_Image[2].gameObject.SetActive(true); // Black_Skill1
            skill_Image[3].gameObject.SetActive(true); // Black_Skill2

            skill_CoolTime_Image[2].fillAmount = Player_Status.instance.red_Skill1_CoolTime / Player_Status.instance.red_Skill1_CoolTime_Max;
            skill_CoolTime_Image[3].fillAmount = Player_Status.instance.red_Skill2_CoolTime / Player_Status.instance.red_Skill2_CoolTime_Max;
        }
    }

    public void DieCall()
    {
        StartCoroutine(DieScreenFadeIn());
    }

    public IEnumerator DieScreenFadeIn()
    {
        yield return new WaitForSeconds(1.5f);

        blackScreen.gameObject.SetActive(true);
        youDie.gameObject.SetActive(true);
        pressText.gameObject.SetActive(true);

        float fadeCount = 0;
        while (fadeCount <= 1.0f)
        {
            fadeCount += 0.01f;
            yield return new WaitForSeconds(0.01f);
            blackScreen.color = new Color(0, 0, 0, fadeCount);
            youDie.color = new Color(255, 255, 255, fadeCount);
        }

        // Delay
        yield return new WaitForSeconds(0.15f);

        fadeCount = 0;
        while (fadeCount <= 1.0f)
        {
            fadeCount += 0.01f;
            yield return new WaitForSeconds(0.01f);
            pressText.color = new Color(255, 255, 255, fadeCount);
        }

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            // 시작화면으로 전환
            yield return null;
        }
        Scene_Loading_Manager.ReturnMain();
    }
}
