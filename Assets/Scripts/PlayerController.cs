using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Image fadeImage;

    private Tween currentTween;

    /// <summary>
    /// Anima el alpha del Image hasta el valor dado (0 = transparente, 1 = opaco).
    /// </summary>
    /// <param name="alpha">Valor de alpha destino</param>
    public void SetAlpha(float alpha, float duration, TweenCallback onComplete = null)
    {
        if (fadeImage == null) return;
        
        currentTween?.Kill();
        currentTween = fadeImage.DOFade(alpha, duration)
            .SetEase(Ease.Linear)
            .OnComplete(onComplete);
    }

    public void BlackScreen()
    {
        fadeImage.color = new Color(0, 0, 0, 1);

    }
}
