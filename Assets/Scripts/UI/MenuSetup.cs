using System.Collections.Generic;
using UnityEngine;

public class MenuSetup : MonoBehaviour
{
    [SerializeField] private List<LocalizedText> titles;


    private void OnEnable()
    {
        ApplyLocalizedText();
    }

    private void ApplyLocalizedText()
    {
        string lang = PlayerPrefs.GetString("language", "es");
        bool isEnglish = lang == "en";

        foreach (var entry in titles)
        {
            if (entry.targetText == null) continue;
            entry.targetText.text = isEnglish ? entry.english : entry.spanish;
        }
    }
}
