using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class FadeOutStep : MonoBehaviour,IStep
{
    [SerializeField] private Image fadeImage; 
    [SerializeField] private float fadeDuration = 1f;
    public UnityEvent onStartStep;


    private void Start()
    {
        if (fadeImage != null)
        {
            // Asegurar que el fade inicia completamente opaco (alpha = 1)
            Color color = fadeImage.color;
            color.a = 1f;
            fadeImage.color = color;
            fadeImage.gameObject.SetActive(true);
        }
    }

    public IEnumerator Execute()
    {                
        onStartStep?.Invoke();

        if (fadeImage == null)
        {
            Debug.LogWarning("Faltan referencias en FadeOutStep.");
            yield break;
        }
        
        fadeImage.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(0));
    }
    
    private IEnumerator Fade(float targetAlpha)
    {
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

        if (targetAlpha == 0)
        {
            fadeImage.gameObject.SetActive(false);
        }
    }
}
