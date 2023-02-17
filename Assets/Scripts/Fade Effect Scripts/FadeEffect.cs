using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    //public Color firstColor;
    //public Color lastColor;
    public Image fadedScreen;
    public GameObject fader;

    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeInRoutine()
    {
        fader.SetActive(true);
        //fadedScreen.color = firstColor;
        fadedScreen.color = new Color(0, 0, 0, 1);

        float duration = 2.0f;
        float currentTime = 0f;

        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, currentTime / duration);
            fadedScreen.color = new Color(fadedScreen.color.r, fadedScreen.color.g, fadedScreen.color.b, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }
        fader.SetActive(false);
    }

    IEnumerator FadeOutRoutine()
    {
        fader.SetActive(true);
        fadedScreen.color = new Color(0, 0, 0, 0);
        //fadedScreen.color = lastColor;

        float duration = 2.0f;
        float currentTime = 0f;

        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, currentTime / duration);
            fadedScreen.color = new Color(fadedScreen.color.r, fadedScreen.color.g, fadedScreen.color.b, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }
    }
}
