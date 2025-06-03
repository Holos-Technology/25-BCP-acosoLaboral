using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeelingButtons : MonoBehaviour
{
    [SerializeField] Image feelingImage;
    [SerializeField] TextMeshProUGUI feelingText;

    public void SetUp(Sprite feelingSprite, string text)
    {
        feelingImage.sprite = feelingSprite;
        feelingText.text = text;
    }

}
