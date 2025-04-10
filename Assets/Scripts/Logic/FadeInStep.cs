using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class FadeInStep : MonoBehaviour,IStep
{
    [SerializeField] private Image fadeImage; 
    [SerializeField] private float fadeDuration = 1f;

    public UnityEvent onStartStep;
    public UnityEvent onEndStep;    
    public IEnumerator Execute()
    {
        if(fadeImage == null)
        {
            Debug.LogWarning("Faltan referencias en TeleportStep.");
            yield break;
        }
        
        fadeImage.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(1));
    }
    
    private IEnumerator Fade(float targetAlpha)
    {
        onStartStep?.Invoke();
        fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        fadeImage.color = color;
        onEndStep?.Invoke();
    }
}
