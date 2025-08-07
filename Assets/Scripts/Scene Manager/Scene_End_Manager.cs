using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scene_End_Manager : MonoBehaviour
{
    [SerializeField] private Text MainText;
    [SerializeField] private Text PressText;
    [SerializeField] private Text ThankYouText;

    void Start()
    {
        StartCoroutine(nameof(Fade));
    }

    private IEnumerator Fade()
    {
        // Main Text
        float a = 0;
        while (a < 1)
        {
            a += Time.deltaTime;
            MainText.color = new Color(MainText.color.r, MainText.color.g, MainText.color.b, a);
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(3f);

        a = 1;
        while (a > 0)
        {
            a -= Time.deltaTime;
            MainText.color = new Color(MainText.color.r, MainText.color.g, MainText.color.b, a);
            yield return null;
        }

        // Thank You Text
        a = 0;
        while (a < 1)
        {
            a += Time.deltaTime;
            ThankYouText.color = new Color(PressText.color.r, PressText.color.g, PressText.color.b, a);
            yield return null;
        }

        // Press Text
        a = 0;
        while (a < 1)
        {
            a += Time.deltaTime;
            PressText.color = new Color(PressText.color.r, PressText.color.g, PressText.color.b, a); 
            yield return null;
        }

        // Wait Input
        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        // Retrun Main
        Scene_Loading_Manager.LoadScene("Scene_Start");
    }
}
