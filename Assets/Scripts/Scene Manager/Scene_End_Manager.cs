using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Scene_End_Manager : MonoBehaviour
{
    [SerializeField] private Image game_Title;
    [SerializeField] private Text madeByText;
    [SerializeField] private Text mainText;
    [SerializeField] private Text pressText;
    [SerializeField] private Text thankYouText;

    Coroutine myCoroutine;

    void Start()
    {
        StartCoroutine(nameof(Fade));
    }

    private IEnumerator Fade()
    {
        float a = 1;
        yield return new WaitForSeconds(1f);

        // Game Title
        game_Title.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);

        while (a > 0)
        {
            a -= Time.deltaTime;
            game_Title.color = new Color(game_Title.color.r, game_Title.color.g, game_Title.color.b, a);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // Made By Text
        while (a < 1)
        {
            a += Time.deltaTime;
            madeByText.color = new Color(madeByText.color.r, madeByText.color.g, madeByText.color.b, a);
            yield return null;
        }

        // Main Text
        a = 0;
        while (a < 1)
        {
            a += Time.deltaTime;
            mainText.color = new Color(mainText.color.r, mainText.color.g, mainText.color.b, a);
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(3f);

        a = 1;
        while (a > 0)
        {
            a -= Time.deltaTime;
            madeByText.color = new Color(madeByText.color.r, madeByText.color.g, madeByText.color.b, a);
            mainText.color = new Color(mainText.color.r, mainText.color.g, mainText.color.b, a);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // Thank You Text
        a = 0;
        while (a < 1)
        {
            a += Time.deltaTime;
            thankYouText.color = new Color(pressText.color.r, pressText.color.g, pressText.color.b, a);
            yield return null;
        }

        // Press Text
        a = 0;
        while (a < 1)
        {
            a += Time.deltaTime;
            pressText.color = new Color(pressText.color.r, pressText.color.g, pressText.color.b, a);
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
