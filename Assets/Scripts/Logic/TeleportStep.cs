using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class TeleportStep : MonoBehaviour,IStep
{   
    [SerializeField] private Transform player;
    [SerializeField] private Transform destination;
    [SerializeField] private Image fadeImage; 
    [SerializeField] private float fadeDuration = 1f;

    public UnityEvent onStartStep { get; }
    public UnityEvent OnStartStep => onStartStep; // Implementación de la interfaz

    public IEnumerator Execute()
    {
        if (player == null || destination == null || fadeImage == null)
        {
            Debug.LogWarning("Faltan referencias en TeleportStep.");
            yield break;
        }

        fadeImage.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(1));

        // Teletransportar posición y rotación
        player.position = destination.position;
        player.rotation = destination.rotation;
        
    }

    private IEnumerator Fade(float targetAlpha)
    {
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
    }
}
